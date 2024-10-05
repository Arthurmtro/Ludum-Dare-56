using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Germinator
{
    public class CombatController : MonoBehaviour
    {
        private Entity entity;
        private bool canAttack = true;
        private List<Entity> enemiesInRange = new List<Entity>();
        private Entity closestEnemy = null;

        [SerializeField]
        private BoxCollider2D visionCollider;

        private PlayerAnimationController animationController;

        void Start()
        {
            entity = GetComponent<Entity>();
            animationController = GetComponent<PlayerAnimationController>();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Entity targetEntity = collider.GetComponent<Entity>();
            if (targetEntity != null && targetEntity.Type != entity.Type)
            {
                enemiesInRange.Add(targetEntity);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            Entity targetEntity = collider.GetComponent<Entity>();
            if (targetEntity != null && enemiesInRange.Contains(targetEntity))
            {
                enemiesInRange.Remove(targetEntity);
            }
        }

        private void Update()
        {
            UpdateClosestEnemy();
            RotateVisionConeTowardsClosestEnemy();

            if (canAttack && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(HandleAttack());
            }
        }

        private void UpdateClosestEnemy()
        {
            if (enemiesInRange.Count == 0)
            {
                closestEnemy = null;
                return;
            }

            closestEnemy = enemiesInRange
                .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
                .FirstOrDefault();
        }

        private void RotateVisionConeTowardsClosestEnemy()
        {
            if (closestEnemy == null)
                return;

            Vector3 direction = (closestEnemy.transform.position - transform.position).normalized;
            animationController.targetPosition = closestEnemy.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Adjust offset to make sure the vision collider points towards the enemy without modifying the transform rotation
            Vector2 offset =
                new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))
                * (visionCollider.size.y / 2f);
            visionCollider.offset = offset;
        }

        private IEnumerator HandleAttack()
        {
            canAttack = false;

            Vector3 boxCenter = visionCollider.transform.TransformPoint(visionCollider.offset);
            Collider2D[] targetsInVision = Physics2D.OverlapBoxAll(
                boxCenter,
                visionCollider.size,
                visionCollider.transform.eulerAngles.z
            );
            foreach (Collider2D target in targetsInVision)
            {
                Entity targetEntity = target.GetComponent<Entity>();
                if (targetEntity != null && targetEntity.Type != entity.Type)
                {
                    Attack(targetEntity);
                }
            }

            yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
            canAttack = true;
        }

        public void Attack(Entity targetEntity)
        {
            if (targetEntity != null)
            {
                targetEntity.OnTakeDamage(entity.AttackSpeed);
            }
        }
    }
}
