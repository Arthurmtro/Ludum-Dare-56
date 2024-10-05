using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Germinator
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        [Header("Target Settings")]
        [Tooltip("The target that the camera will follow.")]
        private Transform player;

        [SerializeField]
        [Tooltip("Distance between the player and the camera")]
        private float baseDistance = 5f;

        [SerializeField]
        [Header("Follow Settings")]
        [Tooltip("How smoothly the camera follows the target.")]
        private float smoothSpeed = 0.125f;

        [SerializeField]
        [Header("Zoom Settings")]
        [Tooltip("How smoothly the camera zooms in and out.")]
        private float zoomSmoothSpeed = 0.1f;

        [SerializeField]
        [Tooltip("The maximum zoom level")]
        private float maxZoom = 0.05f;

        [SerializeField]
        [Tooltip("Whether the camera should perform repetitive zoom in and out.")]
        private bool isZooming = false;

        [SerializeField]
        [Tooltip("Speed at which the camera oscillates between zoom levels.")]
        private float zoomOscillationSpeed = 6f;

        [SerializeField]
        [Header("Rotation Settings")]
        [Tooltip("Maximum angle to rotate ")]
        private float maxRotation = 1f;

        [SerializeField]
        [Tooltip("Rotation speed for reaching the desired angle.")]
        private float rotationSmoothSpeed = 0.1f;

        [SerializeField]
        [Tooltip("Whether the camera should perform repetitive left-right rotation.")]
        private bool isRotating = false;

        [SerializeField]
        [Tooltip("Speed at which the camera oscillates between angles.")]
        private float oscillationSpeed = 3f;

        private float targetZoom;
        private Camera cam;
        private Vector3 velocity = Vector3.zero;
        private Vector3 offset;
        private float currentRotationTime = 0f;
        private float currentZoomTime = 0f;

        void Start()
        {
            cam = GetComponent<Camera>();
            targetZoom = baseDistance;
        }

        void LateUpdate()
        {
            FollowPlayer();
            HandleZoom();
            HandleRotation();
        }

        private void FollowPlayer()
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                smoothSpeed
            );
            transform.position = smoothedPosition;
        }

        private void HandleZoom()
        {
            if (isZooming)
            {
                currentZoomTime += Time.deltaTime * zoomOscillationSpeed;
                float t = (Mathf.Sin(currentZoomTime) + 1f) / 2f;
                targetZoom = Mathf.Lerp(baseDistance, baseDistance + maxZoom, t);
            }
            else
            {
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                targetZoom -= scrollInput * zoomSmoothSpeed;
                targetZoom = Mathf.Clamp(targetZoom, baseDistance, baseDistance + maxZoom);
            }

            if (cam != null)
            {
                float smoothZoom = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSmoothSpeed);
                cam.orthographicSize = smoothZoom;
            }
        }

        private void HandleRotation()
        {
            if (isRotating)
            {
                currentRotationTime += Time.deltaTime * oscillationSpeed;
                float t = (Mathf.Sin(currentRotationTime) + 1f) / 2f;
                float angle = Mathf.Lerp(-maxRotation, maxRotation, t);
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                float smoothRotation = Mathf.LerpAngle(
                    transform.eulerAngles.z,
                    0f,
                    rotationSmoothSpeed
                );
                transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
            }
        }

        public void SetRotationState(bool enable)
        {
            isRotating = enable;
            if (!enable)
            {
                currentRotationTime = 0f;
            }
        }

        public void SetZoomState(bool enable)
        {
            isZooming = enable;
            if (!enable)
            {
                currentZoomTime = 0f;
            }
        }
    }
}
