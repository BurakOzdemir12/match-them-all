using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.Structs
{
    [System.Serializable]
    public struct PlanePartVariation
    {
        public string variationName;
        public string variationID;
        public Image uiIcon;
        public GameObject partPrefab;

        public PlanePartVariation(string variationID, string variationName, Image uiIcon, GameObject partPrefab)
        {
            this.variationID = variationID;
            this.variationName = variationName;
            this.uiIcon = uiIcon;
            this.partPrefab = partPrefab;
        }
    }
}