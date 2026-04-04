using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
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
        [SerializeField] private Image boosterAmountIcon;
        [SerializeField] private Button boosterButton;

        public static event Action<ResourceType, int, Vector3> OnBoosterUseRequested;

        private Camera _mainCamera;
        private int currentAmount;

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
            GameEvents.OnLevelFinishing += HandleLevelFinishing;
            GameEvents.OnGameRevived += HandleLevelRevived;
            GameEvents.OnLevelStarted += HandleLevelStarted;

        }

        private void Start()
        {
            //! This is for test don't forget to delete it.
            EconomyManager.Instance.SetResource(myBoosterType, 3);

            currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);
            UpdateBoosterAmount(currentAmount);

            if (boosterButton != null) boosterButton.interactable = true;
        }

        private void HandleLevelStarted(LevelDataSo obj)
        {
            if (boosterButton != null) boosterButton.interactable = true;
            Fade(1f, 0.1f);

            currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);
            UpdateBoosterAmount(currentAmount);
        }

        private void HandleLevelRevived(FailType failType)
        {
            if (boosterButton != null) boosterButton.interactable = true;
            Fade(1f, 0.1f);

            currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);
            UpdateBoosterAmount(currentAmount);
        }

        private void HandleLevelFinishing()
        {
            if (boosterButton != null) boosterButton.interactable = false;
        }

        private void HandleBoosterAnimationEnded(ResourceType type)
        {
            currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);

            UpdateBoosterAmount(currentAmount);

            if (boosterButton != null && currentAmount > 0)
            {
                boosterButton.interactable = true;
            }

            if (type != myBoosterType) return;
            Fade(1, 0.3f);
        }

        private void HandleBoosterAnimationStarted(ResourceType type)
        {
            if (boosterButton != null) boosterButton.interactable = false;

            if (type != myBoosterType) return;
            Fade(0.5f, 0.3f);
        }

        private void Fade(float value, float duration)
        {
            if (boosterIcon != null)
            {
                boosterIcon.DOKill();
                boosterIcon.DOFade(value, duration).SetUpdate(true);
            }

            if (boosterAmountText != null)
            {
                boosterAmountText.DOKill();
                boosterAmountText.DOFade(value, duration).SetUpdate(true);
            }

            if (boosterAmountIcon != null)
            {
                boosterAmountIcon.DOKill();
                boosterAmountIcon.DOFade(value, duration).SetUpdate(true);
            }
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
            if (amount <= 0 && boosterButton != null) boosterButton.interactable = false;
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
            GameEvents.OnBoosterAnimationStarted -= HandleBoosterAnimationStarted;
            GameEvents.OnBoosterAnimationEnded -= HandleBoosterAnimationEnded;
            GameEvents.OnLevelFinishing -= HandleLevelFinishing;
            GameEvents.OnGameRevived -= HandleLevelRevived;
            GameEvents.OnLevelStarted -= HandleLevelStarted;


            this.gameObject.transform.DOKill();
            if (boosterIcon != null) boosterIcon.DOKill();
            if (boosterAmountText != null) boosterAmountText.DOKill();
            if (boosterAmountIcon != null) boosterAmountIcon.DOKill();
        }
    }
}