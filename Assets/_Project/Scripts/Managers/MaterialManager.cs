using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class MaterialManager : MonoBehaviour
    {
        public static MaterialManager Instance { get; private set; }

        [Header("Materials")] [SerializeField] private Material outlineMaterial;
        public Material OutlineMaterial => outlineMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;
        }
    }
}