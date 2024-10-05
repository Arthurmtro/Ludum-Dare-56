using UnityEngine;
using System;

namespace Germinator
{
    [Serializable]
    public abstract class EnemySpecie : MonoBehaviour
    {
        public EnemyBuilder builder;

        public abstract void OnSpawn(GameObject parent);
        public abstract void OnAttack(GameObject target);
        public abstract void OnMove();
        public abstract void OnDeath();
        public abstract void OnTick();
    }
}

