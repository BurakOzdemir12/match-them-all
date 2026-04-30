using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class PlaneListUIManager : MonoBehaviour
    {
        [Header("Panel references")] [SerializeField]
        private CanvasGroup planeListPanelCanvasGroup;

        [Header("UI Elements")] [SerializeField]
        private PlaneListCardUI planeListCardPrefab;

        [SerializeField] private Transform planeListCardContainer;

        private void OnEnable()
        {
            LobbyEvents.OnPlaneBuildCompleted += HandlePlaneBuildCompleted;
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
            LobbyEvents.OnPlaneLibraryLoaded += HandlePlaneLibraryLoaded;
        }

        private void HandlePlaneLibraryLoaded(PlaneLibrarySo library)
        {
            InitializePlaneCards(library);
        }

        private void HandlePlaneSpawned(PlaneSocketManager socketManager)
        {
        }

        private void HandlePlaneBuildCompleted(PlaneBluePrintSo bluePrintSo)
        {
        }

        private void InitializePlaneCards(PlaneLibrarySo planeLibrarySo)
        {
            // ClearAllCards();

            foreach (var plane in planeLibrarySo.planeListInGame)
            {
                PlaneListCardUI newCard =
                    Instantiate(planeListCardPrefab, planeListCardContainer).GetComponent<PlaneListCardUI>();

                newCard.SetUp(plane);
            }
        }

        private void OnDisable()
        {
        }
    }
}