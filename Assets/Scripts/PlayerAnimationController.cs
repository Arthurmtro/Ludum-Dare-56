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

        private PlayerController playerController;

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
        private Vector3 mousePositionInWorld;

        public int bodyDirection = 1;

        void Start()
        {
            playerController = GetComponent<PlayerController>();
            initialEyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
            initialBodyPosition = spriteManager.GetBodyPart(BodyPart.Body).transform.localPosition;
        }

        void Update()
        {
            mousePositionInWorld = targetPosition;
            mousePositionInWorld.z = 0f;

            BodyTracking();
            EyesTracking();
            BodyBobbing();
            LegsSwinging();
        }

        private void BodyTracking()
        {
            Vector3 characterPosition = transform.position;
            Vector3 directionToMouse = mousePositionInWorld - characterPosition;

            if (directionToMouse.x < 0)
            {
                spriteManager.bodyParts.transform.localScale = new Vector3(1f, 1f, 1);
                bodyDirection = 1;
            }
            else
            {
                spriteManager.bodyParts.transform.localScale = new Vector3(-1f, 1f, 1);
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
            if (!playerController.isMoving)
            {
                spriteManager.GetBodyPart(BodyPart.LeftLeg).transform.localRotation = Quaternion.identity;
                spriteManager.GetBodyPart(BodyPart.RightLeg).transform.localRotation = Quaternion.identity;
                return;
            }

            float timeFactor = Time.time * legSwingSpeed;
            float swingAngle = Mathf.Sin(timeFactor) * legSwingAngle;

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

        private void BodyBobbing()
        {
            if (!playerController.isMoving)
            {
                spriteManager.GetBodyPart(BodyPart.Body).transform.localPosition = initialBodyPosition;
                return;
            }

            float bobOffset = Mathf.Sin(Time.time * bodyBobSpeed) * bodyBobAmount;
            spriteManager.GetBodyPart(BodyPart.Body).transform.localPosition = initialBodyPosition + new Vector3(0, bobOffset, 0);
        }
    }
}
