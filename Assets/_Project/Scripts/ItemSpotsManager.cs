using System;
using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts
{
    public class ItemSpotsManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private InputManager inputManager;

        [SerializeField] private Transform itemSpot;

        [Header("Item Pos Settings")] [SerializeField]
        private Vector3 itemOffsetOnSpot;

        [SerializeField] private Vector3 itemScaleOnSpot;

        private void Awake()
        {
            if (inputManager == null) inputManager = GetComponent<InputManager>();
        }

        private void OnEnable()
        {
            inputManager.OnItemClicked += HandleItemClicked;
        }

        private void HandleItemClicked(GameObject item, IInteractable interactable)
        {
            item.transform.SetParent(itemSpot);

            item.transform.localPosition = itemOffsetOnSpot;
            item.transform.localScale = itemScaleOnSpot;

            interactable.Interact();
        }

        private void OnDisable()
        {
            inputManager.OnItemClicked -= HandleItemClicked;
        }
    }
}