using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.ItemScripts.ScriptableObjects;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using _Project.Scripts.Structs.Level;
using _Project.Scripts.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Managers
{
    public class GoalUIManager : MonoBehaviour
    {
        [SerializeField] private GoalCardUI goalCardPrefab;
        [SerializeField] private Transform gridContainer;

        [Header("Main Item Database")] [SerializeField]
        private ItemDataSo itemDatabase;

        private Dictionary<ItemType, GoalCardUI> spawnedCards = new Dictionary<ItemType, GoalCardUI>();

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GoalManager.OnGoalProgressUpdated += HandleGoalUpdated;
        }

        private void HandleGoalUpdated(ItemType itemType, int goalAmount)
        {
            if (spawnedCards.TryGetValue(itemType, out GoalCardUI targetCard))
            {
                targetCard.UpdateAmount(goalAmount);
            }
        }

        private void HandleLevelStarted(LevelDataSo data)
        {
            InitializeGoalCards(data);
        }

        private void InitializeGoalCards(LevelDataSo data)
        {
            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }

            spawnedCards.Clear();

            foreach (var levelData in data.ItemLevelDataList)
            {
                if (levelData.IsGoal)
                {
                    GoalCardUI newCard = Instantiate(goalCardPrefab, gridContainer);

                    ItemDataEntry itemDataEntry = itemDatabase.GetItemData(levelData.ItemType);


                    newCard.Setup(levelData.Amount, itemDataEntry.icon);

                    spawnedCards.Add(levelData.ItemType, newCard);
                }
            }
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            GoalManager.OnGoalProgressUpdated -= HandleGoalUpdated;
        }
    }
}