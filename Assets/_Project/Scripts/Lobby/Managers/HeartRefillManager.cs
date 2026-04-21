using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.UI.Managers;
using _Project.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class HeartRefillManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hearthReviewTimerText;
        [SerializeField] private float refillTotalWaitTime;
        [SerializeField] private HeartRefillUIManager heartRefillUIManager;
        [SerializeField] private int refillPrice;

        private int _currentHeartAmount;
        private float _timer;
        private bool _isTimerRunning;
        private int lastBroadcastedTime = -1;

        public static event Action<int> OnHeartRefillTimeUpdated;

        private void OnEnable()
        {
            HeartRefillUIManager.OnPayButtonClicked += HandlePayButtonClicked;
            EconomyManager.OnResourceAmountChanged += HandleResourcesChanged;
        }

        private void Start()
        {
            //! Testing 
            heartRefillUIManager.currentRefillPrice = refillPrice;
            //--
            SetCredentials();
        }

        private void Update()
        {
            if (!_isTimerRunning) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                _isTimerRunning = false;
                HandleTimerCompleted();
            }
            else
            {
                BroadCastTimeUpdate();
            }
        }

        private void SetCredentials()
        {
            _currentHeartAmount = EconomyManager.Instance.GetResourceAmount(ResourceType.Health);

            if (_currentHeartAmount < 5)
            {
                if (!_isTimerRunning)
                {
                    StartTimerCounter(Mathf.CeilToInt(refillTotalWaitTime));
                    _timer = refillTotalWaitTime;
                }
            }
            else
            {
                _isTimerRunning = false;
                hearthReviewTimerText.text = "Full";
            }
        }

        private void HandleTimerCompleted()
        {
            EconomyManager.Instance.AddResource(ResourceType.Health, 1);

            SetCredentials();
        }

        private void BroadCastTimeUpdate()
        {
            int currentSecond = Mathf.CeilToInt(_timer);

            if (currentSecond != lastBroadcastedTime)
            {
                lastBroadcastedTime = currentSecond;

                SetTimeText(lastBroadcastedTime);

                OnHeartRefillTimeUpdated?.Invoke(Mathf.CeilToInt(lastBroadcastedTime));
            }
        }

        private void StartTimerCounter(int time)
        {
            SetTimeText(time);

            _isTimerRunning = true;
        }

        private void SetTimeText(int time)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            hearthReviewTimerText.text = $"{minutes:00}:{seconds:00}";
        }

        private void HandleResourcesChanged(ResourceType type, int value)
        {
            if (type != ResourceType.Health) return;

            _currentHeartAmount = value;

            SetCredentials();
        }

        private void HandlePayButtonClicked(int price)
        {
            if (EconomyManager.Instance.TrySpendResource(ResourceType.Coin, price))
            {
                EconomyManager.Instance.AddResource(ResourceType.Health, 1);
            }
            else
            {
                LobbyEvents.TriggerResourceNotEnough(ResourceType.Coin);
            }
        }

        private void OnDisable()
        {
            HeartRefillUIManager.OnPayButtonClicked -= HandlePayButtonClicked;
            EconomyManager.OnResourceAmountChanged -= HandleResourcesChanged;
        }
    }
}