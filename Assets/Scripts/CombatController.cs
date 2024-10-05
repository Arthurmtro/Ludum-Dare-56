using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Germinator
{
    public class CombatController : MonoBehaviour
    {
        [Header("Colliders")]
        [SerializeField]
        private CircleCollider2D detectionCollider;

        [SerializeField]
        private BoxCollider2D visionCollider;

        [Header("Audio")]
        [SerializeField]
        private AudioSource punchAudioSource;

        private EntityData entity;
        private PlayerAnimationController animationController;
        private bool canAttack = true;
        private readonly List<EntityData> enemiesInRange = new();
        private EntityData closestEnemy = null;

        private void Awake()
        {
            entity = GetComponent<EntityData>();
            animationController = GetComponent<PlayerAnimationController>();
        }

        private void Start()
        {
            UpdateColliderSizes();
        }

        private void Update()
        {
            UpdateColliderSizes();
            UpdateClosestEnemy();
            RotateVisionConeTowardsClosestEnemy();

            if (canAttack && closestEnemy != null)
            {
                StartCoroutine(HandleAttack());
            }
        }

        private void UpdateColliderSizes()
        {
            float attackRange = entity.AttackRange;
            detectionCollider.radius = attackRange;
            visionCollider.size = new Vector2(attackRange, attackRange);
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
                * (visionCollider.size.x / 2f);
            visionCollider.offset = offset;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            EntityData targetEntityData = collider.GetComponent<EntityData>();
            if (targetEntityData != null && targetEntityData.Type != entity.Type)
            {
                enemiesInRange.Add(targetEntityData);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            EntityData targetEntityData = collider.GetComponent<EntityData>();
            if (targetEntityData != null)
            {
                enemiesInRange.Remove(targetEntityData);
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (canAttack)
            {
                StartCoroutine(HandleAttack());
            }
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
                EntityData targetEntityData = target.GetComponent<EntityData>();
                if (targetEntityData != null && targetEntityData.Type != entity.Type)
                {
                    Debug.Log("Punch : " + target.gameObject.name);
                    // Attack(targetEntityData);
                }
            }

            punchAudioSource.Play();
            Debug.Log("Punch Animation");
            StartCoroutine(animationController.Punch());

            yield return new WaitForSeconds(entity.AttackSpeed);
            canAttack = true;
        }

        private void Attack(EntityData targetEntityData)
        {
            targetEntityData?.OnTakeDamage(entity.AttackDamage);
        }
    }
}
