using UnityEngine;

namespace Germinator
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        public Rigidbody2D rigidBody;
        private Vector2 movementDirection;

        private PlayerEntity playerEntity;

        void Start()
        {
            playerEntity = GetComponent<PlayerEntity>();
        }

        void Update()
        {
            movementDirection = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        void FixedUpdate()
        {
            if (rigidBody != null && playerEntity != null)
            {
                rigidBody.velocity = playerEntity.data.moveSpeed * Time.deltaTime * movementDirection;
            }
        }
    }
}
