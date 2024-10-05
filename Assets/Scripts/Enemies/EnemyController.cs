using UnityEngine;

namespace Germinator
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        #region Variables

        private Rigidbody2D rigidBody;
        // private float speed = 1.0f;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();

        public EnemyBuilder builder;
        public EnemySpecie specie;

        #endregion

        #region State

        public bool IsActive { get; set; }

        #endregion


        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0;
            // randomSpeedMultiplier = 1 + (float)random.NextDouble();
            randomSpeedMultiplier = 1;
        }

        public void Initialize(EnemyInfo info)
        {
            // speed = info.speed;
        }

        public void MoveTowards(Vector3 position)
        {
            var direction = position - transform.position;

            // Far away, move to player
            if (direction.magnitude > 2)
            {
                transform.position += randomSpeedMultiplier * builder.data.moveSpeed * Time.deltaTime * direction.normalized;
                specie.OnMove();
                specie.OnTick();
                // rigidBody.velocity = randomSpeedMultiplier * speed * direction.normalized;
                return;
            }

            // Attack
            specie.OnAttack(transform.gameObject);
            specie.OnTick();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            IsActive = false;
        }
    }
}