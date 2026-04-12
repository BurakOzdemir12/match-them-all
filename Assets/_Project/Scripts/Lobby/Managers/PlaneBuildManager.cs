using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Managers;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class PlaneBuildManager : MonoBehaviour
    {
        public static PlaneBuildManager Instance { get; private set; }

        [Header("Database")] [Tooltip("All Plane Blue Prints in the game"), SerializeField]
        private List<PlaneBluePrintSo> allPlaneBluePrints = new List<PlaneBluePrintSo>();

        private HangarSaveData _saveData;
        private const string SAVE_KEY = "HangarSaveData_01";

        private PlaneBluePrintSo _currentPlaneBluePrint;
        private PlaneBuildStage _currentBuildStage;

        private readonly List<SavedPartData> _partsToLoad = new List<SavedPartData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            LoadSaveData();
            InitializeHangar();
        }

        private void LoadSaveData()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                _saveData = JsonUtility.FromJson<HangarSaveData>(json);

            }
            else
            {
                _saveData = new HangarSaveData("", "", 0,
                    0, new List<PartSaveInfo>(),
                    new List<string>());

                if (allPlaneBluePrints.Count > 0)
                {
                    _saveData.activePlaneID = allPlaneBluePrints[0].planeID;
                }
            }
        }

        private void SaveData()
        {
            string json = JsonUtility.ToJson(_saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void InitializeHangar()
        {
            _currentPlaneBluePrint = allPlaneBluePrints.Find(b
                => b.planeID == _saveData.activePlaneID);
            if (_currentPlaneBluePrint == null) return;

            _partsToLoad.Clear();

            foreach (PartSaveInfo savedPart in _saveData.builtParts)
            {
                PlanePartSo partSo = FindPartSoByType(savedPart.partType);
                if (partSo != null)
                {
                    _partsToLoad.Add(new SavedPartData(partSo, savedPart));
                    Debug.Log(
                        $"Plane last parts loaded: {partSo.planePartType}- or, name: {partSo.name} " +
                        $"\n Selection variation: {savedPart.selectedVariationID}");
                }
                else
                {
                    Debug.Log($"Part SO not found for type: {savedPart.partType}");
                }
            }

            LobbyEvents.TriggerPlanePartLoaded(_currentPlaneBluePrint, _partsToLoad);

            UpdateAvailablePartsUI();
        }

        private PlanePartSo FindPartSoByType(string partType)
        {
            foreach (var stage in _currentPlaneBluePrint.buildStages)
            foreach (var part in stage.partsInStage)
                if (part.planePartType.ToString() == partType)
                    return part;
            return null;
        }

        private void UpdateAvailablePartsUI()
        {
            if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count) return;

            PlaneBuildStage currentStage = _currentPlaneBluePrint.buildStages[_saveData.currentBuildStageIndex];

            List<PlanePartSo> availablePartsList = new List<PlanePartSo>();

            foreach (var part in currentStage.partsInStage)
            {
                if (!_saveData.builtParts.Exists(p => p.partType == part.planePartType.ToString()))
                {
                    availablePartsList.Add(part);
                }
            }

            LobbyEvents.TriggerAvailablePartsUpdated(availablePartsList);

            // if (_saveData.currentBuildIndex < _currentPlaneBluePrint.partsToBuildInOrder.Count)
            // {
            //     PlanePartSo nextPart = _currentPlaneBluePrint.partsToBuildInOrder[_saveData.currentBuildIndex];
            //     LobbyEvents.TriggerAvailablePartsUpdated(nextPart);
            // }
        }

        private void CompletePlane()
        {
            _saveData.completedPlaneIDs.Add(_saveData.activePlaneID);
            SaveData();

            // TODO: Celebration effects
            Debug.Log("Congratz plane build is completed");

            LobbyEvents.TriggerAllPlaneBuildCompleted(planeBluePrintSo: _currentPlaneBluePrint);
        }

        public void TryBuildPart(PlanePartSo partSo, PlanePartVariation? selectedVariation = null)
        {
            // if (_currentPlaneBluePrint.partsToBuildInOrder[_saveData.currentBuildIndex] != partSo)
            // {
            //     return;
            // }
            if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count) return;

            if (EconomyManager.Instance.TrySpendResource(ResourceType.Wrench, partSo.requiredWrench))
            {
                string variationIDToSave = selectedVariation.HasValue ? selectedVariation.Value.variationID : "NONE";

                PartSaveInfo newPart = new PartSaveInfo(partType: partSo.planePartType.ToString(),
                    selectedVariationID: variationIDToSave);

                _saveData.builtParts.Add(newPart);
                // _saveData.currentBuildIndex++;

                SaveData();

                LobbyEvents.TriggerPlanePartPurchaseConfirmed(partSo, partSo.hasVariation ? selectedVariation : null);
                Debug.Log($"Part Built {partSo.planePartType} or name -> {partSo.name}");

                CheckStageCompletion();

                // if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count)
                // {
                //     CompletePlane();
                // }
                // else
                // {
                //     UpdateAvailablePartsUI();
                // }
            }
            else
            {
                Debug.Log("Not enough resources to build part");
                //TODO Create How to earn wrench tip -> Play Screen
            }
        }

        private void CheckStageCompletion()
        {
            if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count) return;

            _currentBuildStage = _currentPlaneBluePrint.buildStages[_saveData.currentBuildStageIndex];
            bool isStageComplete = true;

            foreach (var part in _currentBuildStage.partsInStage)
            {
                if (!_saveData.builtParts.Exists(p => p.partType == part.planePartType.ToString()))
                {
                    isStageComplete = false;
                    break;
                }
            }

            if (isStageComplete)
            {
                _saveData.currentBuildStageIndex++;
                SaveData();

                if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count)
                {
                    CompletePlane();
                    LobbyEvents.TriggerAvailablePartsUpdated(new List<PlanePartSo>());
                }
                else
                {
                    UpdateAvailablePartsUI();
                }
            }
        }

        //! For testing
        public void OnResetSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
        }
    }
}