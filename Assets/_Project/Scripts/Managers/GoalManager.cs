using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class GoalManager : MonoBehaviour
    {
        private Dictionary<ItemType, int> activeGoals = new Dictionary<ItemType, int>();
        private List<ItemType> _goalTypes = new List<ItemType>();
        private int _remainingGoalTypes = 0;

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            MergeManager.OnMergeCompleted += HandleMergeCompleted;
            ItemSpotsManager.ItemCollected += HandleItemCollected;
        }

        private void HandleItemCollected(ItemType itemType)
        {
            if (activeGoals.ContainsKey(itemType))
            {
                activeGoals[itemType]--;

                if (activeGoals[itemType] <= 0)
                {
                    activeGoals.Remove(itemType);
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
                    _goalTypes.Add(itemData.ItemType);
                    _remainingGoalTypes++;
                }
            }

            Debug.Log($"Targets Set up different type of item value =>  {_remainingGoalTypes} " +
                      $"You must collect those item types {_goalTypes}.");
        }

        private void HandleMergeCompleted(Vector3 position, ItemType itemType)
        {
            if (GameManager.Instance.currentGameState != GameState.Playing) return;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            MergeManager.OnMergeCompleted -= HandleMergeCompleted;
            ItemSpotsManager.ItemCollected -= HandleItemCollected;
        }
    }
}