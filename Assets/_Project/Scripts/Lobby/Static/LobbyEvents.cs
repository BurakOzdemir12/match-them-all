using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;

namespace _Project.Scripts.Lobby.Static
{
    public struct LobbyEvents
    {
        public static event Action<PlaneBluePrintSo, PlanePartSo, PartSaveInfo> OnPlanePartLoaded;
        public static event Action<PlanePartSo, PlanePartVariation?> OnPlanePartBuildStarted;
        public static event Action<List<PlanePartSo>> OnAvailablePartsUpdated;
        public static event Action<PlaneBluePrintSo> OnPlaneBuildCompleted;

        public static void TriggerPlanePartLoaded(PlaneBluePrintSo planeBluePrintSo, PlanePartSo part,
            PartSaveInfo saveInfo)
        {
            OnPlanePartLoaded?.Invoke(planeBluePrintSo, part, saveInfo);
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
    }
}