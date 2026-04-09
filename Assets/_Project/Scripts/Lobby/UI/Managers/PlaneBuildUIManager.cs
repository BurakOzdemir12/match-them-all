using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Lobby.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class PlaneBuildUIManager : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Transform gridContainer;

        [SerializeField] private GameObject buildChoiceCardPrefab;

        private readonly Dictionary<PlanePartSo, PlaneBuildChoiceCardUI> spawnedChoices =
            new Dictionary<PlanePartSo, PlaneBuildChoiceCardUI>();

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildTargeted += HandlePlanePartBuildTargeted;
            LobbyEvents.OnPlanePartBuildStarted += HandlePlaneBuildStarted;
        }

        private void HandlePlaneBuildStarted(PlanePartSo partSo, PlanePartVariation partVariation)
        {
            if (spawnedChoices.TryGetValue(partSo, out PlaneBuildChoiceCardUI card))
            {
                card.OnBuildChoiceSelected -= HandleBuildChoiceSelected;
                Destroy(card.gameObject);
                spawnedChoices.Remove(partSo);
            }
        }

        private void HandlePlanePartBuildTargeted(PlanePartSo partSo)
        {
            InitializeBuildChoiceCards(partSo);
        }

        private void InitializeBuildChoiceCards(PlanePartSo partSo)
        {
            ClearAllCards();

            PlaneBuildChoiceCardUI newCard =
                Instantiate(buildChoiceCardPrefab, gridContainer).GetComponent<PlaneBuildChoiceCardUI>();

            newCard.Setup(partSo);
            newCard.OnBuildChoiceSelected += HandleBuildChoiceSelected;

            spawnedChoices.Add(partSo, newCard);
        }

        private void HandleBuildChoiceSelected(PlanePartSo partSo)
        {
            PlaneBuildManager.Instance.TryBuildPart(partSo, partSo.variations[0]);
        }

        private void ClearAllCards()
        {
            foreach (var kvp in spawnedChoices)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.OnBuildChoiceSelected -= HandleBuildChoiceSelected;
                    Destroy(kvp.Value.gameObject);
                }
            }

            spawnedChoices.Clear();
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildTargeted -= HandlePlanePartBuildTargeted;
            LobbyEvents.OnPlanePartBuildStarted -= HandlePlaneBuildStarted;

            ClearAllCards();
        }
    }
}