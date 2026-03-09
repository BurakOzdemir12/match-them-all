using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Static
{
    public static class GameEvents
    {
        public static event Action<LevelDataSo> OnLevelStarted;
        public static event Action OnLevelCompleted;
        public static event Action<FailType> OnLevelFailed;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;

        public static event Action<FailType> OnReviveRequested;
        public static event Action<FailType> OnGameRevived;

        public static void TriggerLevelStarted(LevelDataSo levelData)
        {
            OnLevelStarted?.Invoke(levelData);
        }

        public static void TriggerLevelCompleted()
        {
            OnLevelCompleted?.Invoke();
        }

        public static void TriggerLevelFailed(FailType failType)
        {
            OnLevelFailed?.Invoke(failType);
        }

        public static void TriggerGameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public static void TriggerGameRevived(FailType failType)
        {
            OnGameRevived?.Invoke(failType);
        }

        public static void TriggerReviveRequested(FailType failType)
        {
            OnReviveRequested?.Invoke(failType);
        }
    }
}