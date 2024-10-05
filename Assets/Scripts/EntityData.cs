using UnityEngine;

namespace Germinator
{
    public abstract class EntityData : MonoBehaviour
    {
        public enum EntityType
        {
            Player,
            Enemy
        }

        #region Stats Properties
        protected EntityType entityType;

        [Header("Entity Stats")]
        [SerializeField]
        [Tooltip("The health of the entity.")]
        protected float health = 100f;

        [SerializeField]
        [Tooltip("The attack speed of the entity.")]
        protected float attackSpeed = 1f;

        [SerializeField]
        [Tooltip("The attack damage of the entity.")]
        protected float attackDamage = 1f;

        [SerializeField]
        [Tooltip("The movement speed of the entity.")]
        protected float moveSpeed = 5f;

        [SerializeField]
        [Tooltip("The attack range for the entity's circle collider.")]
        protected float attackRange = 1.5f;
        #endregion


        #region Getters and Setters
        public float Health
        {
            get => health;
            set => health = Mathf.Max(0, value);
        }
        public float AttackSpeed
        {
            get => attackSpeed;
            set => attackSpeed = Mathf.Max(0, value);
        }
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0, value);
        }
        public float AttackRange
        {
            get => attackRange;
            set => attackRange = Mathf.Max(0, value);
        }

        public float AttackDamage
        {
            get => attackDamage;
            set => attackDamage = Mathf.Max(0, value);
        }

        public EntityType Type
        {
            get => entityType;
        }
        #endregion

        public void OnAttack() { }

        public void OnTakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
            {
                OnDie();
            }
        }

        public void OnDie()
        {
            Destroy(gameObject);
        }
    }
}
