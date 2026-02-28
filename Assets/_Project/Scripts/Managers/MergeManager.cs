using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class MergeManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private ItemSpotsManager itemSpotsManager;

        [Header("Animation Settings")] [Tooltip("Offset for Merged objects ")] [SerializeField]
        private Vector3 mergeOffset;

        [Space(10)] [Tooltip("Easing type for the merge animation -> (Smash)")] [SerializeField]
        private Ease smashEaseType;

        [Tooltip("Easing type for merged items goes up animation (Rise (goes) up)")] [SerializeField]
        private Ease riseUpEaseType;

        [Tooltip("Merge items goes up duration")] [SerializeField]
        private float riseUpDuration = 0.2f;

        [Tooltip("Merge items smash duration")] [SerializeField]
        private float smashDuration = 0.2f;

        [Tooltip("How long items wait in the air before smashing")] [SerializeField]
        private float waitBeforeSmashDuration = 0.3f;

        private void OnEnable()
        {
            if (itemSpotsManager != null)
            {
                itemSpotsManager.OnItemsMergeRequested += HandleMergeRequested;
            }
        }

        private void HandleMergeRequested(List<Item> itemsToMerge)
        {
            Sequence mergeSeq = DOTween.Sequence();

            Vector3 middleOnePos = itemsToMerge[1].transform.position;
            Vector3 targetPos = middleOnePos + mergeOffset;

            //? Rise up merged items
            mergeSeq.Append(CreateRiseUpSequence(itemsToMerge, targetPos));

            //? Wait before smashing
            mergeSeq.AppendInterval(waitBeforeSmashDuration);

            //? Smash merged items
            mergeSeq.Append(CreateSmashSequence(itemsToMerge, targetPos));

            mergeSeq.OnComplete(() => CompleteMergeProcess(itemsToMerge));
        }

        private Sequence CreateSmashSequence(List<Item> itemsToMerge, Vector3 targetPos)
        {
            Sequence smashSeq = DOTween.Sequence();
            foreach (var matchedItem in itemsToMerge)
            {
                smashSeq.Join(
                    matchedItem.transform.DOMove(targetPos, smashDuration).SetEase(smashEaseType));
            }

            return smashSeq;
        }

        private Sequence CreateRiseUpSequence(List<Item> itemsToMerge, Vector3 targetPos)
        {
            Sequence upSeq = DOTween.Sequence();
            foreach (var matchedItem in itemsToMerge)
            {
                matchedItem.transform.DOKill();

                Vector3 riseTargetPos = new Vector3(matchedItem.transform.position.x, targetPos.y, targetPos.z);

                upSeq.Join(
                    matchedItem.transform.DOMove(riseTargetPos, riseUpDuration).SetEase(riseUpEaseType));
            }

            return upSeq;
        }

        private void CompleteMergeProcess(List<Item> itemsToMerge)
        {
            Vector3 mergePosition = itemsToMerge[1].transform.position;
            ItemType mergedType = itemsToMerge[1].itemType;

            foreach (var matchedItem in itemsToMerge)
            {
                Destroy(matchedItem.gameObject);
            }

            GameEvents.OnMergeCompleted?.Invoke(mergePosition, mergedType);
        }

        private void OnDisable()
        {
            if (itemSpotsManager != null)
            {
                itemSpotsManager.OnItemsMergeRequested -= HandleMergeRequested;
            }
        }
    }
}