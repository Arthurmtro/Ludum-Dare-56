using System;
using UnityEngine;


namespace Germinator
{
    public class EnemyManager : MonoBehaviour
    {
        #region Variables

        [Header("Input")]
        [SerializeField] private EnemyCollection enemyCollection;
        [SerializeField] private Transform playerTransform;

        private EnemyController[][] enemies = Array.Empty<EnemyController[]>();
        private int[] lastIndices = Array.Empty<int>();
        private readonly System.Random random = new();

        #endregion

        void Start()
        {
            enemies = new EnemyController[enemyCollection.Count][];
            lastIndices = new int[enemyCollection.Count];
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new EnemyController[0];
            }
        }

        public void Init(EnemyType type, int quantity)
        {
            var enemyInfo = enemyCollection.ByType(type);
            int index = enemyCollection.GetIndex(type);
            ClearEnemyType(index);
            enemies[index] = new EnemyController[quantity];
            for (int i = 0; i < quantity; i++)
            {
                EnemyController enemy = Instantiate(enemyInfo.prefab, transform);
                enemy.name = $"{enemyInfo.name} [{i}]";
                enemy.Initialize(enemyInfo);
                enemies[index][i] = enemy;
            }
        }

        public void InitOne()
        {
            Init(EnemyType.Type1, 100);
        }

        public void SpawnOne()
        {
            Spawn(EnemyType.Type1, 10);
        }

        // Spawns some enemies of a given definition
        public void Spawn(EnemyType type, int quantity)
        {
            int enemyIndex = enemyCollection.GetIndex(type);
            int remaining = quantity;
            int index = lastIndices[enemyIndex];
            int attempts = 0;
            var enemies = this.enemies[enemyIndex];
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

            lastIndices[enemyIndex] = index;
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

            enemies = Array.Empty<EnemyController[]>();
        }

        private void ClearEnemyType(int index)
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
                    if (enemy.IsActive)
                    {
                        enemy.MoveTowards(playerTransform.position);
                    }
                }
            }
        }

        #endregion
    }
}