using System;
using UnityEngine;


namespace Germinator
{
    public class EnemyManager : MonoBehaviour
    {
        #region Variables

        // [Header("Input")]
        // [SerializeField] private EnemyCollection enemyCollection;
        public Transform playerTransform;

        private EnemyController[][] enemies = Array.Empty<EnemyController[]>();
        public int usedEnemiesTypes;

        private int[] lastIndices = Array.Empty<int>();
        private readonly System.Random random = new();

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

        public void InitEnemySpecies(EnemyBuilder builder, int quantity)
        {
            // var enemyInfo = enemyCollection.ByType(builder);
            // int index = enemyCollection.GetIndex(builder);
            int index = builder.key;
            ClearEnemyBuilder(index);
            enemies[index] = new EnemyController[quantity];
            for (int i = 0; i < quantity; i++)
            {
                GameObject enemy = new();
                EnemyController enemyController = enemy.AddComponent<EnemyController>();
                enemyController.builder = builder;
                enemy.transform.parent = transform;
                Debug.Log($"Spawning enemy {enemy.name}");
                builder.behaviour.OnSpawn(enemy);
                // EnemyController enemy = Instantiate(gameObject.AddComponent<EnemyController>(), transform);

                // EnemyController enemy = Instantiate(enemyInfo.prefab, transform);
                enemy.name = $"{builder.name} [{i}]";
                // enemy.Initialize(enemyInfo);
                enemies[index][i] = enemyController;
            }
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
        public void Spawn(EnemyBuilder builder, int quantity)
        {
            // int enemyIndex = enemyCollection.GetIndex(builder);
            int remaining = quantity;
            int index = lastIndices[builder.key];
            int attempts = 0;
            var enemies = this.enemies[builder.key];
            Vector3 position = playerTransform.position + (Vector3)GetRandomPosition(4);
            while (remaining > 0 && attempts < enemies.Length)
            {
                index %= enemies.Length;
                var enemy = enemies[index];
                if (!enemy.IsActive)
                {
                    enemy.transform.position = position + new Vector3((float)random.NextDouble() * 1f, (float)random.NextDouble() * 1f, 0f);
                    remaining--;
                    enemy.IsActive = true;
                }

                index++;
                attempts++;
            }

            lastIndices[builder.key] = index;
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
            float angle = (float)(random.NextDouble() * 2.0 * Math.PI);

            return new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
        }

        #region Game loop

        public void Update()
        {
            foreach (var definitionEnemies in enemies)
            {
                foreach (var enemy in definitionEnemies)
                {
                    // Check if active
                    // if (enemy.IsActive)
                    // {
                    // enemy.MoveTowards(playerTransform.position);
                    // }
                }
            }
        }

        #endregion
    }
}