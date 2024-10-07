using System.Collections;
using UnityEngine;
namespace Germinator
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : Entity
    {
        public int key = 0;

        #region Variables

        private Rigidbody2D rigidBody;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();
        private Animator animator;

        private GameObject target;

        #endregion

        #region State

        private bool canAttack = true;
        private bool isAttacking = false;
        private float attackPerc = 0;
        private float attackCooldown = 0;

        #endregion

        public EnemyController() : base(EntityType.Enemy)
        {
        }

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;
            rigidBody.freezeRotation = true;
            randomSpeedMultiplier = 1 + (float)random.NextDouble();
            gameObject.tag = "Enemy";
            animator = GetComponent<Animator>();
        }

        public void Appear()
        {
            if (animator != null)
            {
                animator.ResetTrigger("Hit");
                animator.ResetTrigger("Death");
                animator.SetTrigger("Appear");
            }
        }

        public void MoveTowards(Entity target)
        {
            var direction = target.transform.position - transform.position;
            if (direction.magnitude > 1f)
            {
                float speed = direction.magnitude > data.attack.range ? randomSpeedMultiplier * data.moveSpeed : randomSpeedMultiplier * data.moveSpeed * 0.5f;

                transform.position += speed * Time.deltaTime * direction.normalized;

                // Avoid extra calculations with physics (we don't have anything that works with physics)
                // rigidBody.MovePosition(rigidBody.position + (speed * Time.deltaTime * direction.normalized));
            }
            if (direction.magnitude > data.attack.range)
            {
                StopAttack();
                return;
            }

            if (isAttacking)
            {
                attackPerc += Time.deltaTime * data.attack.speed;
                if (attackPerc > 1f)
                {
                    target.OnTakeDamage(data.attack.damage);
                    StopAttack();
                }
                return;
            }
            if (canAttack)
            {
                StartAttack();
                return;
            }

            attackCooldown += Time.deltaTime * data.attack.speed;
            if (attackCooldown > data.attack.baseCooldown)
            {
                canAttack = true;
            }
        }

        private void StartAttack()
        {
            isAttacking = true;
            canAttack = false;
            attackPerc = 0;
            attackCooldown = 0;
            if (animator != null)
            {
                animator.SetBool("Attacking", true);
            }
        }
        private void StopAttack()
        {
            isAttacking = false;
            canAttack = false;
            attackPerc = 0;
            attackCooldown = 0;
            if (animator != null)
            {
                animator.SetBool("Attacking", false);
            }
        }

        public override void OnHit()
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }

        public override void OnDie()
        {
            IsActive = false;
            if (animator != null)
            {
                animator.ResetTrigger("Hit");
                animator.ResetTrigger("Appear");
                animator.SetTrigger("Death");
            }
        }
    }
}
