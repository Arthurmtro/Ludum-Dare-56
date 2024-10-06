using System.Collections;
using Unity.VisualScripting;
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
        private float punchSpeed = 5f;

        private Vector3 leftOriginalPosition;
        private Vector3 rightOriginalPosition;
        private Transform currentPunchTransform;
        private float currentPunchAngle;
        private bool isPunching = false;
        private bool isLeftPunch = true;

        protected override void Awake()
        {
            base.Awake();
            leftOriginalPosition = leftPunchTransform.localPosition;
            rightOriginalPosition = rightPunchTransform.localPosition;
        }

        private void Update()
        {
            // sortingOrder
            leftPunchTransform.GetComponent<SpriteRenderer>().sortingOrder = playerAnimationController.bodyDirection == 1 ? 6 : 2;
            rightPunchTransform.GetComponent<SpriteRenderer>().sortingOrder = playerAnimationController.bodyDirection == 1 ? 2 : 6;

            if (isPunching)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = playerAnimationController.bodyDirection;
                transform.localScale = localScale;

                currentPunchTransform.rotation = Quaternion.Euler(
                    0,
                    0,
                    currentPunchAngle - (playerAnimationController.bodyDirection == 1 ? 180f : 0f)
                );
            }
        }

        public override IEnumerator Attack(Entity target)
        {
            isPunching = true;
            Vector3 targetPosition = target.GetComponent<Collider2D>().bounds.center;

            currentPunchTransform = isLeftPunch ? leftPunchTransform : rightPunchTransform;
            Vector3 originalPosition = isLeftPunch ? leftOriginalPosition : rightOriginalPosition;

            Vector3 direction = (targetPosition - currentPunchTransform.position).normalized;
            currentPunchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + ((!isLeftPunch ? 50 : 110) * -playerAnimationController.bodyDirection);

            while (Vector3.Distance(currentPunchTransform.position, targetPosition) > 0.1f)
            {
                Debug.Log(
                    "Moving from world position : "
                        + Vector3.Distance(currentPunchTransform.position, targetPosition)
                );

                currentPunchTransform.position = Vector3.MoveTowards(
                    currentPunchTransform.position,
                    targetPosition,
                    punchSpeed * Time.deltaTime
                );

                if (
                    Vector3.Distance(owner.transform.position, targetPosition)
                    > (owner.data.attack.range * 4)
                )
                {
                    break;
                }

                yield return null;
            }

            PlayAttackSound();

            while (Vector3.Distance(currentPunchTransform.localPosition, originalPosition) > 0.1f)
            {
                Debug.Log(
                    "Moving from local position"
                        + Vector3.Distance(currentPunchTransform.position, targetPosition)
                );
                Vector3 previousPosition = currentPunchTransform.localPosition;
                currentPunchTransform.localPosition = Vector3.MoveTowards(
                    currentPunchTransform.localPosition,
                    originalPosition,
                    punchSpeed * Time.deltaTime
                );

                if (
                    Vector3.Distance(previousPosition, currentPunchTransform.localPosition) < 0.001f
                )
                {
                    break;
                }

                yield return null;
            }

            currentPunchTransform.localRotation = Quaternion.Euler(0, 0, (!isLeftPunch ? 8.5f : 15) * -playerAnimationController.bodyDirection);

            isPunching = false;
            isLeftPunch = !isLeftPunch;
        }
    }
}
