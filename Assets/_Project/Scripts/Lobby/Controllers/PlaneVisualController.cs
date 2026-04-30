using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.CameraScripts;
using _Project.Scripts.Lobby.Components;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Lobby.UI.Managers;
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

        [Tooltip("Skeleton spawn offset ")] [SerializeField]
        private Vector3 skeletonSpawnOffset;

        [Tooltip("Ease type for part install")] [SerializeField]
        private Ease installEase = Ease.InOutSine;

        [Tooltip("Interval before the install animation.")] [SerializeField]
        private float intervalDuration = 0.5f;

        [Header("Plane Paint Settings")] [Tooltip("Total time for paint animation.")] [SerializeField]
        private float paintDuration;

        //--
        private readonly int _baseMapID = Shader.PropertyToID("_BaseMap");
        private readonly int _mainPaintID = Shader.PropertyToID("_MainTexture");
        private readonly int _newTextureID = Shader.PropertyToID("_NewTexture");
        private readonly int _paintSpeedId = Shader.PropertyToID("_PaintSpeed");

        private readonly List<SavedPartData> _savedPartList = new List<SavedPartData>();

        private bool _isPlaneSpawned = false;
        private bool _isDataLoaded = false;

        private PlaneSocketManager _currentSocketManager;
        private PlaneBluePrintSo _cachedBluePrint;
        private List<SavedPartData> _cachedSavedParts;

        private Sequence _animSequence;
        private bool _isWaitingForCelebration = false;

        private void OnEnable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted += HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
            LobbyEvents.OnPlanePartLoaded += HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildCompleted += HandlePlaneBuildCompleted;
        }

        private void HandlePlaneBuildCompleted(PlaneBluePrintSo bluePrintSo)
        {
            //? Get rid of old plane information
            _isWaitingForCelebration = true;
        }

        private void StartPlaneCompleteCelebration(PlaneBluePrintSo bluePrintSo)
        {
            LobbyCameraController.Instance.SwitchCameraTarget();

            _currentSocketManager.PlayTakeoffAnimation(ClearTheDataAndScene);
        }

        private void ClearTheDataAndScene()
        {
            if (_currentSocketManager != null)
            {
                Destroy(_currentSocketManager.gameObject);
            }

            //?Clear memory
            _cachedBluePrint = null;
            _cachedSavedParts = null;
            _isDataLoaded = false;
            _isPlaneSpawned = false;
            _currentSocketManager = null;
        }

        private void HandlePlanePartLoaded(PlaneBluePrintSo bluePrintSo, List<SavedPartData> savedPartData)
        {
            _cachedBluePrint = bluePrintSo;
            _cachedSavedParts = savedPartData;
            _isDataLoaded = true;

            //? if its new plane first spawn skeleton (sockets) then Init visuals of plane
            if (_currentSocketManager == null && bluePrintSo.planeSkeletonPrefab != null)
            {
                GameObject skeleton = Instantiate(bluePrintSo.planeSkeletonPrefab, Vector3.zero + skeletonSpawnOffset,
                    Quaternion.identity);
                LobbyCameraController.Instance.SwitchCameraTarget(skeleton.transform);

                Debug.Log("Plane skeleton spawned for plane: " + bluePrintSo.planeName);
            }
            else
            {
                TryInitializeVisuals();
            }
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
            foreach (var savedPart in savedPartData)
            {
                if (savedPart.partSo.modificationType == ModificationType.Install)
                {
                    if (_currentSocketManager.sockets.TryGetValue(savedPart.partSo.planePartType,
                            out PlaneSocket targetSocket))
                    {
                        if (targetSocket != null)
                            SpawnPart(targetSocket, savedPart);
                    }
                }
            }

            foreach (var savedPart in savedPartData)
            {
                if (savedPart.partSo.modificationType == ModificationType.Paint)
                {
                    if (_currentSocketManager.sockets.TryGetValue(savedPart.partSo.planePartType,
                            out PlaneSocket targetSocket))
                    {
                        ApplySavedPaint(targetSocket, savedPart);
                    }
                }
            }
        }

        private void ApplySavedPaint(PlaneSocket targetSocket, SavedPartData savedData)
        {
            Renderer targetRenderer = targetSocket.GetComponentInChildren<Renderer>();
            if (targetRenderer == null) return;

            var partId = savedData.saveInfo.selectedVariationID;
            Texture2D textureToApply = savedData.partSo.defaultPaintTexture;

            if (savedData.partSo.hasVariation && partId != "NONE")
            {
                var matchedVariation = savedData.partSo.variations.FirstOrDefault(v => v.variationID == partId);
                if (matchedVariation.variationID != null)
                {
                    textureToApply = matchedVariation.paintTexture;
                }
            }

            if (textureToApply != null)
            {
                targetRenderer.sharedMaterial.SetTexture(_baseMapID, textureToApply);
            }
        }

        private void SpawnPart(PlaneSocket targetSocket, SavedPartData savedData)
        {
            var partId = savedData.saveInfo.selectedVariationID;

            GameObject part = savedData.partSo.GetPrefabToSpawn(partId);

            if (part == null) return;

            //? Plane skeleton xray disable.
            targetSocket.ToggleMeshRenderer(false);

            GameObject partToInstall = Instantiate(part, targetSocket.transform.position,
                Quaternion.identity);

            partToInstall.transform.SetParent(targetSocket.transform);
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
            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket)) return;

            Renderer targetRenderer = targetSocket.GetComponentInChildren<Renderer>();
            if (targetRenderer == null) return;

            Material originalMaterial = targetRenderer.sharedMaterial;

            PlaneBuildPreviewController.Instance.RevertPreview(partSo);

            Texture currentTexture = originalMaterial.GetTexture(_baseMapID);
            if (currentTexture == null) currentTexture = Texture2D.whiteTexture;

            Texture2D newTexture = partVariation != null && partVariation.Value.paintTexture != null
                ? partVariation.Value.paintTexture
                : partSo.defaultPaintTexture;

            if (partVariation != null)
            {
                Material effectMat = new Material(partVariation.Value.paintMaterial); //paintEffectMaterialTemplate

                _animSequence?.Kill();
                _animSequence = DOTween.Sequence();

                effectMat.SetTexture(_mainPaintID, currentTexture);
                effectMat.SetTexture(_newTextureID, newTexture);
                effectMat.SetFloat(_paintSpeedId, -2f);

                targetRenderer.material = effectMat;

                _animSequence.Append(
                    effectMat.DOFloat(1.5f, _paintSpeedId, paintDuration).SetEase(Ease.InOutSine)
                ).OnComplete(() =>
                {
                    originalMaterial.SetTexture(_baseMapID, newTexture);
                    targetRenderer.material = originalMaterial;
                    Destroy(effectMat);

                    if (_isWaitingForCelebration)
                    {
                        _isWaitingForCelebration = false;
                        StartPlaneCompleteCelebration(_cachedBluePrint);
                    }

                    LobbyEvents.TriggerPlanePartBuildAnimEnded(partSo, partVariation);
                });
            }
        }

        private void ProcessInstall(PlanePartSo partSo, PlanePartVariation? partVariation)
        {
            //TODO Create CineMachine camera and Call Camera Movement from CameraController.

            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket))
                return;

            PlaneBuildPreviewController.Instance.RevertPreview(partSo);

            GameObject finalPart = partVariation != null ? partVariation.Value.partPrefab : partSo.defaultPartPrefab;

            GameObject partToInstall = Instantiate(finalPart, partSpawnPosition,
                Quaternion.identity);

            _animSequence?.Kill();

            _animSequence = DOTween.Sequence().SetLink(partToInstall);
            _animSequence.AppendInterval(intervalDuration);

            _animSequence.Append(partToInstall.transform.DOMove(targetSocket.transform.position, partInstallDuration)
                .SetEase(installEase));
            _animSequence.Join(partToInstall.transform.DORotate(targetSocket.transform.rotation.eulerAngles,
                partInstallDuration));

            _animSequence.OnComplete(() =>
            {
                partToInstall.transform.SetParent(targetSocket.transform);
                LobbyEvents.TriggerPlanePartBuildAnimEnded(partSo, partVariation);

                if (_isWaitingForCelebration)
                {
                    _isWaitingForCelebration = false;
                    StartPlaneCompleteCelebration(_cachedBluePrint);
                }
            });
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlanePartBuildAnimStarted -= HandlePlanePartBuildAnimStarted;
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
            LobbyEvents.OnPlanePartLoaded -= HandlePlanePartLoaded;
            LobbyEvents.OnPlaneBuildCompleted -= HandlePlaneBuildCompleted;
        }
    }
}