using System;
using _Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static event Action<IInteractable> OnItemClicked;
        public static event Action<IInteractable> OnItemSelected;

        private Camera _mainCamera;
        private IInteractable _interactable;
        private GameObject _currentItem;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        void Update()
        {
            if (Pointer.current.press.isPressed) //Mouse.current.leftButton.isPressed
                HandleMouseClick();
            else if (Pointer.current.press.wasReleasedThisFrame) //Mouse.current.leftButton.wasReleasedThisFrame
                HandleMouseRelease();
        }

        private void HandleMouseClick()
        {
            // Mouse.current.position.value
            Vector2 pointerPos = Pointer.current.position.value;
            Physics.Raycast(_mainCamera.ScreenPointToRay(pointerPos), out RaycastHit hit, 150,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if (!hit.collider || !hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (_interactable != null)
                {
                    _interactable.Deselect();
                    _interactable = null;
                    _currentItem = null;
                }

                return;
            }

            if (_currentItem == hit.collider.gameObject) return;

            _interactable?.Deselect();

            _currentItem = hit.collider.gameObject;
            _interactable = interactable;

            _interactable.Select();

            OnItemSelected?.Invoke(_interactable);
        }

        private void HandleMouseRelease()
        {
            if (!_currentItem) return;
            _interactable.Deselect();
            OnItemClicked?.Invoke(_interactable);
            _currentItem = null;
            _interactable = null;
        }
    }
}