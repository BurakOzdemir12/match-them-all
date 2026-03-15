using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        public Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();

        public static event Action<ResourceType, int> OnResourceAmountChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            LoadInventory();
        }

        private void Start()
        {
        }

        private void LoadInventory()
        {
            foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
            {
                int defaultValue = (resource == ResourceType.Coin) ? 100 : 0;

                int savedAmount = PlayerPrefs.GetInt(resource.ToString(), defaultValue);
                inventory[resource] = savedAmount;
            }
        }

        public int GetResourceAmount(ResourceType resource)
        {
            return inventory.ContainsKey(resource) ? inventory[resource] : 0;
        }

        public void AddResource(ResourceType resource, int amount)
        {
            if (amount < 0) return;
            inventory[resource] += amount;

            PlayerPrefs.SetInt(resource.ToString(), inventory[resource]);
            PlayerPrefs.Save();

            OnResourceAmountChanged?.Invoke(resource, inventory[resource]);
        }

        public bool TrySpendResource(ResourceType resource, int amount)
        {
            if (amount <= 0) return false;

            if (inventory[resource] >= amount)
            {
                inventory[resource] -= amount;

                PlayerPrefs.SetInt(resource.ToString(), inventory[resource]);
                PlayerPrefs.Save();

                OnResourceAmountChanged?.Invoke(resource, inventory[resource]);
                return true;
            }

            return false;
        }

        public void SetResource(ResourceType resource, int amount)
        {
            if (amount < 0) return;
            inventory[resource] = amount;

            PlayerPrefs.SetInt(resource.ToString(), inventory[resource]);
            PlayerPrefs.Save();

            OnResourceAmountChanged?.Invoke(resource, inventory[resource]);
        }
    }
}