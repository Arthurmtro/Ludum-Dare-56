using System;
using UnityEngine;

namespace Germinator
{
    public enum BodyPart
    {
        Eyes,
        Body,
        RightArm,
        LeftArm,
        RightLeg,
        LeftLeg
    }

    [ExecuteInEditMode]
    public class EntitySpriteManager : MonoBehaviour
    {
        [SerializeField]
        [Header("Player's entity")]
        private GameObject player = null;

        #region Sprite Layers
        [SerializeField]
        [Header("Player Sprite Layers")]
        private Texture2D body;

        [SerializeField]
        private Texture2D eyes;

        [SerializeField]
        private Texture2D rightArm;

        [SerializeField]
        private Texture2D leftArm;

        [SerializeField]
        private Texture2D rightLeg;

        [SerializeField]
        private Texture2D leftLeg;
        #endregion

        private GameObject bodyParts;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!player.TryGetComponent<PlayerAnimationController>(out var animationController))
            {
                animationController = player.AddComponent<PlayerAnimationController>();
            }
            animationController.spriteManager = this;
        }

        void Awake()
        {
            GenerateBodyParts();
        }

        public void GenerateBodyParts()
        {
            bodyParts = player.transform.Find("BodyParts")?.gameObject;
            if (bodyParts != null)
            {
                DestroyImmediate(bodyParts);
            }

            bodyParts = new GameObject("BodyParts");
            bodyParts.transform.parent = player.transform;
            bodyParts.transform.localPosition = Vector3.zero;
            bodyParts.transform.localRotation = Quaternion.identity;

            CreateSpriteRenderer(BodyPart.Body, body, 5);
            CreateSpriteRenderer(BodyPart.Eyes, eyes, 7);
            CreateSpriteRenderer(BodyPart.RightArm, rightArm, 6);
            CreateSpriteRenderer(BodyPart.LeftArm, leftArm, 2);
            CreateSpriteRenderer(BodyPart.RightLeg, rightLeg, 4);
            CreateSpriteRenderer(BodyPart.LeftLeg, leftLeg, 3);


            bodyParts.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
        }


        private void CreateSpriteRenderer(BodyPart bodyPart, Texture2D texture, int sortingOrder)
        {
            GameObject spriteObject = new GameObject(bodyPart.ToString());
            spriteObject.transform.parent = bodyParts.transform;
            spriteObject.transform.localPosition = Vector3.zero;
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

            int width = texture.width;
            int height = texture.height;

            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sortingOrder = sortingOrder;
        }

        public GameObject GetBodyPart(BodyPart bodyPart)
        {
            Transform bodyPartTransform = bodyParts.transform.Find(bodyPart.ToString());
            return bodyPartTransform.gameObject;
        }
    }
}