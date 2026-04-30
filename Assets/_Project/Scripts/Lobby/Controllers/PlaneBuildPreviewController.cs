using System;
using _Project.Scripts.Lobby.Components;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using _Project.Scripts.Lobby.Structs;
using _Project.Scripts.Lobby.UI.Managers;
using UnityEngine;

namespace _Project.Scripts.Lobby.Controllers
{
    public class PlaneBuildPreviewController : MonoBehaviour
    {
        public static PlaneBuildPreviewController Instance { get; private set; }

        private PlaneSocketManager _currentSocketManager;
        public Texture2D originalTextureBeforePreview { get; set; }
        public GameObject currentPreviewPart { get; set; }

        private readonly int _baseMapID = Shader.PropertyToID("_BaseMap");

        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            LobbyEvents.OnPlaneSpawned += HandlePlaneSpawned;
            PlaneBuildUIManager.OnPlaneBuildPreviewRequested += HandlePlaneBuildPreviewRequested;
            LobbyEvents.OnPlaneBuildCompleted += HandlePlaneBuildCompleted;

            // PlaneBuildUIManager.OnPlaneBuildPreviewCanceled += HandlePlaneBuildPreviewCanceled;
        }

        private void HandlePlaneBuildCompleted(PlaneBluePrintSo blueprint)
        {
            ClearTheDataAndScene();
        }


        // private void HandlePlaneBuildPreviewCanceled(PlanePartSo partSo)
        // {
        //     if (_currentPreviewPart != null)
        //     {
        //         Destroy(_currentPreviewPart);
        //         _currentPreviewPart = null;
        //     }
        //
        //     if (_originalTextureBeforePreview != null && _currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket))
        //     {
        //         Renderer targetRenderer = targetSocket.GetComponentInChildren<Renderer>();
        //         if (targetRenderer != null)
        //         {
        //             targetRenderer.sharedMaterial.SetTexture(_baseMapID, _originalTextureBeforePreview);
        //         }
        //         _originalTextureBeforePreview = null;
        //     }
        // }

        private void HandlePlaneBuildPreviewRequested(PlanePartSo partSo, PlanePartVariation partVariation)
        {
            switch (partSo.modificationType)
            {
                case ModificationType.Install:
                    PreviewInstall(partSo, partVariation);
                    break;
                case ModificationType.Paint:
                    PreviewPaint(partSo, partVariation);
                    break;
            }
        }

        private void PreviewPaint(PlanePartSo partSo, PlanePartVariation partVariation)
        {
            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket)) return;

            Renderer targetRenderer = targetSocket.GetComponentInChildren<Renderer>();
            if (targetRenderer == null) return;

            if (originalTextureBeforePreview == null)
            {
                Texture currentTexture = targetRenderer.sharedMaterial.GetTexture(_baseMapID);
                originalTextureBeforePreview =
                    currentTexture != null ? (Texture2D)currentTexture : Texture2D.whiteTexture;
            }

            Texture2D newTexture = partVariation.paintTexture != null
                ? partVariation.paintTexture
                : partSo.defaultPaintTexture;
            targetRenderer.sharedMaterial.SetTexture(_baseMapID, newTexture);
        }

        private void PreviewInstall(PlanePartSo partSo, PlanePartVariation partVariation)
        {
            if (!_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket)) return;

            if (currentPreviewPart != null)
            {
                Destroy(currentPreviewPart);
            }

            GameObject prefabToPreview =
                partVariation.partPrefab != null ? partVariation.partPrefab : partSo.defaultPartPrefab;

            currentPreviewPart = Instantiate(prefabToPreview, targetSocket.transform);
            currentPreviewPart.transform.localPosition = Vector3.zero;
            currentPreviewPart.transform.localRotation = Quaternion.identity;
        }

        private void HandlePlaneSpawned(PlaneSocketManager socketManager)
        {
            _currentSocketManager = socketManager;
        }

        private void ClearTheDataAndScene()
        {
            if (currentPreviewPart != null)
            {
                Destroy(currentPreviewPart);
                currentPreviewPart = null;
            }

            //?Clear memory
            originalTextureBeforePreview = null;
            _currentSocketManager = null;
        }

        private void OnDisable()
        {
            LobbyEvents.OnPlaneSpawned -= HandlePlaneSpawned;
            PlaneBuildUIManager.OnPlaneBuildPreviewRequested -= HandlePlaneBuildPreviewRequested;
            LobbyEvents.OnPlaneBuildCompleted -= HandlePlaneBuildCompleted;

            // PlaneBuildUIManager.OnPlaneBuildPreviewCanceled -= HandlePlaneBuildPreviewCanceled;
        }

        public void RevertPreview(PlanePartSo partSo)
        {
            if (partSo.modificationType == ModificationType.Install)
            {
                if (currentPreviewPart != null)
                {
                    Destroy(currentPreviewPart);
                    currentPreviewPart = null;
                }
            }

            else if (partSo.modificationType == ModificationType.Paint)
            {
                if (originalTextureBeforePreview != null && _currentSocketManager != null)
                {
                    if (_currentSocketManager.sockets.TryGetValue(partSo.planePartType, out PlaneSocket targetSocket))
                    {
                        Renderer targetRenderer = targetSocket.GetComponentInChildren<Renderer>();
                        if (targetRenderer != null)
                        {
                            targetRenderer.sharedMaterial.SetTexture(_baseMapID, originalTextureBeforePreview);
                        }
                    }

                    originalTextureBeforePreview = null;
                }
            }
        }
    }
}