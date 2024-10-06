using System;
using UnityEngine;

namespace Germinator
{
    public enum BodyPart
    {
        Eyes,
        Body,
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
        private Texture2D rightLeg;

        [SerializeField]
        private Texture2D leftLeg;
        #endregion

        public GameObject bodyParts;

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

            CreateSpriteRenderer(BodyPart.Body, body, 5, new Vector3(6f, 8f, 0f));
            CreateSpriteRenderer(BodyPart.Eyes, eyes, 7, new Vector3(1f, 3f, 0f));
            CreateSpriteRenderer(BodyPart.RightLeg, rightLeg, 4, new Vector3(-0.33f, -3.5f, 0f));
            CreateSpriteRenderer(BodyPart.LeftLeg, leftLeg, 3, new Vector3(3f, -3.5f, 0f));
        }

        private void CreateSpriteRenderer(BodyPart bodyPart, Texture2D texture, int sortingOrder, Vector3 defaultPosition)
        {
            GameObject spriteObject = new GameObject(bodyPart.ToString());
            spriteObject.transform.parent = bodyParts.transform;
            spriteObject.transform.localPosition = defaultPosition;
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

            int width = texture.width;
            int height = texture.height;

            spriteRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(1f, 1f)
            );
            spriteRenderer.sortingOrder = sortingOrder;
        }

        public GameObject GetBodyPart(BodyPart bodyPart)
        {
            Transform bodyPartTransform = bodyParts.transform.Find(bodyPart.ToString());
            return bodyPartTransform.gameObject;
        }
    }
}
