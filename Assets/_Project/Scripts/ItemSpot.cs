using UnityEngine;

namespace _Project.Scripts
{
    public class ItemSpot : MonoBehaviour
    {
        private GameObject _containedItem;
        public bool IsAvailable => _containedItem == null;

        public void Populate(GameObject item)
        {
            _containedItem = item;
        }

        public void Clear()
        {
            _containedItem = null;
        }
    }
}