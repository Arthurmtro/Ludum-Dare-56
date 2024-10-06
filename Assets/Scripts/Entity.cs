using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    [Serializable]
    public class Entity : MonoBehaviour
    {
        public enum EntityType
        {
            Player,
            Enemy
        }

        [Serializable]
        public struct Attack
        {
            public float speed;
            public float damage;
            public float range;
            public float cooldown;
        }

        [Serializable]
        public struct Data
        {
            public float maxHealth;
            public float health;
            public float moveSpeed;
            public Attack attack;
        }

        public EntityType type;
        public Data data;

        protected Entity(EntityType type)
        {
            this.type = type;
        }

        public void OnAttack() { }

        public void OnTakeDamage(float damage)
        {
            data.health -= damage;

            if (data.health <= 0)
            {
                OnDie();
            }
        }

        public void OnDie()
        {
        }
    }
}
