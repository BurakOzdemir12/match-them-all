using System.Collections.Generic;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;

namespace _Project.Scripts.Lobby.Data.Save
{
    [System.Serializable]
    public struct PartSaveInfo
    {
        public string partType;
        public string selectedVariationID;
        public string modificationType;

        public PartSaveInfo(string partType, string selectedVariationID, string modificationType)
        {
            this.partType = partType;
            this.selectedVariationID = selectedVariationID;
            this.modificationType = modificationType;
        }

        public string GetUniqueKey()
        {
            return $"{partType}_{modificationType}"; //"Body_Install"
        }
    }

    [System.Serializable]
    public class HangarSaveData
    {
        public string activePlaneID;
        public string activePlaneName;
        public int currentBuildIndex = 0;
        public int currentBuildStageIndex = 0;
        public List<PartSaveInfo> builtParts = new List<PartSaveInfo>();
        public List<string> completedPlaneIDs = new List<string>();

        public HangarSaveData(string activePlaneID, string activePlaneName, int currentBuildIndex,
            int currentBuildStageIndex,
            List<PartSaveInfo> builtParts, List<string> completedPlaneIDs)
        {
            this.activePlaneID = activePlaneID;
            this.activePlaneName = activePlaneName;
            this.currentBuildIndex = currentBuildIndex;
            this.currentBuildStageIndex = currentBuildStageIndex;
            this.builtParts = builtParts;
            this.completedPlaneIDs = completedPlaneIDs;
        }
    }
}