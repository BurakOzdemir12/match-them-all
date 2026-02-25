using UnityEngine;

namespace _Project.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Interact();
        void Deselect();
        void Select();
    }
}