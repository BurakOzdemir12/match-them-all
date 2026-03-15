using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class BoosterCardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI boosterAmountText;
        [SerializeField] private ResourceType myBoosterType;

        public static event Action<ResourceType, int> OnBoosterUseRequested;

        private void OnEnable()
        {
            EconomyManager.OnResourceAmountChanged += HandleResourceAmountChanged;
        }

        private void Start()
        {
            //! This is for test don't forget to delete it.
            EconomyManager.Instance.SetResource(myBoosterType, 3);

            int currentAmount = EconomyManager.Instance.GetResourceAmount(myBoosterType);
            UpdateBoosterAmount(currentAmount);
        }

        private void HandleResourceAmountChanged(ResourceType type, int amount)
        {
            if (type == myBoosterType)
            {
                UpdateBoosterAmount(amount);
            }
        }

        private void UpdateBoosterAmount(int amount)
        {
            boosterAmountText.text = amount.ToString();
        }

        public void OnBoosterClicked()
        {
            OnBoosterUseRequested?.Invoke(myBoosterType, 1);
        }

        private void OnDisable()
        {
            EconomyManager.OnResourceAmountChanged -= HandleResourceAmountChanged;
        }
    }
}