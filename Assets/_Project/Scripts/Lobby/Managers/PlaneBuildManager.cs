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
                    new List<PartSaveInfo>(),
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

            for (int i = 0; i < _saveData.currentBuildIndex; i++)
            {
                PlanePartSo builtPartSo = _currentPlaneBluePrint.partsToBuildInOrder[i];

                PartSaveInfo saveInfo = _saveData.builtParts.Find(p
                    => p.partType == builtPartSo.planePartType.ToString());

                LobbyEvents.TriggerPlanePartLoaded(builtPartSo, saveInfo);

                Debug.Log(
                    $"[Hangar] Last part loaded: {builtPartSo.planePartType} - selection: {saveInfo.selectedVariationID}");
            }

            UpdateNextTargetInfo();
        }

        private void UpdateNextTargetInfo()
        {
            if (_saveData.currentBuildIndex < _currentPlaneBluePrint.partsToBuildInOrder.Count)
            {
                PlanePartSo nextPart = _currentPlaneBluePrint.partsToBuildInOrder[_saveData.currentBuildIndex];
                LobbyEvents.TriggerNextPartTargeted(nextPart);
            }
        }

        private void CompletePlane()
        {
            _saveData.completedPlaneIDs.Add(_saveData.activePlaneID);
            SaveData();

            // TODO: Celebration effects
            Debug.Log("Congratz plane build is completed");

            LobbyEvents.TriggerAllPlaneBuildCompleted(planeBluePrintSo: _currentPlaneBluePrint);
        }

        public void TryBuildPart(PlanePartSo partSo, PlanePartVariation selectedVariation)
        {
            if (_currentPlaneBluePrint.partsToBuildInOrder[_saveData.currentBuildIndex] != partSo)
            {
                return;
            }

            if (EconomyManager.Instance.TrySpendResource(ResourceType.Wrench, partSo.requiredWrench))
            {
                PartSaveInfo newPart = new PartSaveInfo(partType: partSo.planePartType.ToString(),
                    selectedVariationID: selectedVariation.variationID);

                _saveData.builtParts.Add(newPart);
                _saveData.currentBuildIndex++;

                SaveData();

                LobbyEvents.TriggerPlanePartBuildStarted(partSo, selectedVariation);
                Debug.Log($"Part Built {partSo.planePartType}");

                if (_saveData.currentBuildIndex >= _currentPlaneBluePrint.partsToBuildInOrder.Count)
                {
                    CompletePlane();
                }
                else
                {
                    UpdateNextTargetInfo();
                }
            }
            else
            {
                Debug.Log("Not enough resources to build part");
                //TODO Create How to earn wrench tip -> Play Screen
            }
        }

        //! For testing
        public void OnResetSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
        }
    }
}