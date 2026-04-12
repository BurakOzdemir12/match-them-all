using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Lobby.Controllers
{
    public class PlaneVisualController : MonoBehaviour
    {
        [Header("Install Animation settings")] [Tooltip("Plane Parts install total anim time")] [SerializeField]
        private float partInstallDuration = 2f;

        private PlaneSocketManager _currentSocketManager;

        private readonly List<PlanePartSo> _savedPartList = new List<PlanePartSo>();

        private bool _isPlaneSpawned = false;
        private bool _isDataLoaded = false;
        private PlaneBluePrintSo _cachedBluePrint;
        private List<SavedPartData> _cachedSavedParts;

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted += HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
            LobbyEvents.OnPlanePartLoaded += HandlePlanePartLoaded;
        }

        private void HandlePlanePartLoaded(PlaneBluePrintSo bluePrintSo, List<SavedPartData> savedPartData)
        {
            _cachedBluePrint = bluePrintSo;
            _cachedSavedParts = savedPartData;
            _isDataLoaded = true;
            TryInitializeVisuals();
        }

        private void HandlePlaneSpawned(PlaneSocketManager socketManager)
        {
            _currentSocketManager = socketManager;
            _isPlaneSpawned = _currentSocketManager != null;

            TryInitializeVisuals();
        }

        private void TryInitializeVisuals()
        {
            if (_isPlaneSpawned && _isDataLoaded)
            {
                InitializePlaneVisuals(_cachedBluePrint, _cachedSavedParts);
            }
        }

        private void InitializePlaneVisuals(PlaneBluePrintSo bluePrintSo, List<SavedPartData> savedPartData)
        {
            _savedPartList.Clear();

            foreach (var part in savedPartData)
            {
                _savedPartList.Add(part.partSo);
            }

            foreach (var savedPart in _savedPartList)
            {
                if (!_currentSocketManager.sockets.TryGetValue(savedPart.planePartType,
                        out Transform targetPart))
                    continue;
                SpawnPart(targetPart, savedPart);
            }
        }

        private void SpawnPart(Transform targetPart, PlanePartSo partSo)
        {
            GameObject defaultPart = partSo.defaultPartPrefab;

            GameObject partToInstall = Instantiate(defaultPart, targetPart.position,
                Quaternion.identity);
        }

        private void HandlePlanePartBuildAnimStarted(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            switch (partSo.modificationType)
            {
                case ModificationType.Install:
                    ProcessInstall(partSo, partVariation);
                    break;
                case ModificationType.Paint:
                    ProcessPaint(partSo, partVariation);
                    break;
            }
        }

        private void ProcessPaint(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
        }

        private void ProcessInstall(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            //TODO Create CineMachine camera and Call Camera Movement from CameraController.

            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out Transform targetSocketTransform))
                return;

            var defaultPart = partSo.defaultPartPrefab;
            Sequence seq = DOTween.Sequence().SetLink(defaultPart);

            GameObject partToInstall = Instantiate(defaultPart,
                defaultPart.transform.position + new Vector3(-20f, 0, 0),
                Quaternion.identity);

            seq.AppendInterval(0.5f);

            seq.Append(partToInstall.transform.DOMove(targetSocketTransform.position, partInstallDuration)
                .SetEase(Ease.InOutBack));
            seq.Join(partToInstall.transform.DORotate(targetSocketTransform.rotation.eulerAngles, partInstallDuration));

            seq.OnComplete(() => { LobbyEvents.TriggerPlanePartBuildAnimEnded(partSo, partVariation); });
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartPurchaseConfirmed -= HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;
        }
    }
}