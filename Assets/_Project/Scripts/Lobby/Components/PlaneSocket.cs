using System;
using _Project.Scripts.Lobby.Enums;
using UnityEngine;

namespace _Project.Scripts.Lobby.Components
{
    public class PlaneSocket : MonoBehaviour
    {
        [field: Header("Plane part Type"), SerializeField]
        public PlanePartType planePartType { get; private set; }

        [Header("References")] [SerializeField]
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<MeshRenderer>();
            }
        }

        public void ToggleMeshRenderer(bool isActive)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = isActive;
            }
        }
    }
}