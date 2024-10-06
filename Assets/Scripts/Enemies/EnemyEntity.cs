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

        public override void OnDie()
        {
            if (TryGetComponent<EnemyController>(out var controller))
            {
                controller.IsActive = false;
            }
            gameObject.SetActive(false);
        }
    }
}
