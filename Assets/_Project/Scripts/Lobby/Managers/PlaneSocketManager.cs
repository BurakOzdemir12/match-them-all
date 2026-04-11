using System;
using System.Collections.Generic;
using _Project.Scripts.Lobby.Components;
using _Project.Scripts.Lobby.Enums;
using _Project.Scripts.Lobby.Static;
using UnityEngine;

namespace _Project.Scripts.Lobby.Managers
{
    public class PlaneSocketManager : MonoBehaviour
    {
        public readonly Dictionary<PlanePartType, Transform> sockets = new Dictionary<PlanePartType, Transform>();


        private void Awake()
        {
            PlaneSocket[] socketComponents = GetComponentsInChildren<PlaneSocket>();
            foreach (PlaneSocket socket in socketComponents)
            {
                if (!sockets.ContainsKey(socket.planePartType))
                {
                    sockets.Add(socket.planePartType, socket.transform);
                }
                else
                {
                    Debug.LogWarning($"Duplicate socket found: {socket.planePartType}");
                }
            }
        }

        private void Start()
        {
            LobbyEvents.TriggerPlaneSpawned(this);
        }
    }
}