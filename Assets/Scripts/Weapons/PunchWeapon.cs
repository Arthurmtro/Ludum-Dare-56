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
        private float punchSpeed = 5f;

        [SerializeField]
        private CombatController combatController;


        private Vector3 leftOriginalPosition;
        private Vector3 rightOriginalPosition;
        private bool isLeftPunch = true;
        private bool isPunching = false;

        protected override void Awake()
        {
            base.Awake();
            leftOriginalPosition = leftPunchTransform.localPosition;
            rightOriginalPosition = rightPunchTransform.localPosition;
        }

        private void Update()
        {
            leftPunchTransform.GetComponent<SpriteRenderer>().sortingOrder = playerAnimationController.bodyDirection == 1 ? 6 : 2;
            rightPunchTransform.GetComponent<SpriteRenderer>().sortingOrder = playerAnimationController.bodyDirection == 1 ? 2 : 6;
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
            if (isPunching)
                yield break;

            isPunching = true;

            Vector3 lockedTargetPosition = target.GetComponent<Collider2D>().bounds.center;

            Transform currentPunchTransform = isLeftPunch ? leftPunchTransform : rightPunchTransform;
            Vector3 originalPosition = isLeftPunch ? leftOriginalPosition : rightOriginalPosition;

            Vector3 direction = (lockedTargetPosition - currentPunchTransform.position).normalized;
            float punchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            currentPunchTransform.rotation = Quaternion.Euler(0, 0, punchAngle - 270f);

            while (Vector3.Distance(currentPunchTransform.position, lockedTargetPosition) > 0.1f)
            {
                currentPunchTransform.position = Vector3.MoveTowards(
                    currentPunchTransform.position,
                    lockedTargetPosition,
                    punchSpeed * Time.deltaTime
                );
                yield return null;
            }

            PlayAttackSound();

            while (Vector3.Distance(currentPunchTransform.localPosition, originalPosition) > 0.1f)
            {
                currentPunchTransform.localPosition = Vector3.MoveTowards(
                    currentPunchTransform.localPosition,
                    originalPosition,
                    punchSpeed * Time.deltaTime
                );
                yield return null;
            }

            currentPunchTransform.localRotation = Quaternion.identity;

            isPunching = false;
            isLeftPunch = !isLeftPunch;
        }
    }
}
