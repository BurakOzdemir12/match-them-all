using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Lobby.ScriptableObjects.Plane
{
    [CreateAssetMenu(fileName = "NewPlaneBluePrint", menuName = "ScriptableObjects/Lobby/Plane/Plane Blue Print")]
    public class PlaneBluePrintSo : ScriptableObject
    {
        [field: Header("Plane blue print"), Tooltip("Plane Id"), SerializeField]
        public string planeID { get; private set; }

        [field: Header("Plane blue print"), Tooltip("Plane Name"), SerializeField]
        public string planeName { get; private set; }

        [field: Header("Parts to built plane (Top to Bottom)"),
                Tooltip("Plane will build by sort top to bottom"),
                SerializeField]
        public List<PlanePartSo> partsToBuildInOrder { get; private set; } = new List<PlanePartSo>();
    }
}