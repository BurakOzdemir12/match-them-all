using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;

namespace _Project.Scripts.Lobby.Static
{
    public struct LobbyEvents
    {
        public static event Action<PlaneBluePrintSo, List<SavedPartData>> OnPlanePartLoaded;
        public static event Action<PlanePartSo, PlanePartVariation?> OnPlanePartPurchaseConfirmed;
        public static event Action<PlanePartSo, PlanePartVariation?> OnPlanePartBuildAnimStarted;
        public static event Action<PlanePartSo, PlanePartVariation?> OnPlanePartBuildAnimEnded;
        public static event Action<List<PlanePartSo>> OnAvailablePartsUpdated;
        public static event Action<PlaneBluePrintSo> OnPlaneBuildCompleted;
        public static event Action<PlaneSocketManager> OnPlaneSpawned;
        public static event Action<float> OnPlaneBuildProgressChanged;

        //UI flow
        public static event Action<ResourceType> OnResourceNotEnough;

        public static void TriggerPlanePartLoaded(PlaneBluePrintSo planeBluePrintSo, List<SavedPartData> savedPartData)
        {
            OnPlanePartLoaded?.Invoke(planeBluePrintSo, savedPartData);
        }

        public static void TriggerPlanePartPurchaseConfirmed(PlanePartSo partSo,
            PlanePartVariation? selectedVariation = null)
        {
            OnPlanePartPurchaseConfirmed?.Invoke(partSo, selectedVariation);
        }

        public static void TriggerPlanePartBuildAnimStarted(PlanePartSo partSo,
            PlanePartVariation? selectedVariation = null)
        {
            OnPlanePartBuildAnimStarted?.Invoke(partSo, selectedVariation);
        }

        public static void TriggerPlanePartBuildAnimEnded(PlanePartSo partSo,
            PlanePartVariation? selectedVariation = null)
        {
            OnPlanePartBuildAnimEnded?.Invoke(partSo, selectedVariation);
        }

        public static void TriggerAvailablePartsUpdated(List<PlanePartSo> availableParts)
        {
            OnAvailablePartsUpdated?.Invoke(availableParts);
        }

        public static void TriggerAllPlaneBuildCompleted(PlaneBluePrintSo planeBluePrintSo)
        {
            OnPlaneBuildCompleted?.Invoke(planeBluePrintSo);
        }

        public static void TriggerPlaneSpawned(PlaneSocketManager socketManager)
        {
            OnPlaneSpawned?.Invoke(socketManager);
        }

        public static void TriggerPlaneBuildProgressChanged(float currentProgress)
        {
            OnPlaneBuildProgressChanged?.Invoke(currentProgress);
        }

        public static void TriggerResourceNotEnough(ResourceType resourceType)
        {
            OnResourceNotEnough?.Invoke(resourceType);
        }
    }
}