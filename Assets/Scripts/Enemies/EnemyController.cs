using UnityEngine;

namespace Germinator
{
    public class EnemyController : MonoBehaviour
    {
        #region Variables

        private float speed = 1.0f;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();

        #endregion

        #region State

        public bool IsActive { get; set; }

        #endregion

        void Start()
        {
            randomSpeedMultiplier = 1 + (float)random.NextDouble();
        }

        public void Initialize(EnemyInfo info)
        {
            speed = info.speed;
        }

        public void MoveTowards(Vector3 position)
        {
            var direction = position - transform.position;

            transform.position += randomSpeedMultiplier * speed * Time.deltaTime * direction.normalized;
        }
    }
}