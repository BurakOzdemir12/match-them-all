using System;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class LevelCompletePanelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI elapsedTimeText;
        [SerializeField] private Image endImage;

        private void OnEnable()
        {
            UpdateFinishTimeUI();
        }

        private void UpdateFinishTimeUI()
        {
            if (TimeManager.Instance == null) return;

            int finishTimeSeconds = Mathf.CeilToInt(TimeManager.Instance.RemainingTime);

            int minutes = finishTimeSeconds / 60;
            int seconds = finishTimeSeconds % 60;

            elapsedTimeText.text = $"{minutes:00}:{seconds:00}";
        }

        public void OnContinueButtonClicked()
        {
            // SceneManager.LoadScene("LobbyScene");
        }
    }
}