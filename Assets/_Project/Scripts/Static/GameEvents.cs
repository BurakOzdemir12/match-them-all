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
        public static event Action OnLevelFailed;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;

        public static void TriggerLevelStarted(LevelDataSo levelData)
        {
            OnLevelStarted?.Invoke(levelData);
        }

        public static void TriggerLevelCompleted()
        {
            OnLevelCompleted?.Invoke();
        }

        public static void TriggerLevelFailed()
        {
            OnLevelFailed?.Invoke();
        }

        public static void TriggerGameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }
    }
}