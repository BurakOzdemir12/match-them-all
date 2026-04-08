using System.Collections.Generic;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Structs;
using UnityEngine;

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

        [field: Space(10)]
        [field: Header("Variations"), SerializeField]
        public List<PlanePartVariation> variations { get; private set; }
    }
}