using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Germinator
{
    [Serializable]
    public class Entity : MonoBehaviour
    {
        #region

        public UnityEvent onKill = new();

        #endregion

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

        // Returns whether the entity died
        public bool OnTakeDamage(float damage)
        {
            data.health -= damage;

            OnHit();
            if (data.health <= 0)
            {
                IsActive = false;
                OnDie();
                return true;
            }

            return false;
        }

        public virtual void OnHit()
        {
        }

        public virtual void OnDie()
        {
        }
    }
}
