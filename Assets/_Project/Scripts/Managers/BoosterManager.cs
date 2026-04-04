using System;
using _Project.Scripts.Enums;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Mechanics.Boosters;
using _Project.Scripts.Static;
using _Project.Scripts.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class BoosterManager : MonoBehaviour
    {
        public static BoosterManager Instance { get; private set; }
        private bool _isLevelFinishing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            BoosterCardUI.OnBoosterUseRequested += TryUseBooster;

            GameEvents.OnLevelFinishing += HandleLevelFinishing;
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GameEvents.OnGameRevived += HandleGameRevived;
        }

        private void HandleLevelFinishing() => _isLevelFinishing = true;
        private void HandleLevelStarted(LevelDataSo data) => _isLevelFinishing = false;
        private void HandleGameRevived(FailType type) => _isLevelFinishing = false;

        private void TryUseBooster(ResourceType type, int amount, Vector3 pos)
        {
            if (_isLevelFinishing) return;

            if (EconomyManager.Instance.TrySpendResource(type, amount))
            {
                switch (type)
                {
                    case ResourceType.FreezeTimeBooster:
                        ProcessFreezeTimerBooster(type);
                        break;
                    case ResourceType.HammerBooster:
                        ProcessHammerBooster(pos);
                        break;
                    case ResourceType.PlaneBombBooster:
                        ProcessPlaneBombBooster(pos);
                        break;
                    case ResourceType.WindBooster:
                        ProcessWindBooster(pos);
                        break;
                }
            }
            else
            {
                Debug.Log("Not enough resource to use booster");
            }
        }

        private void ProcessWindBooster(Vector3 pos)
        {
            WindBoosterMechanic.Instance.PlayWindBoost(pos);
        }

        private void ProcessPlaneBombBooster(Vector3 pos)
        {
            PlaneBombBoosterMechanic.Instance.PlayPlaneBombBoost(pos);
        }

        private void ProcessHammerBooster(Vector3 pos)
        {
            HammerBoosterMechanic.Instance.PlayHammerBoost(pos);
        }

        private void ProcessFreezeTimerBooster(ResourceType type)
        {
            TimeManager.Instance.FreezeTime();
        }

        private void OnDisable()
        {
            BoosterCardUI.OnBoosterUseRequested -= TryUseBooster;

            GameEvents.OnLevelFinishing -= HandleLevelFinishing;
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            GameEvents.OnGameRevived -= HandleGameRevived;
        }
    }
}