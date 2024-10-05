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

        [Header("Legs animation")]
        [SerializeField]
        private float legSwingSpeed = 5f;

        [SerializeField]
        private float legSwingAngle = 5f;

        public bool isWalking = false;

        private Vector3 initialEyesPosition;
        private Vector3 mousePositionInWorld;

        private int bodyDirection = 1;

        void Start()
        {
            initialEyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
        }

        void Update()
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Input.mousePosition;
            mousePositionInWorld = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePositionInWorld.z = 0f;

            BodyTracking();
            EyesTracking();
            ArmsTracking();
            LegsSwinging();
        }

        private void BodyTracking()
        {
            Vector3 characterPosition = transform.position;
            Vector3 directionToMouse = mousePositionInWorld - characterPosition;

            if (directionToMouse.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);  // Flip character to the left
                bodyDirection = 1;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);   // Flip character to the right
                bodyDirection = -1;
            }
        }

        private void EyesTracking()
        {
            Vector3 eyesPosition = spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition;
            Vector3 directionToMouse = (mousePositionInWorld - spriteManager.GetBodyPart(BodyPart.Body).transform.position).normalized;
            directionToMouse.x *= bodyDirection;

            Vector3 eyesTargetPosition = initialEyesPosition + directionToMouse * eyesMaxMoveRange;
            spriteManager.GetBodyPart(BodyPart.Eyes).transform.localPosition = Vector3.Lerp(eyesPosition, eyesTargetPosition, eyesMoveSpeed * Time.deltaTime);
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
            Vector3 armTargetPosition = spriteManager.GetBodyPart(BodyPart.Body).transform.localPosition + directionToMouse * armDistanceFromBody;
            Vector3 directionToMouseWorld = mousePositionInWorld - spriteManager.GetBodyPart(armPart).transform.position;
            float angle = Mathf.Atan2(directionToMouseWorld.y, directionToMouseWorld.x) * Mathf.Rad2Deg;

            spriteManager.GetBodyPart(armPart).transform.localPosition = Vector3.Lerp(armPosition, armTargetPosition, armMoveSpeed * Time.deltaTime);
            spriteManager.GetBodyPart(armPart).transform.rotation = Quaternion.Euler(0, 0, angle);

            spriteManager.GetBodyPart(armPart).GetComponent<SpriteRenderer>().flipX = bodyDirection == 1;
            spriteManager.GetBodyPart(armPart).GetComponent<SpriteRenderer>().flipY = bodyDirection == 1;
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
            spriteManager.GetBodyPart(BodyPart.LeftLeg).transform.localRotation = Quaternion.Euler(0, 0, swingAngle);
            spriteManager.GetBodyPart(BodyPart.RightLeg).transform.localRotation = Quaternion.Euler(0, 0, -swingAngle);
        }

    }
}