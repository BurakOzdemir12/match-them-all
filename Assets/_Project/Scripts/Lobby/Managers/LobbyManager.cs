using System;
using _Project.Scripts.Lobby.UI.Managers;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        private const string GameSceneName = "GameScene";

        private void OnEnable()
        {
            HangarUIManager.OnPlayLevelButtonClicked += HandlePlayLevelButtonClicked;
        }

        private void HandlePlayLevelButtonClicked()
        {
            GameEvents.TriggerSceneLoadRequested(GameSceneName);
        }

        private void OnDisable()
        {
            HangarUIManager.OnPlayLevelButtonClicked -= HandlePlayLevelButtonClicked;
        }
    }
}