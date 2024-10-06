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

        }

        public void Initialize(GameObject target)
        {
            this.target = target;
        }

        public void MoveTowards(GameObject target)
        {

            var direction = (Vector2)(target.transform.position - transform.position);

            if (direction.magnitude > data.attack.range)
            {
                rigidBody.MovePosition(rigidBody.position + (randomSpeedMultiplier * data.moveSpeed * Time.deltaTime * direction.normalized));
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

        }

        public override void OnDie()
        {
            Debug.Log("ON DIE");
            IsActive = false;
        }


    }
}
