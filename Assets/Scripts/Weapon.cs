using System.Collections;
using UnityEngine;

namespace Germinator
{
    public abstract class Weapon : MonoBehaviour
    {
        [Header("Weapon Properties")]
        [SerializeField]
        protected AudioSource audioSource;

        protected Entity owner;

        protected PlayerAnimationController playerAnimationController;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            owner = GetComponentInParent<Entity>();
            playerAnimationController = GetComponentInParent<PlayerAnimationController>();
        }

        public abstract IEnumerator Attack(Entity target);

        protected void PlayAttackSound()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
