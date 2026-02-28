using System;
using _Project.Scripts.Enums;
using UnityEngine;

namespace _Project.Scripts.Static
{
    public static class GameEvents
    {
        public static Action<Vector3, ItemType> OnMergeCompleted;

        // public static void TriggerMergeCompleted(Vector3 position, ItemType itemType)
        // {
        //     OnMergeCompleted?.Invoke(position, itemType);
        // }
    }
}