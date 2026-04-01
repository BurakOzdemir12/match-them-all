using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Mechanics.Boosters
{
    public class WindBoosterMechanic : MonoBehaviour
    {
        public static WindBoosterMechanic Instance { get; private set; }

        [Header("References")] [Tooltip("The fan prefab that it will spawn")] [SerializeField]
        private GameObject heliPrefab;

        [Header("Spawn and Animation Settings")] [Tooltip("Fan scale in game")] [SerializeField]
        private float heliScale;

        [Tooltip("Heli Spawn Pos")] [SerializeField]
        private GameObject heliSpawnPos;

        [Tooltip("Helicopter stop and hold position")] [SerializeField]
        private GameObject heliHoldPos;

        [Tooltip("Heli End Pos")] [SerializeField]
        private GameObject heliEndPos;

        [Tooltip("Heli move animation duration -> (Speed)")] [SerializeField]
        private float heliAnimDuration;

        [Tooltip("Heli rotate animation duration -> (Rotate Speed)")] [SerializeField]
        private float heliRotateDuration;

        private Camera _mainCamera;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            _mainCamera = Camera.main;
        }

        public void PlayWindBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetAllItemsOnTheBoard();
            if (targets == null || targets.Count == 0) return;

            GameEvents.TriggerBoosterAnimationStarted(ResourceType.WindBooster);

            GameObject heli = Instantiate(heliPrefab, heliSpawnPos.transform.position, Quaternion.identity);
            heli.transform.localScale = Vector3.one * heliScale;

            Sequence seq = DOTween.Sequence().SetLink(heli.gameObject);

            SoundEmitter heliSoundEmitter = null;
            seq.Append(heli.transform.DOMove(heliHoldPos.transform.position, heliAnimDuration).SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    heliSoundEmitter = SoundManager.Instance.PlaySoundByType(SoundType.HelicopterEngine,
                        _mainCamera.transform.position);
                }));
            seq.Join(heli.transform.DORotate(new Vector3(30f, 90f, 0f), heliRotateDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine));

//TODO Create Ease animation -> up down effect
//TODO create  animation for move to end position

            seq.OnComplete(() =>
            {
                heliSoundEmitter?.Stop();
                Destroy(heli.gameObject);
            });
        }
    }
}