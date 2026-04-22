using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.ScriptableObjects.Plane
{
    [CreateAssetMenu(fileName = "NewPlanePart", menuName = "ScriptableObjects/Lobby/Plane/Plane Part")]
    public class PlanePartSo : ScriptableObject
    {
        [field: Header("Plane Part Data with variations")]
        [field: Tooltip("The type of plane part this SO represents"), SerializeField]
        public PlanePartType planePartType { get; private set; }

        [field: Tooltip("How many wrench requires to build this part"), SerializeField]
        public int requiredWrench { get; private set; }

        [field: Tooltip("Short description for what it'll build"), SerializeField]
        public string descriptionText { get; private set; }

        [field: Tooltip("Plane part icon"), SerializeField]
        public Sprite partIcon { get; private set; }

        [field: Tooltip("Plane prefab-> default part"), SerializeField]
        public GameObject defaultPartPrefab { get; set; }

        [field: Tooltip("Modification type offers the seperate animations for type of process"), SerializeField]
        public ModificationType modificationType { get; private set; }

        [field: Space(10)]
        [field: Header("Variations"), SerializeField]
        public List<PlanePartVariation> variations { get; set; }

        [field: Tooltip("Is there a variation for this part?"), SerializeField]
        public bool hasVariation => variations != null && variations.Count > 0;

        public GameObject GetPrefabToSpawn(string savedVariationID)
        {
            if (!hasVariation || string.IsNullOrEmpty(savedVariationID) || savedVariationID == "NONE")
            {
                return defaultPartPrefab;
            }

            PlanePartVariation matchedVariation = variations.FirstOrDefault(v => v.variationID == savedVariationID);

            if (matchedVariation.variationID != null)
            {
                return matchedVariation.partPrefab;
            }

            return defaultPartPrefab;
        }
    }
}