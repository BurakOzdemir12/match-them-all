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

        [Tooltip("Parts will spawn on this position ")] [SerializeField]
        private Vector3 partSpawnPosition;

        [Tooltip("Ease type for part install")] [SerializeField]
        private Ease installEase = Ease.InOutSine;

        [Tooltip("Interval before the install animation.")] [SerializeField]
        private float intervalDuration = 0.5f;

        [Header("Plane Paint Settings")] [Tooltip("Plane Renderer")] [SerializeField]
        private Renderer planeRenderer;

        [Tooltip("Total time for paint animation.")] [SerializeField]
        private float paintDuration;

        private readonly int _mainPaintID = Shader.PropertyToID("_MainTexture");
        private readonly int _newTextureID = Shader.PropertyToID("_NewTexture");
        private readonly int _paintSpeedId = Shader.PropertyToID("_PaintSpeed");

        private PlaneSocketManager _currentSocketManager;

        private readonly List<SavedPartData> _savedPartList = new List<SavedPartData>();

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
                _savedPartList.Add(part);
            }

            foreach (var savedPart in _savedPartList)
            {
                if (!_currentSocketManager.sockets.TryGetValue(savedPart.partSo.planePartType,
                        out Transform targetPart))
                    continue;

                SpawnPart(targetPart, savedPart);
            }
        }


        private void SpawnPart(Transform targetPart, SavedPartData savedData)
        {
            var partId = savedData.saveInfo.selectedVariationID;

            GameObject part = savedData.partSo.GetPrefabToSpawn(partId);

            GameObject partToInstall = Instantiate(part, targetPart.position,
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
            //TODO Create Shader values and control paint flow 

            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out Transform targetSocketTransform))
                return;

            Material mat = planeRenderer.material;

            Texture2D finalTexture = partVariation != null
                ? partVariation.Value.texture
                : partSo.defaultTexture;

            mat.SetTexture(_newTextureID, finalTexture);

            mat.SetFloat(_paintSpeedId, -10f);

            mat.DOFloat(10f, _paintSpeedId, paintDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => { mat.SetTexture(_mainPaintID, finalTexture); });
        }

        private void ProcessInstall(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            //TODO Create CineMachine camera and Call Camera Movement from CameraController.

            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out Transform targetSocketTransform))
                return;

            GameObject finalPart = partVariation != null ? partVariation.Value.partPrefab : partSo.defaultPartPrefab;

            GameObject partToInstall = Instantiate(finalPart, partSpawnPosition,
                Quaternion.identity);

            Sequence seq = DOTween.Sequence().SetLink(partToInstall);

            seq.AppendInterval(intervalDuration);

            seq.Append(partToInstall.transform.DOMove(targetSocketTransform.position, partInstallDuration)
                .SetEase(installEase));
            seq.Join(partToInstall.transform.DORotate(targetSocketTransform.rotation.eulerAngles, partInstallDuration));

            seq.OnComplete(() => { LobbyEvents.TriggerPlanePartBuildAnimEnded(partSo, partVariation); });
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted -= HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;
        }
    }
}