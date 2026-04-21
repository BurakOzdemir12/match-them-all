using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class HeartRefillUIManager : MonoBehaviour
    {
        [Header("references")] [Tooltip("time left for heart refill")] [SerializeField]
        private TextMeshProUGUI timeCounterText;

        [Tooltip("How many heart left")] [SerializeField]
        private TextMeshProUGUI heartLeftText;

        [Tooltip("Refill offer ")] [SerializeField]
        private Transform offer;

        [Tooltip("Offer price")] [SerializeField]
        private TextMeshProUGUI offerPrice;

        [Header("Panel ui references ")] [Tooltip("Canvas group panel")] [SerializeField]
        private CanvasGroup heartRefillPanel;

        [Tooltip("Heart Refill panel canvas group")] [SerializeField]
        private Canvas heartRefillPanelCanvas;

        [Header("Dependencies")] [Tooltip("UI overlay manager")] [SerializeField]
        private OverlayUIManager overlayUIManager;

        public static event Action<int> OnPayButtonClicked;

        public int currentRefillPrice { get; set; }
        private int _currentHeart;

        private void OnEnable()
        {
            EconomyManager.OnResourceAmountChanged += HandleResourcesChanged;
            HeartRefillManager.OnHeartRefillTimeUpdated += HandleTimeUpdated;
        }

        private void Start()
        {
            Fade(heartRefillPanel, 0f, 0f, setUpdate: true, false);

            overlayUIManager.HidePanel(heartRefillPanelCanvas);

            offerPrice.text = currentRefillPrice.ToString();
        }

        private void HandleTimeUpdated(int time)
        {
            int minutes = time / 60;
            int seconds = time % 60;

            timeCounterText.text = $"{minutes:00}:{seconds:00}";
        }

        private void HandleResourcesChanged(ResourceType type, int value)
        {
            if (type != ResourceType.Health) return;

            _currentHeart = value;
            heartLeftText.text = value.ToString();
        }


        public void OnOpenRefillPanelPressed()
        {
            if (_currentHeart == 5) return;

            Fade(heartRefillPanel, 1f, 0.15f, setUpdate: true, true);
            overlayUIManager.ShowPanel(heartRefillPanelCanvas);
        }

        public void OnCloseRefillPanelPressed()
        {
            Fade(heartRefillPanel, 0f, 0.15f, setUpdate: true, false);
            overlayUIManager.HidePanel(heartRefillPanelCanvas);
        }

        public void OnPayButtonPressed()
        {
            OnPayButtonClicked?.Invoke(currentRefillPrice);

            OnCloseRefillPanelPressed();
        }

        private void Fade(CanvasGroup group, float value, float duration, bool setUpdate, bool boolValue)
        {
            group.DOFade(value, duration).SetUpdate(setUpdate);
            group.interactable = boolValue;
            group.blocksRaycasts = boolValue;
        }

        private void OnDisable()
        {
            EconomyManager.OnResourceAmountChanged -= HandleResourcesChanged;
            HeartRefillManager.OnHeartRefillTimeUpdated -= HandleTimeUpdated;
        }
    }
}