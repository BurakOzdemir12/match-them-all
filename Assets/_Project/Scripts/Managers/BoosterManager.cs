using System;
using _Project.Scripts.Enums;
using _Project.Scripts.UI.Components;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class BoosterManager : MonoBehaviour
    {
        public static BoosterManager Instance { get; private set; }

        public static event Action<ResourceType, int> OnBoosterUsed;

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

        private void TryUseBooster(ResourceType type, int amount)
        {
            if (EconomyManager.Instance.TrySpendResource(type, amount))
            {
                switch (type)
                {
                    case ResourceType.FreezeTimeBooster:
                        ProcessFreezeTimerBooster(type);
                        break;
                }
            }
            else
            {
                Debug.Log("Not enough resource to use booster");
            }
        }

        private void ProcessFreezeTimerBooster(ResourceType type)
        {
            TimeManager.Instance.FreezeTime();
            OnBoosterUsed?.Invoke(type, 1);
            Debug.Log("ProcessFreezeTimerBooster");
        }

        private void OnDisable()
        {
            BoosterCardUI.OnBoosterUseRequested -= TryUseBooster;
        }
    }
}