using UnityEngine;

namespace Germinator
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float movementSpeed = 2f;
        public Rigidbody2D rb;
        private Vector2 movementDirection;

        void Update()
        {
            movementDirection = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        void FixedUpdate()
        {
            rb.velocity = movementDirection * movementSpeed * Time.deltaTime;
        }
    }
}
