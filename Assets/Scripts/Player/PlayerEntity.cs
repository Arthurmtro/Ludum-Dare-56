using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Germinator
{

    public class PlayerEntity : Entity
    {
        #region events

        public UnityEvent onDie = new();

        #endregion

        PlayerEntity() : base(EntityType.Player)
        {
        }

        public override void OnHit()
        {
            Debug.Log("ON PLAYER HIT");
        }
        public override void OnDie()
        {
            onDie.Invoke();
        }
    }
}
