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
        public Sprite uiIcon;
        public GameObject partPrefab;
        public Material paintMaterial;

        public PlanePartVariation(string variationID, string variationName, Sprite uiIcon, GameObject partPrefab,
            Material paintMaterial)
        {
            this.variationID = variationID;
            this.variationName = variationName;
            this.uiIcon = uiIcon;
            this.partPrefab = partPrefab;
            this.paintMaterial = paintMaterial;
        }
    }
}