using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        private const string PlayerCoin = "PlayerCoin";
        public int CurrentCoin { get; set; }

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
            CurrentCoin = PlayerPrefs.GetInt(PlayerCoin, 0);
        }

        private void LoadInventory()
        {
            foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
            {
                PlayerPrefs.SetInt(PlayerCoin, 100);
                int savedAmount = PlayerPrefs.GetInt(resource.ToString(), 0);
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
    }
}