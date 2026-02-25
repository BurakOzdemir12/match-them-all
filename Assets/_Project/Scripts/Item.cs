using System;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Managers;
using UnityEngine;

namespace _Project.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Item : MonoBehaviour, IInteractable
    {
        [Header("Element References")] [SerializeField]
        private new Rigidbody rigidbody;

        [SerializeField] private new Collider collider;
        [SerializeField] private new Renderer renderer;

        private Material _baseMaterial;

        private void Awake()
        {
            if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
            if (!collider) collider = GetComponent<Collider>();
            if (!renderer) renderer = GetComponentInChildren<Renderer>();

            _baseMaterial = renderer.material;
        }

        public void Interact()
        {
            DisableShadows();
            DisablePhysics();
        }


        private void DisableShadows()
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void DisablePhysics()
        {
            rigidbody.isKinematic = true;
            collider.enabled = false;
        }

        public void Select()
        {
            renderer.materials = new Material[] { _baseMaterial, MaterialManager.Instance.OutlineMaterial };
        }

        public void Deselect()
        {
            renderer.materials = new Material[] { _baseMaterial };
        }
    }
}