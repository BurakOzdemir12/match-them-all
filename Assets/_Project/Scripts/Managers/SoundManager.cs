using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("References")] [SerializeField]
        private AudioSource audioSource;
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;

        }

        private void OnEnable()
        {
            GameEvents.OnMergeCompleted += HandleMergeCompleted;
        }

        private void HandleMergeCompleted(Vector3 pos, ItemType itemType)
        {
        }


        private void OnDisable()
        {
            GameEvents.OnMergeCompleted -= HandleMergeCompleted;
        }
    }
}