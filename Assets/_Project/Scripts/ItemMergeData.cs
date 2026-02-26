using System.Collections.Generic;
using _Project.Scripts.Enums;

namespace _Project.Scripts
{
    public struct ItemMergeData
    {
        public ItemType itemType;
        public List<Item> items;

        public ItemMergeData(Item item)
        {
            this.itemType = item.itemType;
            this.items = new List<Item>() { item };
        }
    }
}