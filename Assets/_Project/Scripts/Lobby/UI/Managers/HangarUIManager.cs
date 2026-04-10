using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class HangarUIManager : MonoBehaviour
    {
        [Header("UI References")] [Tooltip("Play next level of game button")] [SerializeField]
        private Button playLevelButon;

        [Tooltip("Opens Build list panel")] [SerializeField]
        private Button openBuildPanelButon;

        [Tooltip("Build List Panel")] [SerializeField]
        private CanvasGroup buildListPanel;

        [Tooltip("Close Build panel button")] [SerializeField]
        private Button closeBuildPanelButton;

        [Tooltip("Build List Canvas component")] [SerializeField]
        private Canvas buildListCanvas;

        [Header("Dependencies")] [Tooltip("UI overlay manager")] [SerializeField]
        private OverlayUIManager overlayUIManager;

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
    }
}