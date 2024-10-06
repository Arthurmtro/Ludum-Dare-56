using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Germinator
{
    [Serializable]
    public class Entity : MonoBehaviour
    {
        public bool IsActive { get; set; }

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
            public int index;
            public float maxHealth;
            public float health;
            public float moveSpeed;
            public float invincibilityTime;
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

            OnHit();
            if (data.health <= 0)
            {
                IsActive = false;
                OnDie();
            }
        }

        public virtual void OnHit()
        {
        }

        public virtual void OnDie()
        {
        }
    }
}
