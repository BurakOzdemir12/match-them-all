using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool _pendingStageUIUpdate;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted += HandlePlanePartBuildAnimStarted;
        }

        private void HandlePlanePartBuildAnimStarted(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            if (_pendingStageUIUpdate)
            {
                _pendingStageUIUpdate = false;

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

        private void Start()
        {
            LoadSaveData();
            InitializePlane();
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


        private void InitializePlane()
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
                        $"\n Selection variation id: {savedPart.selectedVariationID}");
                }
                else
                {
                    Debug.Log($"Part SO not found for type: {savedPart.partType}");
                }
            }

            LobbyEvents.TriggerPlanePartLoaded(_currentPlaneBluePrint, _partsToLoad);

            CalculateBuildProgress();

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
        }

        public void TryBuildPart(PlanePartSo partSo, PlanePartVariation? selectedVariation = null)
        {
            if (_saveData.currentBuildStageIndex >= _currentPlaneBluePrint.buildStages.Count) return;

            string variationIDToSave = selectedVariation.HasValue ? selectedVariation.Value.variationID : "NONE";

            PartSaveInfo newPart = new PartSaveInfo(partType: partSo.planePartType.ToString(),
                selectedVariationID: variationIDToSave);

            _saveData.builtParts.Add(newPart);
            // _saveData.currentBuildIndex++;

            SaveData();

            CalculateBuildProgress();

            LobbyEvents.TriggerPlanePartBuildRequestConfirmed(partSo, partSo.hasVariation ? selectedVariation : null);
            Debug.Log(
                $"Part Built {partSo.planePartType} or name -> {partSo.name}-- {selectedVariation?.variationName}");

            CheckStageCompletion();
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

                _pendingStageUIUpdate = true;
            }
        }

        private void CalculateBuildProgress()
        {
            var totalBuildCount =
                _currentPlaneBluePrint.buildStages
                    .Sum(stage => stage.partsInStage
                        .Count(part => part != null));
            float currentBuiltCount = _saveData.builtParts.Count;
            float progressPercentage = (currentBuiltCount / totalBuildCount) * 100f;

            LobbyEvents.TriggerPlaneBuildProgressChanged(progressPercentage);
        }

        private void SaveData()
        {
            string json = JsonUtility.ToJson(_saveData);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void CompletePlane()
        {
            _saveData.completedPlaneIDs.Add(_saveData.activePlaneID);
            SaveData();

            // TODO: Celebration effects
            Debug.Log("Congratz plane build is completed");

            LobbyEvents.TriggerAllPlaneBuildCompleted(planeBluePrintSo: _currentPlaneBluePrint);
        }


        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted -= HandlePlanePartBuildAnimStarted;
        }

        //! For testing
        public void OnResetSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
        }
    }
}