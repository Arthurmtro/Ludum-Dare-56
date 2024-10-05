using System.Collections;
using UnityEngine;

namespace Germinator
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Sprite manager")]
        public EntitySpriteManager spriteManager;

        [Header("Eyes animation")]
        [SerializeField]
        private float eyesMaxMoveRange = 0.3f;

        [SerializeField]
        private float eyesMoveSpeed = 2f;

        [Header("Legs animation")]
        [SerializeField]
        private float legSwingSpeed = 5f;

        [SerializeField]
        private float legSwingAngle = 5f;

        public bool isWalking = false;

        private Vector3 initialEyesPosition;
        public Vector3 targetPosition;
        private Vector3 mousePositionInWorld;

        public int bodyDirection = 1;

        void Start()
        {
            initialEyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
        }

        void Update()
        {
            mousePositionInWorld = targetPosition;
            mousePositionInWorld.z = 0f;

            BodyTracking();
            EyesTracking();
            LegsSwinging();
        }

        private void BodyTracking()
        {
            Vector3 characterPosition = transform.position;
            Vector3 directionToMouse = mousePositionInWorld - characterPosition;

            if (directionToMouse.x < 0)
            {
                spriteManager.bodyParts.transform.localScale = new Vector3(0.15f, 0.15f, 1);
                bodyDirection = 1;
            }
            else
            {
                spriteManager.bodyParts.transform.localScale = new Vector3(-0.15f, 0.15f, 1);
                bodyDirection = -1;
            }
        }

        private void EyesTracking()
        {
            Vector3 eyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
            Vector3 directionToMouse = (
                mousePositionInWorld - spriteManager.GetBodyPart(BodyPart.Body).transform.position
            ).normalized;
            directionToMouse.x *= bodyDirection;

            Vector3 eyesTargetPosition = initialEyesPosition + directionToMouse * eyesMaxMoveRange;
            spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition = Vector3.Lerp(
                eyesPosition,
                eyesTargetPosition,
                eyesMoveSpeed * Time.deltaTime
            );
        }

        private void LegsSwinging()
        {
            if (!isWalking)
            {
                return;
            }

            // Calculate a swinging angle using Mathf.Sin to oscillate over time
            float swingAngle = Mathf.Sin(Time.time * legSwingSpeed) * legSwingAngle;

            // Apply the swing angle to the left and right legs
            spriteManager.GetBodyPart(BodyPart.LeftLeg).transform.localRotation = Quaternion.Euler(
                0,
                0,
                swingAngle
            );
            spriteManager.GetBodyPart(BodyPart.RightLeg).transform.localRotation = Quaternion.Euler(
                0,
                0,
                -swingAngle
            );
        }
    }
}
