using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Lobby.ScriptableObjects.Plane
{
    [CreateAssetMenu(fileName = "PlaneLibrary", menuName = "ScriptableObjects/Lobby/Plane/Plane Library")]
    public class PlaneLibrarySo : ScriptableObject
    {
        [field: Header("All Planes In Game"), SerializeField]
        public List<PlaneBluePrintSo> planeListInGame { get; private set; } = new List<PlaneBluePrintSo>();
    }
}