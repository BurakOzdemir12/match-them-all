using UnityEngine;

namespace _Project.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Interact();
        void ReSpawn();
        void Deselect();
        void Select();
    }
}