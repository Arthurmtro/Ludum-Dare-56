using UnityEngine;
using System;
using UnityEngine.Events;

namespace Germinator
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rigidBody;
        private Vector2 movementDirection;

        [HideInInspector]
        public PlayerEntity entity;
        [HideInInspector]
        public bool isMoving = false;

        private PlayerAnimationController animationController;

        [Serializable]
        public struct SoundsClip
        {
            public AudioClip hurt;
        }

        public SoundsClip soundsClip;

        public AudioSource audioSource;

        public enum Sounds
        {
            Hurt,
        }

        #region events

        public UnityEvent onKill => entity.onKill;
        public UnityEvent onHit => entity.onHit;
        public UnityEvent onDie => entity.onDie;

        #endregion

        private Entity.Data defaultValues = new Entity.Data();


        void Awake()
        {
            entity = GetComponent<PlayerEntity>();
            animationController = GetComponent<PlayerAnimationController>();
            defaultValues = entity.data;
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
            if (!entity.IsActive)
            {
                rigidBody.velocity = Vector3.zero;
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

        public void Clear() => entity.data = defaultValues;

        public void OnSpeedModifier() => SetMoveSpeed(entity.data.moveSpeed * 1.10f);
        public void OnAttackSpeedModifier() => SetAttackSpeed(entity.data.attack.speed / 1.50f);
        public void OnAttackDamageModifier() => SetAttackDamage(entity.data.attack.damage * 1.10f);

        public void ChangeBodyColor(Color targetColor, float transitionSpeed) => StartCoroutine(animationController.ChangeBodyColor(targetColor, transitionSpeed));

        public void PlaySound(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.Hurt:
                    if (soundsClip.hurt != null)
                    {
                        audioSource.PlayOneShot(soundsClip.hurt);
                    }
                    break;
            }
        }
    }
}
