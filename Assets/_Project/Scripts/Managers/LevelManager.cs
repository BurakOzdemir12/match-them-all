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

        private LevelDataSo _currentLevelData;
        private int _currentLevelIndex = 0;

        private void Awake()
        {
        }

            _currentLevelData = levelList[_currentLevelIndex];
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

            if (_currentLevelIndex >= levelList.Count)
            {
                _currentLevelIndex = 0;
            }

            LevelDataSo levelToLoad = levelList[_currentLevelIndex];

            GameEvents.TriggerLevelStarted(levelToLoad);
        }
    }
}