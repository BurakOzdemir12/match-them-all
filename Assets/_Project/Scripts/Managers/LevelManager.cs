using System;
using System.Collections.Generic;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Static;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Levels")] [SerializeField] private List<LevelDataSo> levelList;
        public IReadOnlyList<LevelDataSo> LevelList => levelList;
        private const string LevelSaveKey = "SavedLevelIndex";

        private LevelDataSo _currentLevelData;
        private int _currentLevelIndex = 0;

        private void Awake()
        {
            // _currentLevelData = levelList[_currentLevelIndex];
            _currentLevelIndex = PlayerPrefs.GetInt(LevelSaveKey, 0);
        }

        private void OnEnable()
        {
            GameEvents.OnLevelCompleted += AdvanceToNextLevel;
        }

        private void Start()
        {
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            if (levelList == null || levelList.Count == 0)
            {
                Debug.LogError("Level list is empty! Please assign levels in the inspector.");
                return;
            }

            int indexToLoad = _currentLevelIndex % levelList.Count;

            LevelDataSo levelToLoad = levelList[indexToLoad];
            GameEvents.TriggerLevelStarted(levelToLoad);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void AdvanceToNextLevel()
        {
            _currentLevelIndex++;

            PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
            PlayerPrefs.Save();

            Debug.Log($"Level completed! new Level Index: {_currentLevelIndex}");
        }

        private void OnDisable()
        {
            GameEvents.OnLevelCompleted -= AdvanceToNextLevel;
        }
    }
}