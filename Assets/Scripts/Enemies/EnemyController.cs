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
        private readonly System.Random random;

        public EnemyBuilder builder;

        #endregion

        #region State

        public bool IsActive { get; set; }

        #endregion


        public EnemyController()
        {
            // this.builder = builder;
            random = new();


        }

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
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

            // rigidBody.velocity = randomSpeedMultiplier * speed * direction.normalized;
            transform.position += randomSpeedMultiplier * builder.data.moveSpeed * Time.deltaTime * direction.normalized;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            IsActive = false;
        }
    }
}