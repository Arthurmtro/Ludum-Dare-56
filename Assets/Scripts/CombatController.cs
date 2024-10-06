using System;
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

        [SerializeField]
        private GameObject weaponCollection;

        private Entity entity;
        private PlayerAnimationController animationController;
        private bool canAttack = true;
        private readonly List<Entity> enemiesInRange = new();
        private Entity closestEnemy = null;

        private void Awake()
        {
            entity = GetComponent<Entity>();
            animationController = GetComponent<PlayerAnimationController>();

            if (entity == null)
            {
                Debug.LogError("Entity component is missing from the GameObject.");
                enabled = false;
                return;
            }

            if (animationController == null)
            {
                Debug.LogError(
                    "PlayerAnimationController component is missing from the GameObject."
                );
                enabled = false;
                return;
            }

            if (detectionCollider == null)
            {
                Debug.LogError("Detection Collider is not assigned.");
                enabled = false;
                return;
            }

            if (visionCollider == null)
            {
                Debug.LogError("Vision Collider is not assigned.");
                enabled = false;
                return;
            }

            if (weaponCollection == null)
            {
                Debug.LogError("Weapon Collection GameObject is not assigned.");
                enabled = false;
                return;
            }

            entity.IsActive = true;
        }

        private void Start()
        {
            UpdateColliderSizes();
        }

        private void Update()
        {
            if (entity == null || !entity.IsActive)
            {
                return;
            }

            UpdateColliderSizes();
            UpdateClosestEnemy();
            RotateVisionConeTowardsClosestEnemy();

            if (canAttack && closestEnemy != null)
            {
                StartCoroutine(HandleAttack());
            }
        }

        private Weapon GetActiveWeapon()
        {
            if (weaponCollection == null)
                return null;

            foreach (Transform child in weaponCollection.transform)
            {
                if (child == null)
                    continue;
                if (child.gameObject.activeSelf)
                {
                    Weapon weapon = child.GetComponent<Weapon>();
                    if (weapon != null)
                    {
                        return weapon;
                    }
                }
            }
            return null;
        }

        private void UpdateColliderSizes()
        {
            if (detectionCollider == null || visionCollider == null || entity == null)
                return;

            float attackRange = entity.data.attack.range;
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
                .Where(e => e.IsActive && e != null && e.GetComponent<Collider2D>() != null)
                .OrderBy(e =>
                    Vector3.Distance(transform.position, e.GetComponent<Collider2D>().bounds.center)
                )
                .FirstOrDefault();
        }

        private void RotateVisionConeTowardsClosestEnemy()
        {
            if (closestEnemy == null || visionCollider == null)
                return;

            Collider2D enemyCollider = closestEnemy.GetComponent<Collider2D>();
            if (enemyCollider == null)
                return;

            Vector3 direction = (enemyCollider.bounds.center - transform.position).normalized;
            animationController.targetPosition = enemyCollider.bounds.center;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Vector2 offset =
                new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))
                * (visionCollider.size.x / 2f);
            visionCollider.offset = offset;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!entity.IsActive)
            {
                return;
            }

            Entity targetEntity = collider.GetComponent<Entity>();
            if (targetEntity != null && targetEntity.type != entity.type)
            {
                enemiesInRange.Add(targetEntity);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (!entity.IsActive)
            {
                return;
            }

            Entity targetEntity = collider.GetComponent<Entity>();
            if (targetEntity != null)
            {
                enemiesInRange.Remove(targetEntity);
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (!entity.IsActive)
            {
                return;
            }

            if (canAttack && closestEnemy != null && visionCollider != null)
            {
                StartCoroutine(HandleAttack());
            }
        }

        private IEnumerator HandleAttack()
        {
            if (!entity.IsActive)
            {
                yield return null;
            }

            canAttack = false;

            Weapon activeWeapon = GetActiveWeapon();
            if (activeWeapon != null && closestEnemy != null)
            {
                StartCoroutine(activeWeapon.Attack(closestEnemy));
            }

            yield return new WaitForSeconds(entity != null ? entity.data.attack.speed : 0f);
            canAttack = true;
        }

        public void Attack(Entity targetEntity)
        {
            if (targetEntity == null || entity?.data.attack == null)
                return;

            bool didKill = targetEntity.OnTakeDamage(entity.data.attack.damage);
            if (didKill)
            {
                entity.onKill.Invoke();
            }
        }
    }
}
