using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Mechanics.Boosters;
using _Project.Scripts.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class BoosterManager : MonoBehaviour
    {
        public static BoosterManager Instance { get; private set; }


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
        }

        private void TryUseBooster(ResourceType type, int amount, Vector3 pos)
        {
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
                }
            }
            else
            {
                Debug.Log("Not enough resource to use booster");
            }
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
        }
    }
}