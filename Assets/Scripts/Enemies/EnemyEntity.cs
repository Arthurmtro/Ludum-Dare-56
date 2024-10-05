using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Germinator
{
    [Serializable]
    public class EnemyEntity : Entity
    {
        public EnemyEntity() : base(EntityType.Enemy)
        {
        }
    }
}
