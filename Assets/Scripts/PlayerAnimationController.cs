using System.Collections;
using UnityEngine;

namespace Germinator
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Eyes animation")]
        [SerializeField]
        private float eyesMaxMoveRange = 0.3f;

        [SerializeField]
        private float eyesMoveSpeed = 2f;

        [Header("Legs animation")]
        [SerializeField]
        private float legSwingSpeed = 8f;

        [SerializeField]
        private float legSwingAngle = 10f;

        [Header("Body animation")]
        [SerializeField]
        private float bodyBobSpeed = 2f;

        [SerializeField]
        private float bodyBobAmount = 0.05f;

        private Vector3 initialEyesPosition;
        private Vector3 initialBodyPosition;
        public Vector3 targetPosition;

        private Transform bodyParts;
        private Transform eyes;
        private Transform body;
        private Transform leftLeg;
        private Transform rightLeg;

        public int bodyDirection = 1;

        private PlayerController playerController;

        void Start()
        {
            playerController = GetComponent<PlayerController>();

            bodyParts = transform.Find("BodyParts");
            eyes = bodyParts.Find("Eyes");
            body = bodyParts.Find("Body");
            leftLeg = bodyParts.Find("LeftLeg");
            rightLeg = bodyParts.Find("RightLeg");

            // Capture initial positions of the eyes and body
            initialEyesPosition = eyes.localPosition;
            initialBodyPosition = body.localPosition;
        }

        void Update()
        {
            // Ensure z-axis is always 0 to avoid depth changes
            targetPosition.z = 0f;

            // Execute the animations
            BodyTracking();
            EyesTracking();
            BodyBobbing();
            LegsSwinging();
        }

        private void BodyTracking()
        {
            Vector3 characterPosition = transform.position;
            Vector3 diff = targetPosition - characterPosition;

            // Flip the body parts based on the direction of movement
            if (diff.x < 0)
            {
                bodyParts.localScale = new Vector3(1f, 1f, 1f);
                bodyDirection = 1;
            }
            else
            {
                bodyParts.localScale = new Vector3(-1f, 1f, 1f);
                bodyDirection = -1;
            }
        }

        private void EyesTracking()
        {
            Vector3 eyesPosition = eyes.localPosition;
            Vector3 directionToTarget = (targetPosition - body.position).normalized;

            // Adjust eyes movement based on body direction
            directionToTarget.x *= bodyDirection;

            Vector3 eyesTargetPosition = initialEyesPosition + directionToTarget * eyesMaxMoveRange;

            // Lerp the eyes towards the target position
            eyes.localPosition = Vector3.Lerp(
                eyesPosition,
                eyesTargetPosition,
                eyesMoveSpeed * Time.deltaTime
            );
        }

        private void LegsSwinging()
        {
            // Reset legs if not moving
            if (!playerController.isMoving)
            {
                leftLeg.localRotation = Quaternion.identity;
                rightLeg.localRotation = Quaternion.identity;
                return;
            }

            // Animate leg swing if moving
            float timeFactor = Time.time * legSwingSpeed;
            float swingAngle = Mathf.Sin(timeFactor) * legSwingAngle;

            leftLeg.localRotation = Quaternion.Euler(0, 0, swingAngle);
            rightLeg.localRotation = Quaternion.Euler(0, 0, -swingAngle);
        }

        private void BodyBobbing()
        {
            // Reset body position if not moving
            if (!playerController.isMoving)
            {
                body.localPosition = initialBodyPosition;
                return;
            }

            // Apply bobbing movement while moving
            float bobOffset = Mathf.Sin(Time.time * bodyBobSpeed) * bodyBobAmount;
            body.localPosition = initialBodyPosition + new Vector3(0, bobOffset, 0);
        }
    }
}
