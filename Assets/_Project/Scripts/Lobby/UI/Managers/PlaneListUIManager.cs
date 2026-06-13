using System.Collections.Generic;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class PlaneListUIManager : MonoBehaviour
    {
        [Header("Main Data")] [SerializeField] private PlaneLibrarySo planeLibrary;

        [Header("Panel references")] [SerializeField]
        private CanvasGroup planeListPanelCanvasGroup;

        [Header("UI Elements")] [SerializeField]
        private PlaneListCardUI planeListCardPrefab;

        [SerializeField] private Transform planeListCardContainer;

        private readonly Dictionary<PlaneBluePrintSo, PlaneListCardUI> _spawnedPlaneCards =
            new Dictionary<PlaneBluePrintSo, PlaneListCardUI>();


        private void OnEnable()
        {
            LobbyEvents.OnPlaneBuildCompleted += HandlePlaneBuildCompleted;
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
            LobbyEvents.OnPlaneLibraryLoaded += HandlePlaneLibraryLoaded;
        }

        private void HandlePlaneLibraryLoaded(PlaneLibrarySo library)
        {
            planeLibrary = library;
            InitializePlaneCards(planeLibrary);
        }

        private void HandlePlaneSpawned(PlaneSocketManager socketManager)
        {
            InitializePlaneCards(planeLibrary);
        }

        private void HandlePlaneBuildCompleted(PlaneBluePrintSo bluePrintSo)
        {
            InitializePlaneCards(planeLibrary);
        }

        private void InitializePlaneCards(PlaneLibrarySo planeLibrarySo)
        {
            ClearAllCards();

            foreach (var plane in planeLibrarySo.planeListInGame)
            {
                PlaneListCardUI newCard =
                    Instantiate(planeListCardPrefab, planeListCardContainer).GetComponent<PlaneListCardUI>();

                newCard.SetUp(plane);
                newCard.OnPlaneEditSelected += HandlePlaneEditSelected;
                _spawnedPlaneCards.Add(plane, newCard);
            }
        }

        private void HandlePlaneEditSelected(PlaneBluePrintSo bluePrintSo, int planeId)
        {
            //TODO Load plane
        }

        private void ClearAllCards()
        {
            foreach (var card in _spawnedPlaneCards)
            {
                if (card.Value != null)
                {
                    card.Value.OnPlaneEditSelected -= HandlePlaneEditSelected;
                    // Destroy(card.Value.gameObject);
                }
            }

            _spawnedPlaneCards.Clear();
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlaneBuildCompleted -= HandlePlaneBuildCompleted;
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
            LobbyEvents.OnPlaneLibraryLoaded -= HandlePlaneLibraryLoaded;

            ClearAllCards();
        }
    }
}