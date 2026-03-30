using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Mechanics.Revive
{
    public class SlotClearReviveMechanic : MonoBehaviour
    {
        [Header("References")] [Tooltip("Vehicle Prefab")] [SerializeField]
        private GameObject vehiclePrefab;

        [Header("Spawn and Animation Settings")] [Tooltip("vehicle movement speed")] [SerializeField]
        private float vehicleSpeed;

        [Tooltip("Vehicle offset")] [SerializeField]
        private Vector3 vehicleOffset;

        [Tooltip("Vehicle spawn position")] [SerializeField]
        private GameObject vehicleSpawnPos;

        [Tooltip("Vehicle exit position")] [SerializeField]
        private GameObject vehicleExitPos;

        [Tooltip("Total animation duration")] [SerializeField]
        private float totalDuration;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            GameEvents.OnGameRevived += HandleGameRevived;
        }

        private void HandleGameRevived(FailType failType)
        {
            if (failType != FailType.SpotFull) return;
            
            ProcessVehicleAnimation();
        }

        private void ProcessVehicleAnimation()
        {
            GameEvents.TriggerBoosterAnimationStarted(ResourceType.ReviveSlot);

            //? Spawn and rotate vhicle prefab
            GameObject vehicle = Instantiate(vehiclePrefab, vehicleSpawnPos.transform.position,
                Quaternion.identity);

            if (!vehicle) return;

            vehicle.transform.rotation = Quaternion.Euler(0, 90, 80);

            Sequence seq = DOTween.Sequence().SetLink(vehicle);

            SoundEmitter vehicleSoundEmitter = null;
            seq.Append(
                vehicle.transform.DOMove(vehicleExitPos.transform.position, totalDuration)
                    .SetEase(Ease.Linear).OnStart(() =>
                    {
                        vehicleSoundEmitter =
                            SoundManager.Instance.PlaySoundByType(SoundType.LuggageVehicleEngine,
                                _mainCamera.transform.position);
                    }).OnComplete(() =>
                    {
                        if (vehicleSoundEmitter != null) vehicleSoundEmitter.Stop();
                    }));

            GameEvents.TriggerBoosterAnimationEnded(ResourceType.ReviveSlot);

            seq.OnComplete(() => Destroy(vehicle));
        }

        private void OnDisable()
        {
            GameEvents.OnGameRevived -= HandleGameRevived;
        }
    }
}