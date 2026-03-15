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
        [SerializeField] private float freezeDuration = 10f;
        private float _remainingTime;
        public float RemainingTime => _remainingTime;

        //? Use for level completed grade calcualtion
        private float totalLevelTime;
        public float TotalLevelTime => totalLevelTime;

        private bool _isTimerRunning = false;

        private int lastBroadcastedTime = -1;
        private int lastBroadcastedFreezeTime = -1;

        private float _remainFrozeTime = 0f;
        private bool _isTimeFroze = false;

        //Events
        public static event Action<int> OnTimeUpdated;

        public static event Action<float> OnTimeFreezeStarted;
        public static event Action<int> OnFreezeTimeUpdated;
        public static event Action OnTimeFreezeEnded;

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


        public void FreezeTime()
        {
            if (!_isTimerRunning) return;

            _isTimeFroze = true;
            _remainFrozeTime = freezeDuration;
            lastBroadcastedFreezeTime = -1;

            OnTimeFreezeStarted?.Invoke(freezeDuration);
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
            totalLevelTime = levelData.LevelTimeLimit;
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
            if (!_isTimerRunning) return;

            if (_isTimeFroze)
            {
                _remainFrozeTime -= Time.deltaTime;
                BroadcastFrozeTimeUpdate();

                if (_remainFrozeTime <= 0f)
                {
                    _isTimeFroze = false;
                    OnTimeFreezeEnded?.Invoke();
                }

                return;
            }

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

        private void BroadcastTimeUpdate()
        {
            int currentSecond = Mathf.CeilToInt(_remainingTime);
            if (currentSecond != lastBroadcastedTime)
            {
                lastBroadcastedTime = currentSecond;
                OnTimeUpdated?.Invoke(lastBroadcastedTime);
            }
        }

        private void BroadcastFrozeTimeUpdate()
        {
            int currentSecond = Mathf.CeilToInt(_remainFrozeTime);
            if (currentSecond != lastBroadcastedFreezeTime)
            {
                lastBroadcastedFreezeTime = currentSecond;
                OnFreezeTimeUpdated?.Invoke(lastBroadcastedFreezeTime);
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