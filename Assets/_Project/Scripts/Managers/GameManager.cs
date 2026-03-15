using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameState currentGameState { get; private set; }


        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);

            Instance = this;

            currentGameState = GameState.Playing;
        }

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnGamePaused += HandleGamePaused;
            GameEvents.OnLevelCompleted += HandleLevelCompleted;
            GameEvents.OnLevelFailed += HandleGameFailed;
            GameEvents.OnReviveRequested += HandleReviveRequested;
            GameOverPanelUI.OnTryAgainClicked += HandleTryAgainClicked;
        }

        private void Start()
        {
            GameEvents.TriggerGameStarted();
        }

        private void HandleTryAgainClicked()
        {
            GameEvents.TriggerGameStarted();
        }

        private void HandleReviveRequested(FailType failType)
        {
            int reviveCost = 100;
            if (EconomyManager.Instance.TrySpendResource(ResourceType.Coin, reviveCost))
            {
                GameEvents.TriggerGameRevived(failType);

                currentGameState = GameState.Playing;
                Time.timeScale = 1f;
            }
            else
            {
                Debug.Log("Couldn't revive request");
            }
        }

        private void HandleGameFailed(FailType failType)
        {
            currentGameState = GameState.LevelFailed;
            Time.timeScale = 0f;
        }

        private void HandleLevelCompleted()
        {
            currentGameState = GameState.LevelCompleted;
            Time.timeScale = 1f;
        }

        private void HandleGamePaused()
        {
            currentGameState = GameState.Paused;
            Time.timeScale = 0f;
        }

        private void HandleGameStarted()
        {
            currentGameState = GameState.Playing;
            Time.timeScale = 1f;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnGamePaused -= HandleGamePaused;
            GameEvents.OnLevelCompleted -= HandleLevelCompleted;
            GameEvents.OnLevelFailed -= HandleGameFailed;
            GameEvents.OnReviveRequested -= HandleReviveRequested;
            GameOverPanelUI.OnTryAgainClicked -= HandleTryAgainClicked;
        }
    }
}