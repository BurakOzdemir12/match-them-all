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

        [Tooltip("Gradient to change slider color based on remaining time.")] [SerializeField]
        private Gradient timeGradient;

        [Tooltip("Color to indicate the timer is active.")] [SerializeField]
        private Image sliderFillImage;

        [Tooltip("Icon to show when time is running")] [SerializeField]
        private Image timerIcon;

        [Space(10)] [Header("Freeze timer UI references")] [SerializeField]
        private Transform frozeTimerGroup;

        [Tooltip("Text to display remaining freeze time.")] [SerializeField]
        private TextMeshProUGUI frozeTimerText;

        [Tooltip("Slider that visually represents remaining freeze time.")] [SerializeField]
        private Slider frozeTimerSlider;

        [Tooltip("Color to indicate the time froze.")] [SerializeField]
        private Image frozeSliderFillImage;

        [Tooltip("icon to show when time froze")] [SerializeField]
        private Image frozeTimerIcon;

        [Tooltip("Color to indicate the freeze timer is active.")] [SerializeField]
        private Color frozeTimerColor = Color.deepSkyBlue;

        private void OnEnable()
        {
            ToggleFrozeTimeElements(false);

            TimeManager.OnTimeUpdated += HandleTimeUpdated;
            GameEvents.OnLevelStarted += HandleLevelStarted;
            TimeManager.OnTimeFreezeStarted += HandleTimeFroze;
            TimeManager.OnFreezeTimeUpdated += HandleFrozeTimeUpdated;
            TimeManager.OnTimeFreezeEnded += HandleFrozeEnded;

            if (TimeManager.Instance != null)
            {
                HandleTimeUpdated(Mathf.CeilToInt(TimeManager.Instance.RemainingTime));
            }
        }

        private void HandleFrozeEnded()
        {
            ToggleFrozeTimeElements(false);

            timerSlider.gameObject.SetActive(true);
            timerIcon.gameObject.SetActive(true);
        }

        private void HandleFrozeTimeUpdated(int frozeTime)
        {
            int seconds = frozeTime % 60;

            frozeTimerSlider.value = frozeTime;
            frozeTimerText.text = $"{seconds:0}";
        }

        private void HandleTimeFroze(float duration)
        {
            timerSlider.gameObject.SetActive(false);
            timerIcon.gameObject.SetActive(false);

            ToggleFrozeTimeElements(true);

            frozeTimerText.text = $"{duration:0}";
            frozeSliderFillImage.color = frozeTimerColor;

            frozeTimerSlider.value = duration;
            frozeTimerSlider.maxValue = duration;
        }


        private void HandleLevelStarted(LevelDataSo data)
        {
            timerSlider.maxValue = data.LevelTimeLimit;
            timerSlider.value = data.LevelTimeLimit;

            sliderFillImage.color = timeGradient.Evaluate(1f);
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

        private void ToggleFrozeTimeElements(bool isOpen)
        {
            frozeTimerGroup.gameObject.SetActive(isOpen);
            frozeTimerIcon.gameObject.SetActive(isOpen);
        }

        private void OnDisable()
        {
            TimeManager.OnTimeUpdated -= HandleTimeUpdated;
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            TimeManager.OnTimeFreezeStarted -= HandleTimeFroze;
            TimeManager.OnFreezeTimeUpdated -= HandleFrozeTimeUpdated;
            TimeManager.OnTimeFreezeEnded -= HandleFrozeEnded;
        }
    }
}