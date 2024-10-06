using UnityEngine;
using UnityEngine.Events;

namespace Germinator
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rigidBody;
        private Vector2 movementDirection;

        public PlayerEntity entity;

        #region events

        public UnityEvent onDie => entity.onDie;

        #endregion

        private Entity.Data defaultValues = new Entity.Data();

        void Start()
        {
            entity = GetComponent<PlayerEntity>();
            defaultValues = entity.data;
        }

        void Update()
        {
            movementDirection = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        void FixedUpdate()
        {
            if (!entity.IsActive)
            {
                return;
            }
            if (rigidBody != null && entity != null)
            {
                rigidBody.velocity = entity.data.moveSpeed * Time.deltaTime * movementDirection;
            }
        }

        public void SetMaxHealth(float maxHealth) => entity.data.maxHealth = maxHealth;
        public void SetHealth(float health) => entity.data.health = health;
        public void SetMoveSpeed(float moveSpeed) => entity.data.moveSpeed = moveSpeed;

        public void SetAttackSpeed(float attackSpeed) => entity.data.attack.speed = attackSpeed;
        public void SetAttackDamage(float attackDamage) => entity.data.attack.damage = attackDamage;
        public void SetAttackRange(float attackRange) => entity.data.attack.range = attackRange;
        public void SetAttackCooldown(float attackCooldown) => entity.data.attack.cooldown = attackCooldown;

        public void Clear() => entity.data = defaultValues;

        public void OnSpeedModifier() => SetMoveSpeed(entity.data.moveSpeed * 1.10f);
        public void OnAttackSpeedModifier() => SetAttackSpeed(entity.data.attack.speed * 1.10f);
        public void OnAttackDamageModifier() => SetAttackDamage(entity.data.attack.damage * 1.10f);
    }
}
