using System.Collections.Generic;
using _Project.Scripts.Enums;
using UnityEngine;

namespace _Project.Scripts.ItemScripts.ScriptableObjects
{
    [System.Serializable]
    public struct ItemDataEntry
    {
        [field: Header("Item Data"), Tooltip("The type of item this entry represents"), SerializeField]
        public ItemType itemType { get; private set; }

        [field: Tooltip("The prefab of the item"), SerializeField]
        public Item itemPrefab { get; private set; }

        [field: Tooltip("The icon of the item"), SerializeField]
        public Sprite icon { get; private set; }
    }

    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "ScriptableObjects/Game/Item Database")]
    public class ItemDataSo : ScriptableObject
    {
        [SerializeField] private List<ItemDataEntry> itemEntries;

        public ItemDataEntry GetItemData(ItemType itemType)
        {
            foreach (var entry in itemEntries)
            {
                if (entry.itemType == itemType) return entry;
            }

            Debug.LogError($"ItemDatabase: {itemType} not found in database!");
            return default;
        }
    }
}