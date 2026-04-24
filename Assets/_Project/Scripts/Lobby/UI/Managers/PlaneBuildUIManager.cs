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

        [Tooltip("Build UI Panel")] [SerializeField]
        private RectTransform buildPanel;

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

        [Space(10)]
        [Header("Plane build variation select Panel")]
        [Tooltip("Build variation card prefab")]
        [SerializeField]
        private GameObject variationCardPrefab;

        [Tooltip("Variations Container")] [SerializeField]
        private Transform variationsContainer;

        [Header("Build Congratz Panel")] [Tooltip("Build Completed panel")] [SerializeField]
        private Transform buildCompletedPanel;

        //---
        private readonly Dictionary<PlanePartSo, PlaneBuildChoiceCardUI> spawnedChoices =
            new Dictionary<PlanePartSo, PlaneBuildChoiceCardUI>();

        private List<PlaneBuildVariationCardUI> spawnedVariationCards = new List<PlaneBuildVariationCardUI>();

        public static event Action OnPlaneBuildVariationSelectProcess;


        private void OnEnable()
        {
            LobbyEvents.OnAvailablePartsUpdated += HandleAvailablePartsUpdated;
            LobbyEvents.OnPlanePartBuildRequestConfirmed += HandlePlaneBuildRequestConfirmed;
            LobbyEvents.OnPlanePartLoaded += HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildProgressChanged += HandlePlaneBuildProgressChanged;
            LobbyEvents.OnPlaneBuildCompleted += HandlePlaneBuildCompleted;
        }

        private void HandlePlaneBuildCompleted(PlaneBluePrintSo bluePrintSo)
        {
            if (progressSlider != null)
                progressSlider.DOValue(0f, 0.5f).SetEase(Ease.OutQuad);

            if (progressText == null) return;

            int startAmount = 100;

            int.TryParse(progressText.text, out startAmount);

            progressText.DoCounterInt(startAmount, Mathf.RoundToInt(0f), 0.5f, Ease.OutExpo, "{0}%");

            buildPanel.gameObject.SetActive(false);

            buildCompletedPanel.gameObject.SetActive(true);
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

        private void HandlePlaneBuildRequestConfirmed(PlanePartSo partSo, PlanePartVariation? partVariation = null)
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

        //? Spawns Main build choice cards
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
            if (EconomyManager.Instance.TrySpendResource(ResourceType.Wrench, partSo.requiredWrench))
            {
                //? If it has variations for part first open variation select panel, if not directly build part
                if (partSo.hasVariation)
                {
                    InitializeVariationsCard(partSo);

                    OnPlaneBuildVariationSelectProcess?.Invoke();
                }
                else
                {
                    PlaneBuildManager.Instance.TryBuildPart(partSo, null);
                }
            }
            else
            {
                Debug.Log("Not enough resources to build part");
                //TODO Create How to earn wrench tip -> Play Screen
            }
        }

        //? Spawns selected plane part's variation cards
        private void InitializeVariationsCard(PlanePartSo partSo)
        {
            ClearAllVariationCards();

            partSo.variations.ForEach(variation =>
            {
                // Debug.Log("Adding variation: " + variation.variationName);

                PlaneBuildVariationCardUI newCard =
                    Instantiate(variationCardPrefab, variationsContainer)
                        .GetComponent<PlaneBuildVariationCardUI>();
                newCard.SetUp(partSo, variation);
                newCard.OnBuildVariationSelected += HandleBuildVariationSelected;
                spawnedVariationCards.Add(newCard);
            });
        }

        private void HandleBuildVariationSelected(PlanePartSo partSo, PlanePartVariation variation)
        {
            PlaneBuildManager.Instance.TryBuildPart(partSo, variation);

            // LobbyEvents.TriggerPlanePartBuildAnimStarted(partSo, variation);
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

        private void ClearAllVariationCards()
        {
            foreach (var card in spawnedVariationCards)
            {
                if (card != null)
                {
                    card.OnBuildVariationSelected -= HandleBuildVariationSelected;
                    Destroy(card.gameObject);
                }
            }

            spawnedVariationCards.Clear();
        }


        private void OnDisable()
        {
            LobbyEvents.OnAvailablePartsUpdated -= HandleAvailablePartsUpdated;
            LobbyEvents.OnPlanePartBuildRequestConfirmed -= HandlePlaneBuildRequestConfirmed;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildProgressChanged -= HandlePlaneBuildProgressChanged;
            LobbyEvents.OnPlaneBuildCompleted -= HandlePlaneBuildCompleted;

            ClearAllCards();
            ClearAllVariationCards();
        }
    }
}