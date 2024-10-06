using System.Collections;
using UnityEngine;

namespace Germinator
{
    public class PunchWeapon : Weapon
    {
        [SerializeField]
        private Transform leftPunchTransform;

        [SerializeField]
        private Transform rightPunchTransform;

        [SerializeField]
        private CombatController combatController;

        private Vector3 originalLeftArmLocalPosition;
        private Vector3 originalRightArmLocalPosition;
        private Quaternion originLeftArmLocalRotation;
        private Quaternion originRightArmLocalRotation;
        private Vector3 originalLeftArmScale;
        private Vector3 originalRightArmScale;

        private bool isLeftPunch = true;
        private bool isPunching = false;

        protected override void Awake()
        {
            base.Awake();
            // Store the original local positions, rotations, and scales relative to the player
            originalLeftArmLocalPosition = leftPunchTransform.localPosition;
            originalRightArmLocalPosition = rightPunchTransform.localPosition;

            originLeftArmLocalRotation = leftPunchTransform.localRotation;
            originRightArmLocalRotation = rightPunchTransform.localRotation;

            originalLeftArmScale = leftPunchTransform.localScale;
            originalRightArmScale = rightPunchTransform.localScale;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Enemy") && isPunching)
            {
                Entity targetEntity = collider.GetComponent<Entity>();
                if (targetEntity != null && combatController != null)
                {
                    combatController.Attack(targetEntity);
                }
            }
        }

        public override IEnumerator Attack(Entity target)
        {
            if (isPunching) { yield break; }

            isPunching = true;

            Vector3 lockedTargetPosition = target.GetComponent<Collider2D>().bounds.center;

            // Use left or right punch transform based on the current punch state
            Transform currentPunchTransform = isLeftPunch ? leftPunchTransform : rightPunchTransform;
            Vector3 currentArmLocalPositionOrigin = isLeftPunch ? originalLeftArmLocalPosition : originalRightArmLocalPosition;
            Quaternion currentArmLocalRotationOrigin = isLeftPunch ? originLeftArmLocalRotation : originRightArmLocalRotation;
            Vector3 originalScale = isLeftPunch ? originalLeftArmScale : originalRightArmScale;

            // Flip the arm if the target is above the hand
            if (lockedTargetPosition.y > currentPunchTransform.position.y)
            {
                currentPunchTransform.localScale = new Vector3(currentPunchTransform.localScale.x, -Mathf.Abs(currentPunchTransform.localScale.y), currentPunchTransform.localScale.z);
            }
            else
            {
                currentPunchTransform.localScale = new Vector3(currentPunchTransform.localScale.x, Mathf.Abs(currentPunchTransform.localScale.y), currentPunchTransform.localScale.z);
            }

            Vector3 direction = (lockedTargetPosition - currentPunchTransform.position).normalized;
            float punchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate towards the target
            currentPunchTransform.rotation = Quaternion.Euler(0, 0, punchAngle);

            float distanceToTarget = Vector3.Distance(currentPunchTransform.position, lockedTargetPosition);

            float attackSpeed = owner.data.attack.speed;
            float baseCooldown = owner.data.attack.baseCooldown;

            float timeToWait = baseCooldown / attackSpeed;
            float travelTime = Mathf.Min(distanceToTarget / attackSpeed, timeToWait);
            travelTime = Mathf.Min(travelTime, baseCooldown);

            // Move towards the target
            float elapsedTime = 0f;
            Vector3 startPosition = currentPunchTransform.position;
            Quaternion startRotation = currentPunchTransform.rotation;

            while (elapsedTime < travelTime)
            {
                // Interpolate the position and move the arm towards the target
                currentPunchTransform.position = Vector3.Lerp(
                    startPosition,
                    lockedTargetPosition,
                    elapsedTime / travelTime
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            PlayAttackSound();

            // After reaching the target, return the arm to its original local position and local rotation
            elapsedTime = 0f;
            startPosition = currentPunchTransform.position;
            startRotation = currentPunchTransform.rotation;

            isPunching = false;

            float returnTravelTime = Mathf.Min(Vector3.Distance(currentPunchTransform.localPosition, currentArmLocalPositionOrigin) / attackSpeed, timeToWait);
            returnTravelTime = Mathf.Min(returnTravelTime, baseCooldown);

            // Revert the flip on the Y-axis when returning to the original position
            currentPunchTransform.localScale = originalScale;

            while (elapsedTime < returnTravelTime)
            {
                // Lerp position and Slerp rotation back to the original local position and local rotation
                currentPunchTransform.localPosition = Vector3.Lerp(
                    currentPunchTransform.localPosition,
                    currentArmLocalPositionOrigin,  // Use local position relative to the player
                    elapsedTime / returnTravelTime
                );
                currentPunchTransform.localRotation = Quaternion.Slerp(
                    currentPunchTransform.localRotation,
                    currentArmLocalRotationOrigin,  // Use local rotation relative to the player
                    elapsedTime / returnTravelTime
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final local position and rotation are exact
            currentPunchTransform.localPosition = currentArmLocalPositionOrigin;
            currentPunchTransform.localRotation = currentArmLocalRotationOrigin;

            isLeftPunch = !isLeftPunch;  // Switch between left and right punches
        }
    }
}
