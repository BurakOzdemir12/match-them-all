using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Mechanics.Boosters
{
    public class HammerBoosterMechanic : MonoBehaviour
    {
        public static HammerBoosterMechanic Instance { get; private set; }

        [Header("Hammer Settings")] [SerializeField]
        private GameObject hammerPrefab;

        [SerializeField] private float hammerScale = 2f;

        [Header("Hammer Animation Settings")] [Tooltip("Hammer one Hit animation Duration")] [SerializeField]
        private float hammerMoveDuration = 0.4f;

        [Tooltip("Hammer Move point vector offset")] [SerializeField]
        private Vector3 hammerMovePosOffset;

        [Tooltip("Hammer rotation duration to target")] [SerializeField]
        private float hammerRotateDuration = 0.4f;

        [Tooltip("Hammer Hit Rotation vector value")] [SerializeField]
        private Vector3 hammerHitRotationVector = new Vector3(0, 0, -90);

        [Tooltip("Target punch scale on hit")] [SerializeField]
        private float targetPunchScale = 0.5f;

        [Tooltip("Hammer Hit duration")] [SerializeField]
        private float hammerHitDuration = 0.5f;

        private Camera _mainCamera;

        private bool _isWorking = false;
        private GameObject _activeHammer;
        private Sequence _activeSequence;

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

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GameEvents.OnGameRevived += HandleGameRevived;
        }

        private void HandleLevelStarted(LevelDataSo data) => AbortAndRefund();
        private void HandleGameRevived(FailType type) => AbortAndRefund();

        public void PlayHammerBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetRandomIdenticalItemsFromPool(3);
            if (targets == null || targets.Count < 3) return;

            _isWorking = true;
            GameEvents.TriggerBoosterAnimationStarted(ResourceType.HammerBooster);

            _activeHammer = Instantiate(hammerPrefab, pos,
                Quaternion.identity);

            _activeHammer.transform.localScale = Vector3.one * hammerScale;

             _activeSequence = DOTween.Sequence().SetLink(_activeHammer.gameObject);

            foreach (var target in targets)
            {
                Vector3 targetPos = target.transform.position;
                Vector3 directionToTarget = (targetPos - _activeHammer.transform.position).normalized;
                directionToTarget.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 targetEuler = lookRotation.eulerAngles;

                targetEuler.y = -90f;

                _activeSequence.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.HammerMove, _mainCamera.transform.position);
                });

                //? Hammer moves to the target pos with specified offset
                _activeSequence.Append(_activeHammer.transform.DOMove(targetPos + hammerMovePosOffset, hammerMoveDuration)
                    .SetEase(Ease.OutQuad));

                //? Hammer rotates to look at the target -> with euler.y = -90, because flat face must look to the object
                _activeSequence.Join(_activeHammer.transform.DORotate(targetEuler, hammerRotateDuration, RotateMode.Fast)
                    .SetEase(Ease.OutQuad));

                //? This is basically hammering action -> set Vector z = -90 its perfectly plays
                _activeSequence.Append(_activeHammer.transform
                    .DORotate(hammerHitRotationVector, hammerHitDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InBack));

                //? whenever hit animation complete play sound or decrease goal or something else
                _activeSequence.AppendCallback(() =>
                {
                    GameEvents.TriggerBoosterUseRequested(ResourceType.HammerBooster, target);

                    EffectManager.Instance.PlayEffect(EffectType.HammerHitItem, target.transform.position,
                        customScale: 0.35f);

                    SoundManager.Instance.PlaySoundByType(SoundType.HammerHit, _mainCamera.transform.position);
                    _activeSequence.AppendInterval(0.3f);
                });

                //? This is the hit Target object explosion effect
                _activeSequence.Append(target.transform.DOPunchScale(Vector3.one * targetPunchScale, 0.2f)
                    .SetEase(Ease.OutElastic));

                _activeSequence.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.ItemExplode, _mainCamera.transform.position);

                    ItemSpotsManager.Instance.DestroySingleItemFromBoard(target);
                });

                //? Just before the returning back to the spawn pos move a little bit higher
                _activeSequence.Append(_activeHammer.transform.DOMoveY(targetPos.y + 1f, 0f));
            }

            _activeSequence.OnComplete(() =>
            {
                //? Animation ends then publish event icon must listen for fade in fade out transactions.
                GameEvents.TriggerBoosterAnimationEnded(ResourceType.HammerBooster);

                //? Hammer Return back to the first spawned position.
                Sequence finishSeq = DOTween.Sequence().SetLink(this.gameObject);

                finishSeq.Join(_activeHammer.transform.DOMove(pos, hammerMoveDuration)
                    .SetEase(Ease.OutQuad));

                finishSeq.OnComplete(() =>
                {
                    _isWorking = false;
                    Destroy(_activeHammer);
                    _activeHammer = null;
                });
            });
        }

        //? If player dies or level completed while the animation is still playing,
        //?it must be stopped and booster amount must be returned to the player.
        private void AbortAndRefund()
        {
            if (_isWorking) 
            {
                _isWorking = false; 

                Debug.Log("Hammer Booster didnt completed amount will return back!");

                EconomyManager.Instance.AddResource(ResourceType.HammerBooster, 1);

                if (_activeSequence != null) _activeSequence.Kill();
                if (_activeHammer != null) Destroy(_activeHammer);

                GameEvents.TriggerBoosterAnimationEnded(ResourceType.HammerBooster);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            GameEvents.OnGameRevived -= HandleGameRevived;
        }
    }
}