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
        }

        public void Initialize(GameObject target)
        {
            this.target = target;
        }

        public void MoveTowards(Entity target)
        {

            var direction = (Vector2)(target.transform.position - transform.position);
            if (direction.magnitude > 1f)
            {
                float speed = direction.magnitude > data.attack.range ? randomSpeedMultiplier * data.moveSpeed : randomSpeedMultiplier * data.moveSpeed * 0.5f;
                rigidBody.MovePosition(rigidBody.position + (speed * Time.deltaTime * direction.normalized));
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
            if (attackCooldown > data.attack.cooldown)
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
        }
        private void StopAttack()
        {
            isAttacking = false;
            canAttack = false;
            attackPerc = 0;
            attackCooldown = 0;
        }

        public override void OnDie()
        {
            IsActive = false;
        }
    }
}
