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
        public static event Action<BoosterType> OnBoosterClicked;

        public void TryAgainButtonClick()
        {
            OnTryAgainClicked?.Invoke();
        }

        public void LobbyButtonClick()
        {
            OnLobbyClicked?.Invoke();
        }

        //TODO-------------------
        //! Doesnt show up on inspector !!!!! solve this later
        public void BoosterButtonClick(BoosterType boosterType)
        {
            OnBoosterClicked?.Invoke(boosterType);
        }
    }
}