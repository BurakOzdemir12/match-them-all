using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class CoinShopUIManager : MonoBehaviour
    {
        [Header("Coin shop")] [Tooltip("Coin Shop panel")] [SerializeField]
        private CanvasGroup coinShopPanel;

        [Tooltip("Coin Shop panel canvas group")] [SerializeField]
        private Canvas coinShopPanelCanvas;

        [Header("Dependencies")] [Tooltip("UI overlay manager")] [SerializeField]
        private OverlayUIManager overlayUIManager;

        [Tooltip("Scrollbar to check if there is more content to show in the shop")] [SerializeField]
        private Scrollbar coinShopScrollbar;

        [Tooltip("There is more message for given info to user")] [SerializeField]
        private Transform thereIsMoreMessage;

        private void OnEnable()
        {
            LobbyEvents.OnResourceNotEnough += HandleResourceNotEnough;
        }

        private void HandleResourceNotEnough(ResourceType type)
        {
            if (type == ResourceType.Coin)
            {
                OnOpenShopPanelClicked();
            }
        }

        private void Start()
        {
            Fade(coinShopPanel, 0f, 0f, setUpdate: true, false);

            overlayUIManager.HidePanel(coinShopPanelCanvas);

            if (coinShopScrollbar.value < coinShopScrollbar.size)
            {
                InfoTextToggle(true);
            }
        }

        private void InfoTextToggle(bool isOpen)
        {
            thereIsMoreMessage.gameObject.SetActive(isOpen);
        }

        public void OnOpenShopPanelClicked()
        {
            Fade(coinShopPanel, 1f, 0.15f, setUpdate: true, true);
            overlayUIManager.ShowPanel(coinShopPanelCanvas);
        }

        public void OnCloseShopPanelClicked()
        {
            Fade(coinShopPanel, 0f, 0.15f, setUpdate: true, false);
            overlayUIManager.HidePanel(coinShopPanelCanvas);
        }

        private void Fade(CanvasGroup group, float value, float duration, bool setUpdate, bool boolValue)
        {
            group.DOFade(value, duration).SetUpdate(setUpdate);
            group.interactable = boolValue;
            group.blocksRaycasts = boolValue;
        }

        private void OnDisable()
        {
            LobbyEvents.OnResourceNotEnough -= HandleResourceNotEnough;
        }
    }
}