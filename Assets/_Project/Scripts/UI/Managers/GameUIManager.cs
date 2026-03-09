using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.UI.Components;
using UnityEngine;

namespace _Project.Scripts.UI.Managers
{
    public class GameUIManager : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private GameObject mainMenu;
        [Tooltip("Main in-game UI, shown during gameplay")] 
        [SerializeField] private GameObject gamePlayPanel;

        [Tooltip("Time is up panel, shown when player runs out of time")] 
        [SerializeField] private GameObject timeIsUpPanel;

        [Tooltip("No Space left panel, shown when player runs out of spot space")] 
        [SerializeField] private GameObject noSpaceLeftPanel;

        [Tooltip("Main game over panel")] 
        [SerializeField] private GameObject gameOverPanel;

        [Tooltip("Main level completed panel")] 
        [SerializeField] private GameObject levelCompletedPanel;

        [Tooltip("paused panel In Game")] 
        [SerializeField] private GameObject pauseGamePanel;

        [Header("Warning Carousel Controller")] 
        [SerializeField] private SecondChancePanelUI secondChancePanelUI;

        private void OnEnable()
        {
            GameEvents.OnGameStarted += HandleGameStarted;
            GameEvents.OnGamePaused += HandleGamePaused;
            GameEvents.OnLevelCompleted += HandleLevelCompleted;
            GameEvents.OnLevelFailed += HandleGameFailed;
            
            secondChancePanelUI.OnFullyDismissed += FinalizeGameOver;
        }

        private void FinalizeGameOver(FailType failType)
        {
            if (failType == FailType.SpotFull) noSpaceLeftPanel.SetActive(false);
            if (failType == FailType.TimeIsUp) timeIsUpPanel.SetActive(false);
            
            gameOverPanel.SetActive(true);
        }

        private void HandleGameFailed(FailType failType)
        {
            gamePlayPanel.SetActive(false);
            
            switch (failType)
            {
                case FailType.SpotFull:
                    noSpaceLeftPanel.SetActive(true);
                    secondChancePanelUI.Setup(FailType.SpotFull);
                    break;
                case FailType.TimeIsUp:
                    timeIsUpPanel.SetActive(true);
                    secondChancePanelUI.Setup(FailType.TimeIsUp);
                    break;
            }
        }

        private void HandleLevelCompleted()
        {
            levelCompletedPanel.SetActive(true);
        }

        private void HandleGamePaused()
        {
            pauseGamePanel.SetActive(true);
        }

        private void HandleGameStarted()
        {
            mainMenu.SetActive(false);
            gamePlayPanel.SetActive(true);
            
            timeIsUpPanel.SetActive(false);
            noSpaceLeftPanel.SetActive(false); 
            gameOverPanel.SetActive(false);
            levelCompletedPanel.SetActive(false);
            pauseGamePanel.SetActive(false);
        }

        private void OnDisable()
        {
            GameEvents.OnGameStarted -= HandleGameStarted;
            GameEvents.OnGamePaused -= HandleGamePaused;
            GameEvents.OnLevelCompleted -= HandleLevelCompleted;
            GameEvents.OnLevelFailed -= HandleGameFailed;
            
            secondChancePanelUI.OnFullyDismissed -= FinalizeGameOver;
        }
    }
}