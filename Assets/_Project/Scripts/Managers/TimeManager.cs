using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class TimeManager : MonoBehaviour
    {
        private float _remainingTime;
        private bool _isTimerRunning = false;

        private int lastBroadcastedTime = -1;

        //Events
        public static event Action<int> OnTimeUpdated;

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GameEvents.OnLevelCompleted += HandleLevelCompleted;
            GameEvents.OnLevelFailed += HandleLevelFailed;
            GameEvents.OnGamePaused += HandleGamePaused;
        }

        private void HandleGamePaused()
        {
            StopTimer();
        }

        private void HandleLevelFailed(FailType failType)
        {
            StopTimer();
        }

        private void HandleLevelCompleted()
        {
            StopTimer();
        }

        private void HandleLevelStarted(LevelDataSo levelData)
        {
            StartTimer(levelData.LevelTimeLimit);
        }

        private void StartTimer(float timeLimit)
        {
            _remainingTime = timeLimit;
            _isTimerRunning = true;
            OnTimeUpdated?.Invoke(lastBroadcastedTime);
        }

        private void StopTimer()
        {
            _isTimerRunning = false;
        }

        private void Update()
        {
            if (_isTimerRunning)
            {
                _remainingTime -= Time.deltaTime;

                if (_remainingTime <= 0f)
                {
                    _remainingTime = 0f;
                    _isTimerRunning = false;
                    GameEvents.TriggerLevelFailed(FailType.TimeIsUp);
                }

                OnTimeUpdated?.Invoke(lastBroadcastedTime);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            GameEvents.OnLevelCompleted -= HandleLevelCompleted;
            GameEvents.OnLevelFailed -= HandleLevelFailed;
            GameEvents.OnGamePaused -= HandleGamePaused;
        }
    }
}