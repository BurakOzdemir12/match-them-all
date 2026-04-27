using System;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.Static;
using Unity.Cinemachine;
using UnityEngine;

namespace _Project.Scripts.CameraScripts
{
    public class LobbyCameraController : MonoBehaviour
    {
        public static LobbyCameraController Instance { get; private set; }

        [Header("References")] [SerializeField]
        private CinemachineCamera cm;

        [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

        [Header("Zoom and Look speed values")] [SerializeField]
        private float zoomSpeed;

        [SerializeField] private float lookSpeed;

        [Header("Stable Camera positions")] [Tooltip("Plane Takeoff watch camera position.")] [SerializeField]
        private Transform stableCameraPosition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (cm == null)
                cm = GetComponent<CinemachineCamera>();

            if (orbitalFollow == null)
                orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        }

        private void OnEnable()
        {
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
        }

        private void HandlePlaneSpawned(PlaneSocketManager socketManager)
        {
            if (cm != null && socketManager != null)
            {
                // Assign tracking target to the newly spawned plane
                cm.Target.TrackingTarget = socketManager.transform;
            }
        }

        private void LateUpdate()
        {
            if (LobbyInputManager.Instance == null) return;

            float zoomValue = LobbyInputManager.Instance.zoomDelta;

            if (Mathf.Abs(zoomValue) > 0.01f)
            {
                float targetZoom = orbitalFollow.RadialAxis.Value - (zoomValue * zoomSpeed);
                orbitalFollow.RadialAxis.Value = Mathf.Clamp(targetZoom, orbitalFollow.RadialAxis.Range.x,
                    orbitalFollow.RadialAxis.Range.y);
            }

            Vector2 lookValue = LobbyInputManager.Instance.lookDelta;

            if (lookValue.sqrMagnitude > 0.01f)
            {
                orbitalFollow.HorizontalAxis.Value += lookValue.x * lookSpeed;

                float targetVertical = orbitalFollow.VerticalAxis.Value - (lookValue.y * lookSpeed);

                orbitalFollow.VerticalAxis.Value = Mathf.Clamp(targetVertical, orbitalFollow.VerticalAxis.Range.x,
                    orbitalFollow.VerticalAxis.Range.y);
            }
        }

        public void SwitchCameraTarget(Transform target = null)
        {
            cm.Target.TrackingTarget = stableCameraPosition;
        }
    }
}