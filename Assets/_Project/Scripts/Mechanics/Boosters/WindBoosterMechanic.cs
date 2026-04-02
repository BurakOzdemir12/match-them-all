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

        [Space(10)] [Tooltip("Heli move Y position")] [SerializeField]
        private float heliMoveYPos;

        [Tooltip("Heli yoyo Animation duration")] [SerializeField]
        private float heliYoyoDuration;

        [Tooltip("Loops Count")] [SerializeField]
        private int loopsCount = 4;

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
            GameEvents.TriggerBoosterAnimationStarted(ResourceType.WindBooster);

            GameObject heli = Instantiate(heliPrefab, heliSpawnPos.transform.position, Quaternion.identity);
            heli.transform.localScale = Vector3.one * heliScale;

            Sequence seq = DOTween.Sequence().SetLink(heli.gameObject);

            //? Helicopter moves to specified position
            SoundEmitter heliSoundEmitter = null;
            seq.Append(heli.transform.DOMove(heliHoldPos.transform.position, heliAnimDuration)
                .SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    heliSoundEmitter = SoundManager.Instance.PlaySoundByType(SoundType.HelicopterEngine,
                        _mainCamera.transform.position);
                })
            );
            //? X rotation sets 30f while moving.
            seq.Join(heli.transform.DORotate(new Vector3(30f, 90f, 0f), heliRotateDuration, RotateMode.Fast)
                .SetEase(Ease.OutSine)
            );

            //? Just before the movement to hold pos, it rotates back to 0f X rotation.
            float pitchBackStartTime = heliAnimDuration - heliRotateDuration;
            seq.Insert(pitchBackStartTime,
                heli.transform.DORotate(new Vector3(0f, 90f, 0f), heliRotateDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine));

            //? it's lilke yoyo animation up-down up-down
            //! Remember the formula heliYoyoDuration * loopsCount gives total animation duration
            seq.Append(heli.transform.DOMoveY(heliMoveYPos, heliYoyoDuration)
                .SetRelative()
                .SetLoops(loopsCount, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
            ).OnStart(() =>
            {
                //? This is the main effects for items, wind, objects flying aroun
                SoundManager.Instance.PlaySoundByType(SoundType.WindSound, _mainCamera.transform.position);
                List<Item> targets = ItemSpotsManager.Instance.GetAllItemsOnTheBoard();
                if (targets == null || targets.Count == 0) return;
            });

            //? Heli moves out of the screen with rotation.
            seq.Append(heli.transform.DOMove(heliEndPos.transform.position, heliAnimDuration)
                .SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    heli.transform.DORotate(new Vector3(30f, 90f, 0f), heliRotateDuration,
                        RotateMode.FastBeyond360);
                })
            );

            seq.OnComplete(() =>
            {
                heliSoundEmitter?.Stop();
                GameEvents.TriggerBoosterAnimationEnded(ResourceType.WindBooster);
                Destroy(heli.gameObject);
            });
        }
    }
}