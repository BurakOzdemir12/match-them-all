using System;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class TimeDisplayUI : MonoBehaviour
    {
        [Header("Timer UI Elements")] [SerializeField]
        private TextMeshProUGUI timerText;

        [Tooltip("Slider that visually represents remaining time.")] [SerializeField]
        private Slider timerSlider;

        [SerializeField] private Gradient timeGradient;
        [SerializeField] private Image sliderFillImage;

        [SerializeField] private Image timerIcon;

        private void OnEnable()
        {
            TimeManager.OnTimeUpdated += HandleTimeUpdated;
            GameEvents.OnLevelStarted += HandleLevelStarted;
        }

        private void HandleLevelStarted(LevelDataSo data)
        {
            timerSlider.maxValue = data.LevelTimeLimit;
            timerSlider.value = data.LevelTimeLimit;

            // timerIcon.color = timeGradient.Evaluate(1f);
        }

        private void HandleTimeUpdated(int time)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            timerText.text = $"{minutes:00}:{seconds:00}";

            timerSlider.value = time;

            float normalizedTime = time / timerSlider.maxValue;

            sliderFillImage.color = timeGradient.Evaluate(normalizedTime);
        }

        private void OnDisable()
        {
            TimeManager.OnTimeUpdated -= HandleTimeUpdated;
            GameEvents.OnLevelStarted -= HandleLevelStarted;
        }
    }
}