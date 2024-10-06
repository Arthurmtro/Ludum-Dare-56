using UnityEngine;
using System;

namespace Germinator
{

    [Serializable]
    public class Melee : EnemySpecie
    {
        private GameObject body;
        private GameObject weapon;

        private bool isAttacking = false;

        public override void OnSpawn(GameObject parent)
        {
            // Body
            body = new();
            body.name = "Body";
            body.transform.parent = gameObject.transform;
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            SpriteRenderer bodyRenderer = body.AddComponent<SpriteRenderer>();
            bodyRenderer.sprite = Sprite.Create(
                builder.body,
                new Rect(0, 0, builder.body.width, builder.body.height),
                new Vector2(1f, 1f)
            );
            bodyRenderer.sortingOrder = 2;

            // Weapon
            weapon = new();
            weapon.name = "Weapon";
            weapon.transform.parent = gameObject.transform;
            weapon.transform.localPosition = new Vector3(0.111f, 0.042f, 0f);
            weapon.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            SpriteRenderer weaponRenderer = weapon.AddComponent<SpriteRenderer>();
            weaponRenderer.sprite = Sprite.Create(
                builder.weapon,
                new Rect(0, 0, builder.weapon.width, builder.weapon.height),
                new Vector2(1f, 1f)
            );
            weaponRenderer.sortingOrder = 1;
        }

        public override void OnAttack(GameObject target)
        {
            if (weapon == null)
            {
                return;
            }

            isAttacking = true;
            // weapon.transform.rotation = Quaternion.Euler(0, 0, 0);

            // float rotationSpeed = 100.0f;
            // float rotationTarget = 360.0f;

            // float rotation = 0.0f;
            // while (rotation < rotationTarget)
            // {
            //     rotation += rotationSpeed * Time.deltaTime;
            //     weapon.transform.rotation = Quaternion.Euler(0, 0, rotation);
            // }
        }

        public override void OnMove()
        {
            isAttacking = false;
        }

        public override void OnDeath()
        {

        }

        public override void OnTick()
        {
            if (isAttacking)
            {
                // float rotationSpeed = 100.0f;
                // float rotationTarget = weapon.transform.rotation.z + Time.deltaTime;

                // // float rotation = 0.0f;
                // while (rotation < rotationTarget)
                // {
                // rotation += rotationSpeed * Time.deltaTime;
                // weapon.transform.rotation = Quaternion.Euler(0, 0, rotationTarget);
                // }
            }
        }
    }

}