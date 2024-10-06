using UnityEngine;
using System;

namespace Germinator
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rigidBody;
        private Vector2 movementDirection;

        private PlayerEntity playerEntity;
        private Entity.Data defaultValues = new Entity.Data();

        public bool isMoving = false;

        void Start()
        {
            playerEntity = GetComponent<PlayerEntity>();
            defaultValues = playerEntity.data;
        }

        void Update()
        {
            movementDirection = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            bool currentlyMoving = movementDirection != Vector2.zero;
            if (currentlyMoving && !isMoving)
            {
                isMoving = true;
            }
            else if (!currentlyMoving && isMoving)
            {
                isMoving = false;
            }
        }

        void FixedUpdate()
        {
            if (rigidBody != null && playerEntity != null)
            {
                rigidBody.velocity = playerEntity.data.moveSpeed * Time.deltaTime * movementDirection;
            }
        }

        public void SetMaxHealth(float maxHealth) => playerEntity.data.maxHealth = maxHealth;
        public void SetHealth(float health) => playerEntity.data.health = health;
        public void SetMoveSpeed(float moveSpeed) => playerEntity.data.moveSpeed = moveSpeed;

        public void SetAttackSpeed(float attackSpeed) => playerEntity.data.attack.speed = attackSpeed;
        public void SetAttackDamage(float attackDamage) => playerEntity.data.attack.damage = attackDamage;
        public void SetAttackRange(float attackRange) => playerEntity.data.attack.range = attackRange;
        public void SetAttackCooldown(float attackCooldown) => playerEntity.data.attack.cooldown = attackCooldown;

        public void Clear() => playerEntity.data = defaultValues;

        public void OnSpeedModifier() => SetMoveSpeed(playerEntity.data.moveSpeed * 1.10f);
        public void OnAttackSpeedModifier() => SetAttackSpeed(playerEntity.data.attack.speed * 1.10f);
        public void OnAttackDamageModifier() => SetAttackDamage(playerEntity.data.attack.damage * 1.10f);
    }
}
