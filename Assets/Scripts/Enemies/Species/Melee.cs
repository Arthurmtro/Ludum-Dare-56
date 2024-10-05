using UnityEngine;
using System;

namespace Germinator
{

    [Serializable]
    public class Melee : EnemySpecie
    {
        private Transform weapon;
        private Transform body;

        public Melee() { }

        public Melee(Transform weapon, Transform body)
        {
            this.weapon = weapon;
            this.body = body;
        }

        public override void OnSpawn(GameObject parent)
        {
            GameObject spriteObject = new GameObject(body.ToString());
            spriteObject.transform.parent = parent.transform;
            spriteObject.transform.localPosition = Vector3.zero;
            spriteObject.AddComponent<SpriteRenderer>();
        }

        public override void OnAttack(GameObject target)
        {
            weapon.rotation = Quaternion.Euler(0, 0, 0);

            float rotationSpeed = 100.0f;
            float rotationTarget = 360.0f;

            float rotation = 0.0f;
            while (rotation < rotationTarget)
            {
                rotation += rotationSpeed * Time.deltaTime;
                weapon.rotation = Quaternion.Euler(0, 0, rotation);
            }

        }

        public override void OnMove()
        {
        }

        public override void OnDeath()
        {

        }
    }

}