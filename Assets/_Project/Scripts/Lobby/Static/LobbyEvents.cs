using System;
using _Project.Scripts.Lobby.Data.Save;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;

namespace _Project.Scripts.Lobby.Static
{
    public struct LobbyEvents
    {
        public static event Action<PlanePartSo, PartSaveInfo> OnPlanePartLoaded;
        public static event Action<PlanePartSo, PlanePartVariation> OnPlanePartBuildStarted;
        public static event Action<PlanePartSo> OnPlanePartBuildTargeted;
        public static event Action<PlaneBluePrintSo> OnPlanePartBuildCompleted;

        public static void TriggerPlanePartLoaded(PlanePartSo part, PartSaveInfo saveInfo)
        {
            OnPlanePartLoaded?.Invoke(part, saveInfo);
        }

        public static void TriggerPlanePartBuildStarted(PlanePartSo partSo, PlanePartVariation selectedVariation)
        {
            OnPlanePartBuildStarted?.Invoke(partSo, selectedVariation);
        }

        public static void TriggerNextPartTargeted(PlanePartSo nextPart)
        {
            OnPlanePartBuildTargeted?.Invoke(nextPart);
        }

        public static void TriggerPlaneBuildCompleted(PlaneBluePrintSo planeBluePrintSo)
        {
            OnPlanePartBuildCompleted?.Invoke(planeBluePrintSo);
        }
    }
}