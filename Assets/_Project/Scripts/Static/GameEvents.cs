using System;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using UnityEngine;

namespace _Project.Scripts.Static
{
    public static class GameEvents
    {
        public static event Action<LevelDataSo> OnLevelPrepareStarted;
        public static event Action<LevelDataSo> OnLevelStarted;
        public static event Action OnLevelCompleted;
        public static event Action<FailType> OnLevelFailed;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;

        public static event Action<FailType> OnReviveRequested;
        public static event Action<FailType> OnGameRevived;
        public static event Action OnGameOver;
        public static event Action OnLevelFinishing;

        public static event Action<string> OnSceneLoadRequested;


        #region Booster Events

        public static event Action<ResourceType, Item> OnBoosterUsed;
        public static event Action<ResourceType> OnBoosterAnimationStarted;
        public static event Action<ResourceType> OnBoosterAnimationEnded;

        #endregion

        public static void TriggerLevelPrepare(LevelDataSo levelData)
        {
            OnLevelPrepareStarted?.Invoke(levelData);
        }

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

        public static void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }

        public static void TriggerLevelFinishing()
        {
            OnLevelFinishing?.Invoke();
        }

        public static void TriggerSceneLoadRequested(string sceneName)
        {
            OnSceneLoadRequested?.Invoke(sceneName);
        }

        #region Booster Events

        public static void TriggerBoosterUseRequested(ResourceType type, Item item)
        {
            OnBoosterUsed?.Invoke(type, item);
        }

        public static void TriggerBoosterAnimationStarted(ResourceType type)
        {
            OnBoosterAnimationStarted?.Invoke(type);
        }

        public static void TriggerBoosterAnimationEnded(ResourceType type)
        {
            OnBoosterAnimationEnded?.Invoke(type);
        }

        #endregion
    }
}