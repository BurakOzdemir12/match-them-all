using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using _Project.Scripts.Enums;
using _Project.Scripts.Interfaces;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace _Project.Scripts.Managers
{
    public class ItemSpotsManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private ItemSpot[] availableSpots;

        [SerializeField] private Transform itemSpotsParent;
        [SerializeField] private Transform itemsParent;

        [Header("Item Pos Settings")] [SerializeField]
        private Vector3 itemOffsetOnSpot;

        [SerializeField] private Vector3 itemScaleOnSpot;

        [Space(10)] [Header("Item Returned (Base) Settings")] [Tooltip("Item Base scale ")] [SerializeField]
        private Vector3 itemBaseScale = new Vector3(0.6f, 0.6f, 0.6f);

        [Tooltip("Delay between throwing items")] [SerializeField]
        private float delayBetweenItems;

        [Tooltip("Throw(Return to board) duration")] [SerializeField]
        private float returnDuration = 0.5f;

        [Tooltip("How strong the item will jump to return to board")] [SerializeField]
        private float returnJumpPower = 2.5f;

        [Header("ItemSettings")] [SerializeField]
        private bool isBusy = false;

        // private bool isMerging = false;

        [Header("Data")]
        private Dictionary<ItemType, ItemMergeData> itemMergeDataDictionary = new Dictionary<ItemType, ItemMergeData>();

        [SerializeField] private List<Item> itemsInBar = new List<Item>();


        [Header("Animation Settings")] [Tooltip("Animation duration for DOTween animations")] [SerializeField]
        private float animationDuration = 0.15f;

        [Tooltip("How strong the item will jump")] [SerializeField]
        private float jumpPower = 1.5f;

        [Tooltip("How many times the item will jump")] [SerializeField]
        private int numJumps = 1;

        [Tooltip("The scale of the item when it's on the way to the spot (squash & stretch effect)")] [SerializeField]
        private Vector3 itemScaleOnGoing;

        [Tooltip("How strong the item will jump to right or left spot")] [SerializeField]
        private float jumpPowerOnSpot = 0.5f;


        //Event Actions
        public static event Action<List<Item>> OnItemsMergeRequested;
        public static event Action<ItemType> ItemCollected;
        public static event Action<ItemType> OnItemReturnedToBoard;

        private void Awake()
        {
            InitSpots();
        }

        private void OnEnable()
        {
            InputManager.OnItemClicked += HandleItemClicked;
            GameEvents.OnGameRevived += HandleGameRevived;
            GameEvents.OnLevelStarted += HandleLevelStarted;
        }

        private void HandleLevelStarted(LevelDataSo levelData)
        {
            ClearAllSpots();
            isBusy = false;
        }

        private void HandleGameRevived(FailType type)
        {
            if (type == FailType.SpotFull)
            {
                SpillItemsBackToBoard();
            }
        }

        private void SpillItemsBackToBoard()
        {
            isBusy = true;

            for (var i = 0; i < itemsInBar.Count; i++)
            {
                Item item = itemsInBar[i];
                if (item != null && item.gameObject != null)
                {
                    item.transform.DOKill();

                    Vector3 randomDropPos = new Vector3(
                        Random.Range(-1.5f, 1.5f),
                        1f,
                        Random.Range(-1.5f, 3f)
                    );

                    Vector3 randomRotation =
                        new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

                    Sequence itemSeq = DOTween.Sequence().SetLink(item.gameObject);

                    itemSeq.AppendInterval(i * delayBetweenItems);

                    itemSeq.AppendCallback(() =>
                    {
                        if (item != null && item.gameObject != null)
                        {
                            item.transform.SetParent(itemsParent, true);
                        }
                    });

                    itemSeq.Append(item.transform.DOJump(randomDropPos, returnJumpPower, numJumps, returnDuration)
                        .SetEase(Ease.Linear));
                    itemSeq.Join(item.transform.DORotate(randomRotation, returnDuration, RotateMode.FastBeyond360));
                    itemSeq.Join(item.transform.DOScale(itemBaseScale, returnDuration));

                    itemSeq.OnComplete(() =>
                    {
                        if (item != null && item.gameObject != null)
                        {
                            item.ReSpawn();
                            OnItemReturnedToBoard?.Invoke(item.itemType);
                        }
                    });
                }
            }

            int totalItemsSpilled = itemsInBar.Count;

            itemsInBar.Clear();
            itemMergeDataDictionary.Clear();

            foreach (var spot in availableSpots)
            {
                spot.Clear();
            }

            float totalSpillTime = (totalItemsSpilled * delayBetweenItems) + animationDuration;

            DOVirtual.DelayedCall(totalSpillTime, () => { isBusy = false; });
        }

        private void ClearAllSpots()
        {
            foreach (var item in itemsInBar)
            {
                if (item != null && item.gameObject != null)
                {
                    item.transform.DOKill();
                    DOTween.Kill(item.gameObject);

                    Destroy(item.gameObject);
                }
            }

            itemsInBar.Clear();
            itemMergeDataDictionary.Clear();

            foreach (var spot in availableSpots)
            {
                spot.Clear();
            }
        }


        private void InitSpots()
        {
            if (availableSpots == null || availableSpots.Length == 0)
                availableSpots = GetComponentsInChildren<ItemSpot>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void HandleItemClicked(IInteractable interactable)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (isBusy)
            {
                Debug.Log("Item Spots Manager is busy");
                return;
            }

            // if (interactable is not Item itemComponent) return;
            if (interactable is not Item itemComponent || itemComponent == null ||
                itemComponent.gameObject == null) return;

            //? If we already clicked and item in the bar, then ignore it
            if (itemsInBar.Contains(itemComponent)) return;

            //? avaliable spot check
            if (itemsInBar.Count >= availableSpots.Length) return;

            isBusy = true;


            int insertIndex = itemsInBar.Count; // ? if it's new, then insert at the end

            if (itemMergeDataDictionary.TryGetValue(itemComponent.itemType, out var mergeData))
            {
                //? If there is the same item type, found that item in the itemsInBar list and insert +1 to the right.
                insertIndex = itemsInBar.FindLastIndex(x => x.itemType == itemComponent.itemType) + 1;
                mergeData.items.Add(itemComponent);
            }
            else
            {
                CreateItemMergeData(itemComponent);
            }

            //? If an item is inserted, all the items in the List move to the right
            itemsInBar.Insert(insertIndex, itemComponent);

            interactable.Interact();

            UpdateSpotVisuals(itemComponent);

            //? Trigger the event for collecting the item, so that GoalManager can listen and update the goals.
            ItemCollected?.Invoke(itemComponent.itemType);

            // ? If the items are merging, then the game is not over yet, wait for the merge to finish.
            // bool isMergeTriggered = itemMergeDataDictionary[itemComponent.itemType].items.Count == 3;
            // if (!isMergeTriggered)
            // {
            //     float totalFlyTime = (animationDuration * 2) + 1.5f;
            //
            //     DOVirtual.DelayedCall(totalFlyTime, CheckForGameOver);
            // }

            isBusy = false;
        }

        private void UpdateSpotVisuals(Item newlyAddedItem = null)
        {
            for (int i = 0; i < itemsInBar.Count; i++)
            {
                Item currentItem = itemsInBar[i];
                if (currentItem == null || currentItem.gameObject == null) continue;

                ItemSpot targetSpot = availableSpots[i];

                bool isNewToBar = (currentItem == newlyAddedItem);
                bool isChangingSpot = (currentItem.transform.parent != targetSpot.transform);

                if (!isNewToBar && !isChangingSpot)
                {
                    continue;
                }

                targetSpot.Populate(currentItem.gameObject);
                currentItem.transform.SetParent(targetSpot.transform);

                //? DOTween animations ->
                //? Clearing the previous animation
                currentItem.transform.DOKill();
                // Sequence itemSeq = DOTween.Sequence();
                Sequence itemSeq = DOTween.Sequence().SetLink(currentItem.gameObject);

                if (isNewToBar)
                {
                    //? Setting the new animation

                    //? Jumping: move to the target in an arc of 1.5 units, jumping once, in 0.35 seconds
                    itemSeq.Join(
                        currentItem.transform.DOLocalJump(itemOffsetOnSpot, jumpPower, numJumps, animationDuration));

                    //? Translation: Do a 360-degree somersault while flying in the air (RotateMode.FastBeyond360 is important!)
                    itemSeq.Join(currentItem.transform.DOLocalRotate(new Vector3(0, 360, 0), animationDuration,
                        RotateMode.FastBeyond360));

                    itemSeq.Join(currentItem.transform.DOScale(itemScaleOnSpot, animationDuration));

                    //? Bumping: play the bump effects
                    itemSeq.AppendCallback(() => { targetSpot.AnimateBump(); });

                    //? SITTING ON THE SLOT (SQUASH & STRETCH): The moment the object touches the slot (We used Append, it works when the above ones are finished)
                    //? First, let it flatten slightly with the speed coming from above (shrink in Y axis, swell in X and Z).
                    itemSeq.Append(currentItem.transform.DOScale(itemScaleOnGoing, 0.1f));

                    //? RECOVERY: Then it immediately returns to its original size (itemScaleOnSpot) by shaking like jell
                    itemSeq.Append(currentItem.transform.DOScale(itemScaleOnSpot, animationDuration)
                        .SetEase(Ease.OutBack));

                    itemSeq.AppendCallback(CheckForGameOver);
                }
                else
                {
                    //? Bumping: play the bump effects
                    itemSeq.AppendCallback(() => { targetSpot.AnimateBump(); });

                    //? Jumping: move to the target in an arc of 1.5 units, jumping once, in 0.35 seconds
                    itemSeq.Join(
                        currentItem.transform.DOLocalJump(itemOffsetOnSpot, jumpPowerOnSpot, numJumps,
                            animationDuration));
                }

                int indexOfItem = itemMergeDataDictionary[currentItem.itemType].items.IndexOf(currentItem);
                if ((indexOfItem + 1) % 3 == 0)
                {
                    itemSeq.OnComplete(() =>
                    {
                        if (currentItem != null && currentItem.gameObject != null)
                        {
                            PrepareForMerge(currentItem);
                        }
                    });
                }
            }

            //? Clearing the rest of the spots
            for (int i = itemsInBar.Count; i < availableSpots.Length; i++)
            {
                availableSpots[i].Clear();
            }
        }

        private void PrepareForMerge(Item item)
        {
            // isMerging = true;
            if (item == null || item.gameObject == null) return;

            if (!itemMergeDataDictionary.TryGetValue(item.itemType, out var mergeData)) return;

            if (mergeData.items.Count < 3) return;

            if (mergeData.items.Any(i => i == null || i.gameObject == null)) return;

            List<Item> itemsToMerge = mergeData.items.GetRange(0, 3);

            mergeData.items.RemoveRange(0, 3);

            //? If there are no more items in the list with same types, remove the item from the dictionary
            if (mergeData.items.Count == 0)
            {
                itemMergeDataDictionary.Remove(item.itemType);
            }

            foreach (var matched in itemsToMerge)
            {
                itemsInBar.Remove(matched);
            }


            //? objects on the right side move to the left and clear the spaces
            UpdateSpotVisuals();

            OnItemsMergeRequested?.Invoke(itemsToMerge);
        }

        private void CreateItemMergeData(Item item)
        {
            itemMergeDataDictionary.Add(item.itemType, new ItemMergeData(item));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void CheckForGameOver()
        {
            if (itemsInBar.Count < availableSpots.Length) return;

            bool isMergePending = itemMergeDataDictionary.Values.Any(data => data.items.Count >= 3);

            if (!isMergePending)
            {
                GameEvents.TriggerLevelFailed(FailType.SpotFull);
                Debug.Log("Game Over!");
            }
        }

        private void OnDisable()
        {
            InputManager.OnItemClicked -= HandleItemClicked;
            GameEvents.OnGameRevived -= HandleGameRevived;
            GameEvents.OnLevelStarted -= HandleLevelStarted;
        }
    }
}