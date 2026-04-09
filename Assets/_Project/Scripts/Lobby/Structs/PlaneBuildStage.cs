using System.Collections.Generic;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;

namespace _Project.Scripts.Lobby.Structs
{
    [System.Serializable]
    public struct PlaneBuildStage
    {
        public string stageName;

        public List<PlanePartSo> partsInStage;

        public PlaneBuildStage(List<PlanePartSo> partsInStage, string stageName)
        {
            this.partsInStage = partsInStage;
            this.stageName = stageName;
        }
    }
}