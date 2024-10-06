using System.Collections;
using UnityEngine;
namespace Germinator
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        #region Variables

        private Rigidbody2D rigidBody;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();

        public EnemyBuilder builder;
        public EnemySpecie specie;

        private GameObject target;

        #endregion

        #region State

        public bool IsActive { get; set; }
        private bool canAttack = true;
        private bool isAttacking = false;

        #endregion


        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;
            rigidBody.freezeRotation = true;
            randomSpeedMultiplier = 1 + (float)random.NextDouble();

            specie.OnSpawn(gameObject);
        }

        public void Initialize(GameObject target)
        {
            this.target = target;
        }

        void Update()
        {
            if (!IsActive || target == null) return;

            MoveTowards(target);

            if (isAttacking && specie.CompletedAttack())
            {
                FinishAttack();
            }
        }

        public void MoveTowards(GameObject target)
        {
            specie.OnTick();

            var direction = (Vector2)(target.transform.position - transform.position);

            if (direction.magnitude > builder.data.attack.range)
            {
                rigidBody.MovePosition(rigidBody.position + (randomSpeedMultiplier * builder.data.moveSpeed * Time.deltaTime * direction.normalized));
                specie.OnMove();
                return;
            }

            if (canAttack)
            {
                Attack();
            }
        }

        private void Attack()
        {
            isAttacking = true;
            canAttack = false;

            specie.OnAttack();
        }

        private void FinishAttack()
        {
            isAttacking = false;

            // Delegate damage handling to the specie
            // specie.DealDamage(target);

            StartCoroutine(AttackCooldown());
        }

        private IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(builder.data.attack.cooldown);
            canAttack = true;
        }
    }
}
