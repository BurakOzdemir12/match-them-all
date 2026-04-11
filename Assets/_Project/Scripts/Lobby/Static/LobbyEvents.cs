using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.Managers;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;

namespace _Project.Scripts.Lobby.Static
{
    public struct LobbyEvents
    {
        public static event Action<PlaneBluePrintSo, List<SavedPartData>> OnPlanePartLoaded;
        public static event Action<PlanePartSo, PlanePartVariation?> OnPlanePartBuildStarted;
        public static event Action<List<PlanePartSo>> OnAvailablePartsUpdated;
        public static event Action<PlaneBluePrintSo> OnPlaneBuildCompleted;
        public static event Action<PlaneSocketManager> OnPlaneSpawned;

        public static void TriggerPlanePartLoaded(PlaneBluePrintSo planeBluePrintSo, List<SavedPartData> savedPartData)
        {
            OnPlanePartLoaded?.Invoke(planeBluePrintSo, savedPartData);
        }

        public static void TriggerPlanePartBuildStarted(PlanePartSo partSo,
            PlanePartVariation? selectedVariation = null)
        {
            OnPlanePartBuildStarted?.Invoke(partSo, selectedVariation);
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
    }
}