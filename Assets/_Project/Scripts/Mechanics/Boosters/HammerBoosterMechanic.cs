using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
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

        public void PlayHammerBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetRandomIdenticalItemsFromPool(3);
            if (targets == null || targets.Count < 3) return;


            GameEvents.TriggerBoosterAnimationStarted(ResourceType.HammerBooster);

            GameObject hammer = Instantiate(hammerPrefab, pos,
                Quaternion.identity);
            
            hammer.transform.localScale = Vector3.one * hammerScale;

            Sequence seq = DOTween.Sequence().SetLink(hammer.gameObject);

            foreach (var target in targets)
            {
                Vector3 targetPos = target.transform.position;
                Vector3 directionToTarget = (targetPos - hammer.transform.position).normalized;
                directionToTarget.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 targetEuler = lookRotation.eulerAngles;

                targetEuler.y = -90f;

                seq.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.HammerMove, _mainCamera.transform.position);
                });

                //? Hammer moves to the target pos with specified offset
                seq.Append(hammer.transform.DOMove(targetPos + hammerMovePosOffset, hammerMoveDuration)
                    .SetEase(Ease.OutQuad));

                //? Hammer rotates to look at the target -> with euler.y = -90, because flat face must look to the object
                seq.Join(hammer.transform.DORotate(targetEuler, hammerRotateDuration, RotateMode.Fast)
                    .SetEase(Ease.OutQuad));

                //? This is basically hammering action -> set Vector z = -90 its perfectly plays
                seq.Append(hammer.transform
                    .DORotate(hammerHitRotationVector, hammerHitDuration, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.InBack));

                //? whenever hit animation complete play sound or decrease goal or something else
                seq.AppendCallback(() =>
                {
                    GameEvents.TriggerBoosterUseRequested(ResourceType.HammerBooster, target);

                    EffectManager.Instance.PlayEffect(EffectType.HammerHitItem, target.transform.position,
                        customScale: 0.35f);

                    SoundManager.Instance.PlaySoundByType(SoundType.HammerHit, _mainCamera.transform.position);
                    seq.AppendInterval(0.3f);
                });

                //? This is the hit Target object explosion effect
                seq.Append(target.transform.DOPunchScale(Vector3.one * targetPunchScale, 0.2f)
                    .SetEase(Ease.OutElastic));

                seq.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.ItemExplode, _mainCamera.transform.position);

                    ItemSpotsManager.Instance.DestroySingleItemFromBoard(target);
                });

                //? Just before the returning back to the spawn pos move a little bit higher
                seq.Append(hammer.transform.DOMoveY(targetPos.y + 1f, 0f));
            }

            seq.OnComplete(() =>
            {
                //? Animation ends then publish event icon must listen for fade in fade out transactions.
                GameEvents.TriggerBoosterAnimationEnded(ResourceType.HammerBooster);

                //? Hammer Return back to the first spawned position.
                Sequence finishSeq = DOTween.Sequence().SetLink(this.gameObject);

                finishSeq.Join(hammer.transform.DOMove(pos, hammerMoveDuration)
                    .SetEase(Ease.OutQuad));

                finishSeq.OnComplete(() => { Destroy(hammer); });
            });
        }
    }
}