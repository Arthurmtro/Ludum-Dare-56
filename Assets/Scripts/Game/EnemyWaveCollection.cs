using System;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    [InitializeOnLoad]
    [CreateAssetMenu(fileName = "Assets/Resources/EnemyWaveCollection.asset", menuName = "Germinator/EnemyWaveCollection")]
    public class EnemyWaveCollection : ScriptableObject
    {
        [SerializeField] private EnemyWave[] waves = Array.Empty<EnemyWave>();

        public int Count => waves.Length;
        public EnemyWave[] All => waves;
        public EnemyWave AtIndex(int index) => waves[index];

        public void OnEnable()
        {
        }
    }

}