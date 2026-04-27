using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Components;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class PlaneSocketManager : MonoBehaviour
    {
        [Header("Animation Settings")] 
        [Tooltip("Takeoff point")] [SerializeField]
        private Vector3 takeoffPoint;

        [Tooltip("plane Fly end position")] [SerializeField]
        private Vector3 flyEndPos;

        public readonly Dictionary<PlanePartType, Transform> sockets = new Dictionary<PlanePartType, Transform>();
        [SerializeField] private float moveAnimDuration;
        [SerializeField] private float flyUpAnimDuration;

        private void Awake()
        {
            PlaneSocket[] socketComponents = GetComponentsInChildren<PlaneSocket>();
            foreach (PlaneSocket socket in socketComponents)
            {
                if (!sockets.ContainsKey(socket.planePartType))
                {
                    sockets.Add(socket.planePartType, socket.transform);
                }
                else
                {
                    Debug.LogWarning($"Duplicate socket found: {socket.planePartType}");
                }
            }
        }

        private void Start()
        {
            LobbyEvents.TriggerPlaneSpawned(this);
        }

        public void PlayTakeoffAnimation(Action onAnimationFinished)
        {
            Sequence seq = DOTween.Sequence();

            //? plane moves to runway
            Tween move = seq.Append(transform.DOMove(takeoffPoint, moveAnimDuration).SetEase(Ease.Linear));

            //? Take of starts on exactly time.
            float takeoffTime = move.Duration() * 0.7f;
            seq.Insert(takeoffTime,
                transform.DOMoveY(transform.position.y + 10f, flyUpAnimDuration)
                    .SetEase(Ease.InOutSine)
                    .OnStart(() =>
                        //? Take ofrotation for airplane noise 
                        transform.DORotate(new Vector3(-20f, 0f, 0f), 0.2f, RotateMode.Fast)
                    ));

            //? While plane is flying, it also moves forward to fly end position.
            seq.Join(transform.DOMove(flyEndPos, moveAnimDuration * 2f).SetEase(Ease.InQuad));

            seq.OnComplete(() => onAnimationFinished?.Invoke());
        }
    }
}