using System.Collections.Generic;
using _Project.Scripts.Lobby.Structs;
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

        [field: Space(10), Header("Skeleton of the plane"),
                Tooltip("This is the what sockets for show exactly plane parts spawn that position"),
                SerializeField]
        public GameObject planeSkeletonPrefab;

        [field: Header("Parts to built plane (Top to Bottom)"),
                Tooltip("Plane will build by sort top to bottom"),
                SerializeField]
        public List<PlaneBuildStage> buildStages { get; private set; } = new List<PlaneBuildStage>();
    }
}