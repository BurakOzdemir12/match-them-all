using System;
using _Project.Scripts.Components.DoTween;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class HangarUIManager : MonoBehaviour
    {
        [Header("UI Buttons")] [Tooltip("Play next level of game button")] [SerializeField]
        private Button playLevelButon;

        [Tooltip("Opens Build list panel")] [SerializeField]
        private Button openBuildPanelButon;

        [Tooltip("Close Build panel button")] [SerializeField]
        private Button closeBuildPanelButton;

        [Header("UI Panels-Canvas")] [Tooltip("Build List Panel")] [SerializeField]
        private CanvasGroup buildListPanel;

        [Tooltip("Build List Canvas component")] [SerializeField]
        private Canvas buildListCanvas;

        [Header("Dependencies")] [Tooltip("UI overlay manager")] [SerializeField]
        private OverlayUIManager overlayUIManager;

        [Header("Progress Ui references")] [Tooltip("Plane build progress -> Slider")] [SerializeField]
        private Slider progressSlider;

        [Tooltip("Progress Text")] [SerializeField]
        private TextMeshProUGUI progressText;

        [Header("Stats/Resources UI references")] [Tooltip("Coin amount text")] [SerializeField]
        private TextMeshProUGUI coinText;

        [Tooltip("Health amount text")] [SerializeField]
        private TextMeshProUGUI healthText;

        [Tooltip("Wrench amount text")] [SerializeField]
        private TextMeshProUGUI wrenchText;

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted += HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlanePartBuildAnimEnded += HandlePlanePartBuildAnimEnded;
            LobbyEvents.OnPlaneBuildProgressChanged += HandlePlaneBuildProgressChanged;
            EconomyManager.OnResourceAmountChanged += HandleResourceAmountChanged;
        }

        private void HandleResourceAmountChanged(ResourceType resourceType, int newAmount)
        {
            switch (resourceType)
            {
                case ResourceType.Coin:
                    UpdateResourceStats(coinText, newAmount);
                    break;
                case ResourceType.Health:
                    UpdateResourceStats(healthText, newAmount);
                    break;
                case ResourceType.Wrench:
                    UpdateResourceStats(wrenchText, newAmount);
                    break;
            }
        }

        private void HandlePlaneBuildProgressChanged(float progress)
        {
            if (progressSlider != null)
                progressSlider.DOValue(progress, 0.5f).SetEase(Ease.OutQuad);

            if (progressText == null) return;

            int startAmount = 0;
            int.TryParse(progressText.text, out startAmount);
            progressText.DoCounterInt(startAmount, Mathf.RoundToInt(progress), 0.5f, Ease.OutExpo, "{0}%");
        }

        private void HandlePlanePartBuildAnimEnded(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            playLevelButon.gameObject.SetActive(true);
            openBuildPanelButon.gameObject.SetActive(true);
        }

        private void HandlePlanePartBuildAnimStarted(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            Fade(0f, 0f, setUpdate: true, false);
            overlayUIManager.HidePanel(buildListCanvas);
            playLevelButon.gameObject.SetActive(false);
            openBuildPanelButon.gameObject.SetActive(false);
        }

        private void Start()
        {
            Fade(0f, 0f, setUpdate: true, false);
            overlayUIManager.HidePanel(buildListCanvas);
        }

        public void OnPlayLevelClicked()
        {
        }

        public void OnOpenBuildPanelClicked()
        {
            Fade(1f, 0.15f, setUpdate: true, true);
            overlayUIManager.ShowPanel(buildListCanvas);
        }

        public void OnCloseBuildPanelClicked()
        {
            Fade(0f, 0.15f, setUpdate: true, false);
            overlayUIManager.HidePanel(buildListCanvas);
        }

        private void Fade(float value, float duration, bool setUpdate, bool boolValue)
        {
            buildListPanel.DOFade(value, duration).SetUpdate(setUpdate);
            buildListPanel.interactable = boolValue;
            buildListPanel.blocksRaycasts = boolValue;
        }

        private void UpdateResourceStats(TextMeshProUGUI textElement, int value)
        {
            if (textElement == null) return;

            textElement.text = value.ToString();

            int startAmount = 0;
            int.TryParse(textElement.text, out startAmount);

            textElement.DoCounterInt(startAmount, value, 0.5f, Ease.OutExpo);
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted -= HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlanePartBuildAnimEnded -= HandlePlanePartBuildAnimEnded;
            LobbyEvents.OnPlaneBuildProgressChanged -= HandlePlaneBuildProgressChanged;
            EconomyManager.OnResourceAmountChanged -= HandleResourceAmountChanged;
        }
    }
}