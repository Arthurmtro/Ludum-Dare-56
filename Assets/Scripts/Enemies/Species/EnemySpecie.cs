using UnityEngine;
using System;

namespace Germinator
{
    [Serializable]
    public abstract class EnemySpecie : ScriptableObject
    {
        public abstract void OnSpawn(GameObject parent);
        public abstract void OnAttack(GameObject target);
        public abstract void OnMove();
        public abstract void OnDeath();
    }
}

