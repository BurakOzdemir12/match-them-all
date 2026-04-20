using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace _Project.Scripts.Lobby.Managers
{
    public class LobbyInputManager : MonoBehaviour
    {
        public static LobbyInputManager Instance { get; private set; }
        public static bool enabled { get; }
        public Vector2 lookDelta { get; private set; }
        public float zoomDelta { get; private set; }

        // public static event Action<Finger> onFingerMove;
        // public static event Action<Finger> onFingerDown;
        // public static event Action<Finger> onFingerUp;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void Update()
        {
            lookDelta = Vector2.zero;
            zoomDelta = 0f;

            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    lookDelta = Mouse.current.delta.ReadValue();
                }

                float scrollValue = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollValue) > 0.1f)
                {
                    float normalizedScroll = Mathf.Clamp(scrollValue, -1f, 1f);

                    // zoomDelta = normalizedScroll * 5f;
                    zoomDelta = Mathf.Sign(scrollValue);
                }
            }

            ReadOnlyArray<Touch> activeTouches = Touch.activeTouches;

            if (activeTouches.Count > 0)
            {
                if (activeTouches.Count == 1)
                {
                    lookDelta = activeTouches[0].delta;
                }
                else if (activeTouches.Count == 2)
                {
                    Touch touch1 = activeTouches[0];
                    Touch touch2 = activeTouches[1];

                    Vector2 touch1PrevPos = touch1.screenPosition - touch1.delta;
                    Vector2 touch2PrevPos = touch2.screenPosition - touch2.delta;

                    var prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;

                    float currentMagnitude = (touch1.screenPosition - touch2.screenPosition).magnitude;

                    zoomDelta = (prevMagnitude - currentMagnitude) * 0.02f;
                    lookDelta = Vector2.zero;
                }
            }

            Debug.Log($"Active Touches: {activeTouches.Count}");
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }
    }
}