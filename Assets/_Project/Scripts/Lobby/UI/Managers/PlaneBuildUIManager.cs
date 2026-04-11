using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Lobby.UI.Components;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class PlaneBuildUIManager : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private Transform gridContainer;

        [SerializeField] private GameObject buildChoiceCardPrefab;

        [Tooltip("Plane Name -> shown top")] [SerializeField]
        private TextMeshProUGUI planeName;

        private readonly Dictionary<PlanePartSo, PlaneBuildChoiceCardUI> spawnedChoices =
            new Dictionary<PlanePartSo, PlaneBuildChoiceCardUI>();

        private void OnEnable()
        {
            LobbyEvents.OnAvailablePartsUpdated += HandleAvailablePartsUpdated;
            LobbyEvents.OnPlanePartBuildStarted += HandlePlaneBuildStarted;
            LobbyEvents.OnPlanePartLoaded += HandlePlanePartLoaded;
        }

        private void HandlePlanePartLoaded(PlaneBluePrintSo planeSo, List<SavedPartData> savedPartData)
        {
            if (planeName != null)
            {
                planeName.text = planeSo.planeName;
            }
        }

        private void HandlePlaneBuildStarted(PlanePartSo partSo, PlanePartVariation? partVariation = null)
        {
            if (spawnedChoices.TryGetValue(partSo, out PlaneBuildChoiceCardUI card))
            {
                card.OnBuildChoiceSelected -= HandleBuildChoiceSelected;
                Destroy(card.gameObject);
                spawnedChoices.Remove(partSo);
            }
        }

        private void HandleAvailablePartsUpdated(List<PlanePartSo> partSo)
        {
            ClearAllCards();

            InitializeBuildChoiceCards(partSo);
        }


        private void InitializeBuildChoiceCards(List<PlanePartSo> partSoList)
        {
            ClearAllCards();

            foreach (var partSo in partSoList)
            {
                PlaneBuildChoiceCardUI newCard =
                    Instantiate(buildChoiceCardPrefab, gridContainer).GetComponent<PlaneBuildChoiceCardUI>();
                newCard.Setup(partSo);
                newCard.OnBuildChoiceSelected += HandleBuildChoiceSelected;
                spawnedChoices.Add(partSo, newCard);
            }
        }

        private void HandleBuildChoiceSelected(PlanePartSo partSo)
        {
            PlaneBuildManager.Instance.TryBuildPart(partSo, partSo.hasVariation ? partSo.variations[0] : null);
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
            LobbyEvents.OnAvailablePartsUpdated -= HandleAvailablePartsUpdated;
            LobbyEvents.OnPlanePartBuildStarted -= HandlePlaneBuildStarted;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;

            ClearAllCards();
        }
    }
}