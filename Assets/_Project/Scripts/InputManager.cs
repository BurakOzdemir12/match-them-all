using System;
using _Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts
{
    public class InputManager : MonoBehaviour
    {
        public event Action<GameObject, IInteractable> OnItemClicked;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleMousePressed();
        }

        private void HandleMousePressed()
        {
            Physics.Raycast(_mainCamera.ScreenPointToRay(Mouse.current.position.value), out RaycastHit hit, 150);

            if (!hit.collider) return;

            if (!hit.collider.TryGetComponent(out IInteractable interactable)) return;

            Debug.Log("Mouse Clicked on Object name: " + hit.collider.gameObject.name + "");
            OnItemClicked?.Invoke(hit.collider.gameObject, interactable);
        }
    }
}