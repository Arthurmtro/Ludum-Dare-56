using System;
using UnityEngine;


namespace Germinator
{
    public class EnemyManager : MonoBehaviour
    {
        #region Variables

        public PlayerController player;

        private EnemyController[][] enemies = Array.Empty<EnemyController[]>();
        public int usedEnemiesTypes;

        private int[] lastIndices = Array.Empty<int>();
        private readonly System.Random random = new();
        private bool isActive = false;

        #endregion

        public void Init()
        {
            enemies = new EnemyController[usedEnemiesTypes][];
            lastIndices = new int[usedEnemiesTypes];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new EnemyController[0];
            }
        }

        public void InitEnemySpecies(EnemyController prefab, int quantity)
        {
            // var enemyInfo = enemyCollection.ByType(builder);
            // int index = enemyCollection.GetIndex(builder);
            int index = prefab.data.index;
            ClearEnemyBuilder(index);
            enemies[index] = new EnemyController[quantity];
            for (int i = 0; i < quantity; i++)
            {
                // GameObject enemy = new();
                // enemyController.builder = builder;
                // enemy.transform.parent = transform;
                // Debug.Log($"Spawning enemy {enemy.name}");
                // builder.behaviour.OnSpawn(enemy);
                GameObject enemy = Instantiate(prefab.gameObject, transform);
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                // EnemySpecie enemySpecie = enemy.GetComponent<EnemySpecie>();
                // enemySpecie.builder = builder;

                // EnemyController enemyController = enemy.AddComponent<EnemyController>();
                // enemyController.builder = builder;
                // enemyController.specie = enemySpecie;

                // CircleCollider2D circleCollider2D = enemy.AddComponent<CircleCollider2D>();
                // circleCollider2D.radius = 0.5f;
                // circleCollider2D.offset = new Vector2(-0.28f, -0.29f);
                // circleCollider2D.isTrigger = true;

                // EnemyEntity enemyEntity = enemy.AddComponent<EnemyEntity>();
                // enemyEntity.data = enemyController.prefab.data;

                // enemySpecie.OnSpawn(enemy);

                enemy.name = $"{prefab.name} [{i}]";
                enemies[index][i] = enemyController;
            }
        }


        public void SetActive(bool value)
        {
            isActive = value;
        }


        public void InitOne()
        {
            // Init(EnemyBuilder.Type1, 100);
        }

        public void SpawnOne()
        {
            // Spawn(EnemyBuilder.Type1, 10);
        }

        // Spawns some enemies of a given definition
        public void Spawn(EnemyController prefab, int quantity)
        {
            // int enemyIndex = enemyCollection.GetIndex(prefab);
            int remaining = quantity;
            int index = lastIndices[prefab.data.index];
            int attempts = 0;
            var enemies = this.enemies[prefab.data.index];
            Vector3 position = player.transform.position + (Vector3)GetRandomPosition(4);
            while (remaining > 0 && attempts < enemies.Length)
            {
                index %= enemies.Length;
                var enemy = enemies[index];
                if (!enemy.IsActive)
                {
                    enemy.transform.position = position + new Vector3((float)random.NextDouble() * 3f, (float)random.NextDouble() * 3f, 0f);
                    remaining--;
                    enemy.IsActive = true;
                }

                index++;
                attempts++;
            }

            lastIndices[prefab.data.index] = index;
        }

        public void Clear()
        {
            foreach (var definitionEnemies in enemies)
            {
                foreach (var enemy in definitionEnemies)
                {
                    Destroy(enemy.gameObject);
                }
            }

            enemies = new EnemyController[usedEnemiesTypes][];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new EnemyController[0];
            }
        }

        private void ClearEnemyBuilder(int index)
        {
            foreach (var enemy in enemies[index])
            {
                Destroy(enemy.gameObject);
            }
        }

        private Vector2 GetRandomPosition(float distance)
        {
            float angle = (float)(random.NextDouble() * 5.0 * Math.PI);

            return new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
        }

        #region Game loop

        public void Update()
        {
            if (!isActive)
            {
                return;
            }

            foreach (var definitionEnemies in enemies)
            {
                foreach (var enemy in definitionEnemies)
                {
                    // Check if active
                    if (enemy.IsActive)
                    {
                        enemy.MoveTowards(player.entity);
                    }
                }
            }
        }

        #endregion
    }
}