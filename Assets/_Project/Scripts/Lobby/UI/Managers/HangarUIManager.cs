using DG.Tweening;
using UnityEngine;
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


        public void OnPlayLevelClicked()
        {
        }

        public void OnOpenBuildPanelClicked()
        {
            buildListPanel.DOFade(1, 0.2f).SetUpdate(true);
            buildListPanel.interactable = true;
            buildListPanel.blocksRaycasts = true;
        }

        public void OnCloseBuildPanelClicked()
        {
            buildListPanel.DOFade(0, 0.2f).SetUpdate(true);
            buildListPanel.interactable = false;
            buildListPanel.blocksRaycasts = false;
        }
    }
}