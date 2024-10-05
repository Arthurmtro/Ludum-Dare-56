using UnityEngine;

namespace Germinator
{
    public class Enemy : MonoBehaviour
    {
        #region Variables

        [Header("Stats")]
        [SerializeField] private int health;
        [SerializeField] private int maxHealth;
        [SerializeField][Range(0, 10)] private float speed;
        private float randomSpeedMultiplier;
        private readonly System.Random random = new();

        #endregion

        #region State

        public bool IsActive { get; set; }

        #endregion

        void Start()
        {
            randomSpeedMultiplier = 1 + (float)random.NextDouble();
            print(randomSpeedMultiplier);
        }



        public void MoveTowards(Vector3 position)
        {
            var direction = position - transform.position;

            transform.position += randomSpeedMultiplier * speed * Time.deltaTime * direction.normalized;
        }
    }
}