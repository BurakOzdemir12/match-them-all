using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class ItemSpotsManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private InputManager inputManager;

        [SerializeField] private ItemSpot[] availableSpots;
        [SerializeField] private Transform itemSpotsParent;

        [Header("Item Pos Settings")] [SerializeField]
        private Vector3 itemOffsetOnSpot;

        [SerializeField] private Vector3 itemScaleOnSpot;

        [Header("ItemSettings")] [SerializeField]
        private bool isBusy = false;

        [Header("Data")]
        private Dictionary<ItemType, ItemMergeData> itemMergeDataDictionary = new Dictionary<ItemType, ItemMergeData>();

        [SerializeField] private List<Item> itemsInBar = new List<Item>();

        private void Awake()
        {
            InitSpots();
            if (inputManager == null) inputManager = GetComponent<InputManager>();
        }

        private void OnEnable()
        {
            inputManager.OnItemClicked += HandleItemClicked;
        }

        private void InitSpots()
        {
            if (availableSpots == null || availableSpots.Length == 0)
                availableSpots = GetComponentsInChildren<ItemSpot>();
        }

        private void HandleItemClicked(IInteractable interactable)
        {
            if (isBusy)
            {
                Debug.Log("Item Spots Manager is busy");
                return;
            }

            if (interactable is not Item itemComponent) return;

            //? avaliable spot check
            if (itemsInBar.Count >= availableSpots.Length) return;

            isBusy = true;


            int insertIndex = itemsInBar.Count; // ? if it's new, then insert at the end

            if (itemMergeDataDictionary.ContainsKey(itemComponent.itemType))
            {
                //? If there is the same item type, found that item in the itemsInBar list and insert +1 to the right.
                insertIndex = itemsInBar.FindLastIndex(x => x.itemType == itemComponent.itemType) + 1;
                itemMergeDataDictionary[itemComponent.itemType].items.Add(itemComponent);
            }
            else
            {
                CreateItemMergeData(itemComponent);
            }

            //? If an item is inserted, all the items in the List move to the right
            itemsInBar.Insert(insertIndex, itemComponent);

            interactable.Interact();

            UpdateSpotVisuals();

            if (itemMergeDataDictionary[itemComponent.itemType].items.Count == 3)
            {
                ProcessMerge(itemComponent.itemType);
            }

            CheckForGameOver();

            isBusy = false;
        }

        private void UpdateSpotVisuals()
        {
            for (int i = 0; i < itemsInBar.Count; i++)
            {
                Item currentItem = itemsInBar[i];
                ItemSpot targetSpot = availableSpots[i];

                targetSpot.Populate(currentItem.gameObject);

                currentItem.transform.SetParent(targetSpot.transform);
                currentItem.transform.localPosition = itemOffsetOnSpot;
                currentItem.transform.localScale = itemScaleOnSpot;
                currentItem.transform.rotation = Quaternion.identity;
            }

            //? Clearing the rest of the spots
            for (int i = itemsInBar.Count; i < availableSpots.Length; i++)
            {
                availableSpots[i].Clear();
            }
        }

        private void ProcessMerge(ItemType type)
        {
            var mergeData = itemMergeDataDictionary[type];

            foreach (var matched in mergeData.items)
            {
                itemsInBar.Remove(matched);
                Destroy(matched.gameObject);
            }

            itemMergeDataDictionary.Remove(type);

            UpdateSpotVisuals();
        }

        private void CreateItemMergeData(Item item)
        {
            itemMergeDataDictionary.Add(item.itemType, new ItemMergeData(item));
        }

        private void CheckForGameOver()
        {
            if (itemsInBar.Count >= availableSpots.Length)
            {
                Debug.LogWarning("Game Over!");
            }
        }

        private void OnDisable()
        {
            inputManager.OnItemClicked -= HandleItemClicked;
        }
    }
}