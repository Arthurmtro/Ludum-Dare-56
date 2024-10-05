using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Germinator
{
    public class CombatController : MonoBehaviour
    {
        private EntityData entity;
        private bool canAttack = true;
        private List<EntityData> enemiesInRange = new List<EntityData>();
        private EntityData closestEnemy = null;

        [SerializeField]
        private CircleCollider2D detectionCollider;

        [SerializeField]
        private BoxCollider2D visionCollider;

        [SerializeField]
        private AudioSource punchAudioSource;

        private PlayerAnimationController animationController;

        void Start()
        {
            entity = GetComponent<EntityData>();
            animationController = GetComponent<PlayerAnimationController>();

            UpdateColliderSizes();
        }

        void UpdateColliderSizes()
        {
            detectionCollider.radius = entity.AttackRange;

            visionCollider.size = new Vector2(entity.AttackRange, entity.AttackRange);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            EntityData targetEntity = collider.GetComponent<EntityData>();
            if (targetEntity != null && targetEntity.Type != entity.Type)
            {
                enemiesInRange.Add(targetEntity);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            EntityData targetEntity = collider.GetComponent<EntityData>();
            if (targetEntity != null && enemiesInRange.Contains(targetEntity))
            {
                enemiesInRange.Remove(targetEntity);
            }
        }

        private void Update()
        {
            UpdateClosestEnemy();
            RotateVisionConeTowardsClosestEnemy();
            UpdateColliderSizes();

            if (canAttack && closestEnemy != null)
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
                .OrderBy(e =>
                    Vector3.Distance(transform.position, e.GetComponent<Collider2D>().bounds.center)
                )
                .FirstOrDefault();
        }

        private void RotateVisionConeTowardsClosestEnemy()
        {
            if (closestEnemy == null)
                return;

            Vector3 direction = (
                closestEnemy.GetComponent<Collider2D>().bounds.center - transform.position
            ).normalized;
            animationController.targetPosition = closestEnemy
                .GetComponent<Collider2D>()
                .bounds.center;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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
                EntityData targetEntity = target.GetComponent<EntityData>();
                if (targetEntity != null && targetEntity.Type != entity.Type)
                {
                    Debug.Log("Punch : " + target.gameObject.name);
                    // Attack(targetEntity);
                }
            }

            // punchAudioSource.Play();
            Debug.Log("Punch Animation ");
            StartCoroutine(animationController.Punch());

            yield return new WaitForSeconds(entity.AttackSpeed);
            canAttack = true;
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (canAttack)
            {
                StartCoroutine(HandleAttack());
            }
        }

        public void Attack(EntityData targetEntity)
        {
            if (targetEntity != null)
            {
                targetEntity.OnTakeDamage(entity.AttackDamage);
            }
        }
    }
}
