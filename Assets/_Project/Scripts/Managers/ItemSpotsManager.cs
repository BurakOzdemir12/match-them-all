using System.Collections.Generic;
using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class ItemSpotsManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private InputManager inputManager;

        [SerializeField] private ItemSpot[] availableSpots;
        [SerializeField] private Transform itemSpotsParent;

        [Header("Item Pos Settings")] [SerializeField]
        private Vector3 itemOffsetOnSpot;

        [SerializeField] private Vector3 itemScaleOnSpot;

        private void Awake()
        {
            InitSpots();
            if (inputManager == null) inputManager = GetComponent<InputManager>();
        }


        private void OnEnable()
        {
            inputManager.OnItemClicked += HandleItemClicked;
        }

        private void InitSpots()
        {
            if (availableSpots == null || availableSpots.Length == 0)
                availableSpots = GetComponentsInChildren<ItemSpot>();
        }

        private void HandleItemClicked(GameObject item, IInteractable interactable)
        {
            foreach (var spot in availableSpots)
            {
                if (!spot.IsAvailable) continue;

                spot.Populate(item);

                item.transform.SetParent(spot.transform);

                item.transform.localPosition = itemOffsetOnSpot;
                item.transform.localScale = itemScaleOnSpot;
                item.transform.rotation = Quaternion.identity;

                interactable.Interact();

                break;
            }
        }

        private void OnDisable()
        {
            inputManager.OnItemClicked -= HandleItemClicked;
        }
    }
}