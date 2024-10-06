using UnityEngine;

namespace Germinator
{

    [System.Serializable]
    public class Melee : EnemySpecie
    {
        private GameObject body;
        private GameObject weapon;
        private bool isAttacking = false;
        private bool attackCompleted = false;
        private float attackRotationSpeed = 360f;
        private float attackDuration = 0.5f;  // Example attack duration

        private float attackTimer = 0f;


        public override void OnSpawn(GameObject parent)
        {
            // gameObject = parent;

            // // Body setup
            // body = new GameObject("Body");
            // body.transform.parent = gameObject.transform;
            // body.transform.localPosition = Vector3.zero;
            // body.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            // SpriteRenderer bodyRenderer = body.AddComponent<SpriteRenderer>();
            // bodyRenderer.sprite = Sprite.Create(builder.body, new Rect(0, 0, builder.body.width, builder.body.height), new Vector2(1f, 1f));
            // bodyRenderer.sortingOrder = 2;

            // // Weapon setup
            // weapon = new GameObject("Weapon");
            // weapon.transform.parent = gameObject.transform;
            // weapon.transform.localPosition = new Vector3(0.111f, 0.042f, 0f);
            // weapon.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            // SpriteRenderer weaponRenderer = weapon.AddComponent<SpriteRenderer>();
            // weaponRenderer.sprite = Sprite.Create(builder.weapon, new Rect(0, 0, builder.weapon.width, builder.weapon.height), new Vector2(1f, 1f));
            // weaponRenderer.sortingOrder = 1;
        }

        public override void OnAttack()
        {
            if (weapon == null) return;

            isAttacking = true;
            attackCompleted = false;
            attackTimer = 0f;  // Reset attack timer
        }

        public override void OnMove()
        {
            isAttacking = false;
        }

        public override void OnDeath()
        {
            // Handle death logic, such as animations or effects
        }

        public override void OnTick()
        {
            if (isAttacking)
            {
                RotateWeapon();
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackDuration)
                {
                    attackCompleted = true;
                }
            }
        }

        private void RotateWeapon()
        {
            if (weapon != null)
            {
                weapon.transform.Rotate(Vector3.forward * attackRotationSpeed * Time.deltaTime);
            }
        }

        public override bool CompletedAttack()
        {
            return attackCompleted;
        }

        // public override void DealDamage(GameObject target)
        // {
        //     // Implement damage logic here
        //     Debug.Log("Melee enemy dealt damage to " + target.name);
        //     // You can call the player's damage function here
        //     // target.GetComponent<Player>().TakeDamage(builder.data.attack.damage);
        // }
    }
}
