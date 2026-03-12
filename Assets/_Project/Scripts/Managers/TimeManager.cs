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

        //? If level failed due to time is up, player can revive with a time bonus. This variable defines how much time will be added as bonus.
        [SerializeField] private float timeRenewalBonus = 60f;

        private float _remainingTime;
        public float RemainingTime => _remainingTime;

        private bool _isTimerRunning = false;

        private int lastBroadcastedTime = -1;


        //Events
        public static event Action<int> OnTimeUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GameEvents.OnLevelCompleted += HandleLevelCompleted;
            GameEvents.OnLevelFailed += HandleLevelFailed;
            GameEvents.OnGamePaused += HandleGamePaused;
            GameEvents.OnGameRevived += HandleGameRevived;
        }

        private void HandleGameRevived(FailType type)
        {
            if (type != FailType.TimeIsUp) return;

            AddTimeBonus();
        }

        private void AddTimeBonus()
        {
            _remainingTime += timeRenewalBonus;
            StartTimer(_remainingTime);
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
            lastBroadcastedTime = -1;
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
            GameEvents.OnGameRevived -= HandleGameRevived;
        }
    }
}