using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Managers
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [Header("References")] [SerializeField]
        private ParticleSystem particleSystem;

        [Space(10)] [Header("Pool Settings")] [SerializeField]
        private EffectEmitter effectEmitterPrefab;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private Transform poolContainer;

        [Header("Default Effects Settings")] [SerializeField]
        private Vector3 defaultScale = Vector3.one;

        private IObjectPool<EffectEmitter> effectEmitterPool;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;

            InitializePool();
        }

        private void OnEnable()
        {
            GameEvents.OnMergeCompleted += HandleMergeCompleted;
        }

        private void InitializePool()
        {
            if (poolContainer == null) poolContainer = new GameObject("EffectEmitter_Pool").transform;
            poolContainer.SetParent(this.transform);

            effectEmitterPool = new ObjectPool<EffectEmitter>(
                CreateEffectEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                defaultCapacity,
                maxPoolSize
            );
        }

        private EffectEmitter CreateEffectEmitter()
        {
            EffectEmitter emitter = Instantiate(effectEmitterPrefab, poolContainer);
            emitter.gameObject.SetActive(false);
            return emitter;
        }

        private void OnTakeFromPool(EffectEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(EffectEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(EffectEmitter emitter)
        {
            if (emitter != null && emitter.gameObject != null)
            {
                Destroy(emitter.gameObject);
            }
        }

        private void HandleMergeCompleted(Vector3 position, ItemType itemType)
        {
            EffectData mergeEffectData = new EffectData(
                position: position,
                rotation: Quaternion.identity,
                scale: defaultScale);

            PlayEffect(mergeEffectData);
        }

        private void PlayEffect(EffectData data)
        {
            EffectEmitter emitter = effectEmitterPool.Get();
            emitter.Initialize(data, effectEmitterPool);
            emitter.Play();
        }

        private void OnDisable()
        {
            GameEvents.OnMergeCompleted -= HandleMergeCompleted;
        }
    }
}