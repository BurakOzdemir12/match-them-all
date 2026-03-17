using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Mechanics.Boosters
{
    public class HammerBoosterMechanic : MonoBehaviour
    {
        public static HammerBoosterMechanic Instance { get; private set; }

        [Header("Hammer Settings")] [SerializeField]
        private GameObject hammerPrefab;

        [SerializeField] private float hammerScale = 2f;

        [Header("Hammer Animation Settings")] [Tooltip("Hammer one Hit animation Duration")] [SerializeField]
        private float hammerHitDuration = 0.4f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        public void PlayHammerBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetRandomIdenticalItemsFromPool(3);
            if (targets == null || targets.Count < 3) return;


            GameEvents.TriggerBoosterAnimationStarted(ResourceType.HammerBooster);



            foreach (var target in targets)
            {
                var targetPos = target.transform.position;

                GameEvents.TriggerBoosterUseRequested(ResourceType.HammerBooster, target);
            }
        }
    }
}