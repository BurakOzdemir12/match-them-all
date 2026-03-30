using System;
using _Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class GameOverPanelUI : MonoBehaviour
    {
        public static event Action OnTryAgainClicked;
        public static event Action OnLobbyClicked;

        public void TryAgainButtonClick()
        {
            OnTryAgainClicked?.Invoke();
        }

        public void LobbyButtonClick()
        {
            OnLobbyClicked?.Invoke();
        }

    }
}