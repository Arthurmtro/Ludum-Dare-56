using UnityEngine;

namespace Germinator
{
    public abstract class EnemyBuilder : MonoBehaviour
    {
        public enum BodyPart
        {
            Body,
            Weapon
        }

        public EnemyEntity entity;


        #region Sprite Layers
        [SerializeField]
        [Header("Sprite Layers")]
        private Texture2D body;

        [SerializeField]
        private Texture2D weapon;

        #endregion
    }
}