using System;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.UI.Managers;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        private const string GameSceneName = "GameScene";

        [SerializeField] private PlaneLibrarySo planeLibrarySo;

        private void OnEnable()
        {
            HangarUIManager.OnPlayLevelButtonClicked += HandlePlayLevelButtonClicked;
        }

        private void Start()
        {
            LobbyEvents.TriggerPlaneLibraryLoaded(planeLibrarySo);
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