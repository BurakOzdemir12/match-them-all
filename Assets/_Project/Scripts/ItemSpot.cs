using UnityEngine;

namespace _Project.Scripts
{
    public class ItemSpot : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Animator animator;

        private GameObject _containedItem;
        public bool IsAvailable => _containedItem == null;

        private readonly int _bumpAnimationHash = Animator.StringToHash("Bump");

        public void Populate(GameObject item)
        {
            _containedItem = item;
        }

        public void Clear()
        {
            _containedItem = null;
        }

        public void AnimateBump()
        {
            animator.CrossFadeInFixedTime(_bumpAnimationHash, 0.001f);
        }
    }
}