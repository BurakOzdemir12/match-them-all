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
        public Sprite sprite;
        public GameObject partPrefab;
        public Material paintMaterial;
        public Color backgroundColor;
        public Texture2D texture;

        public PlanePartVariation(string variationID, string variationName, Sprite sprite, GameObject partPrefab,
            Material paintMaterial, Color backgroundColor, Texture2D texture)
        {
            this.variationID = variationID;
            this.variationName = variationName;
            this.sprite = sprite;
            this.partPrefab = partPrefab;
            this.paintMaterial = paintMaterial;
            this.backgroundColor = backgroundColor;
            this.texture = texture;
        }
    }
}