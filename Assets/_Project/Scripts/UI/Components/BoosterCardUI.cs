using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class BoosterCardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI boosterAmountText;
        [SerializeField] private ResourceType myBoosterType;
        [SerializeField] private Image boosterIcon;

        public static event Action<ResourceType, int, Vector3> OnBoosterUseRequested;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (boosterIcon == null) GetComponent<Image>();
        }

        private void OnEnable()
        {
            EconomyManager.OnResourceAmountChanged += HandleResourceAmountChanged;
            GameEvents.OnBoosterAnimationStarted += HandleBoosterAnimationStarted;
            GameEvents.OnBoosterAnimationEnded += HandleBoosterAnimationEnded;
        }

        private void Start()
        {
            //! This is for test don't forget to delete it.
            EconomyManager.Instance.SetResource(myBoosterType, 3);

            int currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);
            UpdateBoosterAmount(currentAmount);
        }

        private void HandleBoosterAnimationEnded(ResourceType type)
        {
            if (type != myBoosterType) return;
            Fade(1, 0.3f);
        }

        private void HandleBoosterAnimationStarted(ResourceType type)
        {
            if (type != myBoosterType) return;
            Fade(0, 0.3f);
        }

        private void Fade(float value, float duration)
        {
            boosterIcon.DOFade(value, duration);
            boosterAmountText.DOFade(value, duration);
        }

        private void HandleResourceAmountChanged(ResourceType type, int amount)
        {
            if (type == myBoosterType)
            {
                UpdateBoosterAmount(amount);
            }
        }

        private void UpdateBoosterAmount(int amount)
        {
            boosterAmountText.text = amount.ToString();
        }

        public void OnBoosterClicked()
        {
            Vector3 screenPos = this.transform.position;
            screenPos.z = Mathf.Abs(_mainCamera.transform.position.y);
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            OnBoosterUseRequested?.Invoke(myBoosterType, 1, worldPos);
        }

        private void OnDisable()
        {
            EconomyManager.OnResourceAmountChanged -= HandleResourceAmountChanged;
        }
    }
}