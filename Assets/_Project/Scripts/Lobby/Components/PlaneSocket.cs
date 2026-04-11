using _Project.Scripts.Lobby.Enums;
using UnityEngine;

namespace _Project.Scripts.Lobby.Components
{
    public class PlaneSocket : MonoBehaviour
    {
        [field: Header("Plane part Type"), SerializeField]
        public PlanePartType planePartType { get; private set; }
    }
}