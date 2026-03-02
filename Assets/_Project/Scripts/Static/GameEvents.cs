using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Static
{
    public static class GameEvents
    {
        public static event Action<Vector3, ItemType> OnMergeCompleted;
        public static event Action<LevelDataSo> OnLevelStarted;
        public static event Action OnLevelCompleted;

        public static void TriggerMergeCompleted(Vector3 position, ItemType itemType)
        {
            OnMergeCompleted?.Invoke(position, itemType);
        }

        public static void TriggerLevelStarted(LevelDataSo levelData)
        {
            OnLevelStarted?.Invoke(levelData);
        }

        public static void TriggerLevelCompleted()
        {
            OnLevelCompleted?.Invoke();
        }
    }
}