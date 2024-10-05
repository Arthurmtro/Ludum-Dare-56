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

        [Header("Arms tracking")]
        [SerializeField]
        private float armDistanceFromBody = 1f;

        [SerializeField]
        private float armMoveSpeed = 5f;

        [SerializeField]
        private float punchMoveSpeed = 10f;

        [SerializeField]
        private float punchDistance = 1f;

        public bool isPunching = false;

        [Header("Legs animation")]
        [SerializeField]
        private float legSwingSpeed = 5f;

        [SerializeField]
        private float legSwingAngle = 5f;

        public bool isWalking = false;

        private Vector3 initialEyesPosition;
        public Vector3 targetPosition;
        private Vector3 mousePositionInWorld;

        private int bodyDirection = 1;

        void Start()
        {
            initialEyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
        }

        void Update()
        {
            // Get the mouse position in world space
            mousePositionInWorld = targetPosition;
            mousePositionInWorld.z = 0f;

            BodyTracking();
            EyesTracking();
            ArmsTracking();
            LegsSwinging();

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Punch());
            }
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

        private void ArmsTracking()
        {
            Vector3 bodyPosition = spriteManager.GetBodyPart(BodyPart.Body).transform.position;
            Vector3 directionToMouse = (mousePositionInWorld - bodyPosition).normalized;

            TrackArm(BodyPart.LeftArm, directionToMouse, -1);
            TrackArm(BodyPart.RightArm, directionToMouse, 1);
        }

        private void TrackArm(BodyPart armPart, Vector3 directionToMouse, int armSide)
        {
            directionToMouse.x *= bodyDirection * armSide;

            Vector3 armPosition = spriteManager.GetBodyPart(armPart).transform.localPosition;
            Vector3 armTargetPosition =
                spriteManager.GetBodyPart(BodyPart.Body).transform.localPosition
                + directionToMouse * armDistanceFromBody;
            Vector3 directionToMouseWorld =
                mousePositionInWorld - spriteManager.GetBodyPart(armPart).transform.position;
            float angle =
                Mathf.Atan2(directionToMouseWorld.y, directionToMouseWorld.x) * Mathf.Rad2Deg;

            spriteManager.GetBodyPart(armPart).transform.localPosition = Vector3.Lerp(
                armPosition,
                armTargetPosition,
                armMoveSpeed * Time.deltaTime
            );
            spriteManager.GetBodyPart(armPart).transform.rotation = Quaternion.Euler(0, 0, angle);

            spriteManager.GetBodyPart(armPart).GetComponent<SpriteRenderer>().flipX =
                bodyDirection == 1;
            spriteManager.GetBodyPart(armPart).GetComponent<SpriteRenderer>().flipY =
                bodyDirection == 1;
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

        public IEnumerator Punch()
        {
            isPunching = true;

            // Get world positions for the initial and target punch positions
            Vector3 leftArmInitialPosition = spriteManager
                .GetBodyPart(BodyPart.LeftArm)
                .transform.position;
            Vector3 rightArmInitialPosition = spriteManager
                .GetBodyPart(BodyPart.RightArm)
                .transform.position;

            // Calculate punch target position (mouse position) with punchDistance
            Vector3 directionToMouse = (mousePositionInWorld - leftArmInitialPosition).normalized;
            Vector3 leftArmTargetPosition =
                leftArmInitialPosition + directionToMouse * punchDistance;

            directionToMouse = (mousePositionInWorld - rightArmInitialPosition).normalized;
            Vector3 rightArmTargetPosition =
                rightArmInitialPosition + directionToMouse * punchDistance;

            float punchDuration = 0.2f;
            float elapsedTime = 0f;

            // Move hands to the target (mouse) position with punch distance
            while (elapsedTime < punchDuration)
            {
                spriteManager.GetBodyPart(BodyPart.LeftArm).transform.position = Vector3.Lerp(
                    leftArmInitialPosition,
                    leftArmTargetPosition,
                    (elapsedTime / punchDuration)
                );
                spriteManager.GetBodyPart(BodyPart.RightArm).transform.position = Vector3.Lerp(
                    rightArmInitialPosition,
                    rightArmTargetPosition,
                    (elapsedTime / punchDuration)
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0f;

            // Move hands back to their initial positions
            while (elapsedTime < punchDuration)
            {
                spriteManager.GetBodyPart(BodyPart.LeftArm).transform.position = Vector3.Lerp(
                    leftArmTargetPosition,
                    leftArmInitialPosition,
                    (elapsedTime / punchDuration)
                );
                spriteManager.GetBodyPart(BodyPart.RightArm).transform.position = Vector3.Lerp(
                    rightArmTargetPosition,
                    rightArmInitialPosition,
                    (elapsedTime / punchDuration)
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset the isPunching flag once the punch is completed
            isPunching = false;
        }
    }
}
