using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Germinator
{
    public enum EnemyType : uint
    {
        Type1 = 1,
    }

    [Serializable]
    public struct EnemyInfo
    {
        public EnemyType type;
        public string name;
        public EnemyController prefab;
        public string hp;
        public float speed;
    }

    [InitializeOnLoad]
    [CreateAssetMenu(fileName = "Assets/Resources/EnemyCollection.asset", menuName = "Germinator/EnemyCollection")]
    public class EnemyCollection : ScriptableObject
    {
        [SerializeField] private EnemyInfo[] enemies = Array.Empty<EnemyInfo>();
        private readonly Dictionary<EnemyType, int> enemyIndices = new();

        public int Count => enemies.Length;
        public EnemyInfo[] All => enemies;
        public EnemyInfo AtIndex(int index) => enemies[index];
        public int GetIndex(EnemyType type) => enemyIndices[type];
        public EnemyInfo ByType(EnemyType type) => AtIndex(GetIndex(type));

        public void OnEnable()
        {
            // for (int i = 0; i < enemies.Length; i++)
            // {
            //     // enemyIndices.Add(enemies[i].type, i);
            // }
        }
    }
}
