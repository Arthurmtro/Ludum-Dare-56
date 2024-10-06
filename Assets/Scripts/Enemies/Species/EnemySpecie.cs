using UnityEngine;
using System;

namespace Germinator
{
    [Serializable]
    public abstract class EnemySpecie : MonoBehaviour
    {
        protected new GameObject gameObject;
        public EnemyBuilder builder;

        public abstract void OnSpawn(GameObject parent);
        public abstract void OnAttack();
        public abstract void OnTick();
        public abstract void OnMove();
        public abstract void OnDeath();

        public abstract bool CompletedAttack();
    }
}

