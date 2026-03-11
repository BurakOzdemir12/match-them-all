using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        private float _remainingTime;
        public float RemainingTime => _remainingTime;
        
        private bool _isTimerRunning = false;

        private int lastBroadcastedTime = -1;

        //Events
        public static event Action<int> OnTimeUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
        }

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

            BroadcastTimeUpdate();
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
                    BroadcastTimeUpdate();
                    GameEvents.TriggerLevelFailed(FailType.TimeIsUp);
                }
                else
                {
                    BroadcastTimeUpdate();
                }
            }
        }

        private void BroadcastTimeUpdate()
        {
            int currentSecond = Mathf.CeilToInt(_remainingTime);
            if (currentSecond != lastBroadcastedTime)
            {
                lastBroadcastedTime = currentSecond;
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