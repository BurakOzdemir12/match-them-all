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


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        public void PlayHammerBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetRandomIdenticalItemsFromPool(3);
            if (targets == null || targets.Count < 3) return;

            Sequence seq = DOTween.Sequence().SetLink(this.gameObject);

            GameEvents.TriggerBoosterAnimationStarted(ResourceType.HammerBooster);

            GameObject hammer = Instantiate(hammerPrefab, pos,
                Quaternion.identity);
            hammer.transform.localScale = Vector3.one * hammerScale;


            foreach (var target in targets)
            {
                Vector3 targetPos = target.transform.position;
                Vector3 directionToTarget = (targetPos - hammer.transform.position).normalized;
                directionToTarget.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 targetEuler = lookRotation.eulerAngles;

                targetEuler.y = -90f;

                //TODO Sound effect for move 
                
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
                //TODO Sound effect for hammering

                //? whenever hit animation complete play sound or decrease goal or something else
                seq.AppendCallback(() =>
                {
                    GameEvents.TriggerBoosterUseRequested(ResourceType.HammerBooster, target);
                });

                //? This is the hit Target object explosion effect
                seq.Append(target.transform.DOPunchScale(Vector3.one * targetPunchScale, 0.2f)
                    .SetEase(Ease.OutElastic));
                //TODO target Explosion sound effect
                //? Just before the returning back to the spawn pos move a little bit higher
                seq.Append(hammer.transform.DOMoveY(targetPos.y + 1f, 0f));
            }

            seq.OnComplete(() =>
            {
                //? Animation ends then publish event icon must listen for fade in fade out transactions.
                GameEvents.TriggerBoosterAnimationEnded(ResourceType.HammerBooster);

                //? Target items will destroy
                ItemSpotsManager.Instance.DestroyItemsFromBoard(targets);

                //? Hammer Return back to the first spawned position.
                Sequence finishSeq = DOTween.Sequence().SetLink(this.gameObject);

                finishSeq.Join(hammer.transform.DOMove(pos, hammerMoveDuration)
                    .SetEase(Ease.OutQuad));

                finishSeq.OnComplete(() => { Destroy(hammer); });
            });
        }
    }
}