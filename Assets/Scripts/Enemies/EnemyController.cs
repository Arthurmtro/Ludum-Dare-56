using System.Runtime.CompilerServices;
using UnityEngine;

namespace Germinator
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        #region Variables

        private Rigidbody2D rigidBody;
        private float speed = 1.0f;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();

        #endregion

        #region State

        public bool IsActive { get; set; }

        #endregion

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            randomSpeedMultiplier = 1 + (float)random.NextDouble();
        }

        public void Initialize(EnemyInfo info)
        {
            speed = info.speed;
        }

        public void MoveTowards(Vector3 position)
        {
            var direction = position - transform.position;

            // rigidBody.velocity = randomSpeedMultiplier * speed * direction.normalized;
            transform.position += randomSpeedMultiplier * speed * Time.deltaTime * direction.normalized;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            IsActive = false;
        }
    }
}