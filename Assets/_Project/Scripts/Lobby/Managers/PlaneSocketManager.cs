using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Components;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class PlaneSocketManager : MonoBehaviour
    {
        [Header("Animation Settings")] [Tooltip("Takeoff point")] [SerializeField]
        private Vector3 takeoffPoint;

        [Tooltip("plane Fly end position")] [SerializeField]
        private Vector3 flyEndPos;

        [Tooltip("speed-> Move Anim duration")] [SerializeField]
        private float moveAnimDuration;

        [Tooltip("plane will dock this position")] [SerializeField]
        private Vector3 groundPos;

        [Tooltip("Plane dock anim duration")] [SerializeField]
        private float dockAnimDuration;

        [Tooltip("Speed multiplier for the flying animation")] [SerializeField]
        private float speedMultiplier = 0.8f;

        [Tooltip("Rotation duration")] [SerializeField]
        private float rotationDuration = 0.35f;

        public readonly Dictionary<PlanePartType, PlaneSocket> sockets = new Dictionary<PlanePartType, PlaneSocket>();

        private void Awake()
        {
            PlaneSocket[] socketComponents = GetComponentsInChildren<PlaneSocket>();
            foreach (PlaneSocket socket in socketComponents)
            {
                if (!sockets.TryAdd(socket.planePartType, socket))
                {
                    Debug.LogWarning($"Duplicate socket found: {socket.planePartType}");
                }
            }
        }

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildAnimEnded += HandlePlanePartBuildAnimEnded;
        }

        private void HandlePlanePartBuildAnimEnded(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            sockets.TryGetValue(partSo.planePartType, out PlaneSocket socket);

            if (socket == null) return;

            if (partSo.planePartType == socket.planePartType)
            {
                socket.ToggleMeshRenderer(false);
            }
        }

        private void Start()
        {
            LobbyEvents.TriggerPlaneSpawned(this);
        }

        public void PlayTakeoffAnimation(Action onAnimationFinished)
        {
            Sequence seq = DOTween.Sequence();

            //? plane docking 
            seq.Append(transform.DOMove(groundPos, dockAnimDuration));

            SoundEmitter planeEmitter = null;

            //? Play sound precisely when the plane starts moving to the runway
            seq.AppendCallback(() =>
            {
                planeEmitter = SoundManager.Instance.PlaySoundByType(SoundType.PlaneEngine, transform.position,
                    followTarget: transform, isLooping: true);
            });

            //? plane moves to runway
            Tween move = seq.Append(transform.DOMove(takeoffPoint, moveAnimDuration).SetEase(Ease.Linear));

            //? Take of starts on exactly time.
            float takeoffAbsoluteTime = dockAnimDuration + (moveAnimDuration * 0.7f);
            seq.Insert(takeoffAbsoluteTime,
                transform.DORotate(new Vector3(-20f, 0f, 0f), rotationDuration, RotateMode.Fast).SetEase(Ease.OutSine)
            );

            //? While plane is flying, it also moves forward to fly end position.
            seq.Insert(takeoffAbsoluteTime,
                transform.DOMove(flyEndPos, moveAnimDuration * speedMultiplier).SetEase(Ease.InSine));

            seq.OnComplete(() =>
            {
                planeEmitter?.Stop();
                onAnimationFinished?.Invoke();
            });
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildAnimEnded -= HandlePlanePartBuildAnimEnded;
        }
    }
}