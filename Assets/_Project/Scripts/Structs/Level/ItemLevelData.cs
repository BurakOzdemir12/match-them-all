using _Project.Scripts.Enums;

namespace _Project.Scripts.Structs.Level
{
    [System.Serializable]
    public struct ItemLevelData
    {
        public ItemType ItemType;
        public int Amount;
        public bool IsGoal;

        public ItemLevelData(ItemType ıtemType, int amount, bool ısGoal)
        {
            ItemType = ıtemType;
            Amount = amount;
            IsGoal = ısGoal;
        }
    }
}