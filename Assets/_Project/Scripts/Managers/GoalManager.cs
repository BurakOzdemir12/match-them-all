using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class GoalManager : MonoBehaviour
    {
        private Dictionary<ItemType, int> activeGoals = new Dictionary<ItemType, int>();
        private int _remainingGoalTypes = 0;

        //Events
        public static event Action<ItemType, int> OnGoalProgressUpdated;

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            ItemSpotsManager.ItemCollected += HandleItemCollected;
            ItemSpotsManager.OnItemReturnedToBoard += HandleItemReturnedToBoard;
            GameEvents.OnBoosterUsed += HandleBoosterUsed;
        }

        private void HandleBoosterUsed(ResourceType resource, Item item)
        {
            CheckIsItemInGoal(item.itemType);
        }

        private void HandleItemReturnedToBoard(ItemType itemType)
        {
            if (activeGoals.ContainsKey(itemType))
            {
                activeGoals[itemType]++;

                if (activeGoals[itemType] == 1)
                {
                    _remainingGoalTypes++;
                }

                int displayAmount = Mathf.Max(0, activeGoals[itemType]);
                OnGoalProgressUpdated?.Invoke(itemType, displayAmount);
            }
        }

        private void HandleItemCollected(ItemType itemType)
        {
            CheckIsItemInGoal(itemType);
        }

        private void CheckIsItemInGoal(ItemType itemType)
        {
            if (activeGoals.ContainsKey(itemType))
            {
                activeGoals[itemType]--;

                int displayAmount = Mathf.Max(0, activeGoals[itemType]);
                OnGoalProgressUpdated?.Invoke(itemType, displayAmount);

                if (activeGoals[itemType] <= 0)
                {
                    // activeGoals.Remove(itemType);
                    _remainingGoalTypes--;

                    CheckForLevelWin();
                }
            }
        }

        private void CheckForLevelWin()
        {
            if (_remainingGoalTypes <= 0)
            {
                GameEvents.TriggerLevelCompleted();
            }
        }

        private void HandleLevelStarted(LevelDataSo data)
        {
            InitializeGoals(data);
        }

        private void InitializeGoals(LevelDataSo data)
        {
            activeGoals.Clear();
            _remainingGoalTypes = 0;

            foreach (var itemData in data.ItemLevelDataList)
            {
                if (itemData.IsGoal)
                {
                    activeGoals.Add(itemData.ItemType, itemData.Amount);
                    _remainingGoalTypes++;
                }
            }
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            ItemSpotsManager.ItemCollected -= HandleItemCollected;
            ItemSpotsManager.OnItemReturnedToBoard -= HandleItemReturnedToBoard;
            GameEvents.OnBoosterUsed -= HandleBoosterUsed;
        }
    }
}