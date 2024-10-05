using UnityEditor;
using UnityEngine;

namespace Germinator
{
    [InitializeOnLoad]
    [CreateAssetMenu(menuName = "Germinator/EnemyBuilder")]
    public class EnemyBuilder : ScriptableObject
    {
        static EnemyBuilder()
        {
        }

        public enum BodyPart
        {
            Body,
            Weapon
        }

        public int key;

        public EnemyEntity.Data data;

        [SerializeReference]
        public EnemySpecie behaviour;

        #region Sprite Layers
        [SerializeField]
        [Header("Sprite Layers")]
        private Texture2D body;

        [SerializeField]
        private Texture2D weapon;
        #endregion
    }
}