using System;
using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Item : MonoBehaviour, IInteractable
    {
        [Header("Element References")] [SerializeField]
        private new Rigidbody rigidbody;

        [SerializeField] private new Collider collider;

        private void Awake()
        {
            if (!collider) collider = GetComponent<Collider>();
            if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        }

        public void Interact()
        {
            DisableShadows();
            DisablePhysics();
        }

        private void DisableShadows()
        {
        }

        private void DisablePhysics()
        {
            rigidbody.isKinematic = true;
            collider.enabled = false;
        }
    }
}