using System;
using System.Collections.Generic;
using _Project.Scripts.Components.DoTween;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Lobby.UI.Components;
using _Project.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class PlaneBuildUIManager : MonoBehaviour
    {
        [Header("UI References")] [Tooltip("Main Container")] [SerializeField]
        private Transform gridContainer;

        [Tooltip("Container Canvas group")] [SerializeField]
        private CanvasGroup gridCanvasGroup;

        [Tooltip("Build choice card prefab")] [SerializeField]
        private GameObject buildChoiceCardPrefab;

        [Tooltip("Plane Name -> shown top")] [SerializeField]
        private TextMeshProUGUI planeName;

        [Tooltip("Current wrench amount")] [SerializeField]
        private TextMeshProUGUI wrenchText;

        [Tooltip("Wrench decrease anim duration")] [SerializeField]
        private float wrenchAnimDuration;

        [Tooltip("Card Shake position -> Vector")] [SerializeField]
        private Vector3 shakePosition = new Vector3(10f, 10f, 0);

        [Tooltip("Card Shake rotation -> Vector")] [SerializeField]
        private Vector3 shakeRotation = new Vector3(0, 0, 5f);

        [Tooltip("Plane build progress -> Slider")] [SerializeField]
        private Slider progressSlider;

        [Tooltip("Progress Text")] [SerializeField]
        private TextMeshProUGUI progressText;

        private readonly Dictionary<PlanePartSo, PlaneBuildChoiceCardUI> spawnedChoices =
            new Dictionary<PlanePartSo, PlaneBuildChoiceCardUI>();


        private void OnEnable()
        {
            LobbyEvents.OnAvailablePartsUpdated += HandleAvailablePartsUpdated;
            LobbyEvents.OnPlanePartPurchaseConfirmed += HandlePlanePurchaseConfirmed;
            LobbyEvents.OnPlanePartLoaded += HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildProgressChanged += HandlePlaneBuildProgressChanged;
        }

        private void HandlePlaneBuildProgressChanged(float progress)
        {
            if (progressSlider != null)
                progressSlider.DOValue(progress, 0.5f).SetEase(Ease.OutQuad);

            if (progressText == null) return;

            int startAmount = 0;
            int.TryParse(progressText.text, out startAmount);
            progressText.DoCounterInt(startAmount, Mathf.RoundToInt(progress), 0.5f, Ease.OutExpo, "{0}%");
        }

        private void HandlePlanePartLoaded(PlaneBluePrintSo planeSo, List<SavedPartData> savedPartData)
        {
            if (planeName != null)
            {
                planeName.text = planeSo.planeName;
            }

            var resourceAmount = EconomyManager.Instance.GetResourceAmount(ResourceType.Wrench);
            if (wrenchText != null) wrenchText.text = resourceAmount.ToString();
        }

        private void HandlePlanePurchaseConfirmed(PlanePartSo partSo, PlanePartVariation? partVariation = null)
        {
            if (!spawnedChoices.TryGetValue(partSo, out PlaneBuildChoiceCardUI card)) return;

            ApplyCardAnimation(card);

            ApplyWrenchAnimation(partSo, partVariation, card);
        }

        private void ApplyCardAnimation(PlaneBuildChoiceCardUI card)
        {
            gridCanvasGroup.interactable = false;

            Sequence seq = DOTween.Sequence().SetLink(card.gameObject);

            seq.Append(
                card.transform.DOShakePosition(wrenchAnimDuration, shakePosition, 20, 90)
            );

            seq.Join(
                card.transform.DOShakeRotation(wrenchAnimDuration, shakeRotation, 15)
            );
        }

        private void ApplyWrenchAnimation(PlanePartSo partSo, PlanePartVariation? partVariation,
            PlaneBuildChoiceCardUI card)
        {
            int startWrenchAmount = int.Parse(wrenchText.text);
            int targetWrenchAmount = EconomyManager.Instance.GetResourceAmount(ResourceType.Wrench);

            wrenchText.DoCounterInt(startWrenchAmount, targetWrenchAmount,
                wrenchAnimDuration, Ease.OutExpo).OnComplete(() =>
            {
                card.OnBuildChoiceSelected -= HandleBuildChoiceSelected;
                Destroy(card.gameObject);
                spawnedChoices.Remove(partSo);

                gridCanvasGroup.interactable = true;

                LobbyEvents.TriggerPlanePartBuildAnimStarted(partSo, partVariation);
            });
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
            LobbyEvents.OnPlanePartPurchaseConfirmed -= HandlePlanePurchaseConfirmed;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildProgressChanged -= HandlePlaneBuildProgressChanged;

            ClearAllCards();
        }
    }
}