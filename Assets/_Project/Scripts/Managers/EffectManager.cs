using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using _Project.Scripts.UI.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Managers
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [Space(10)] [Header("Pool Settings")] [SerializeField]
        private EffectEmitter effectEmitterPrefab;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private Transform poolContainer;

        [Header("Default Effects Settings")] [SerializeField]
        private float defaultScale = 0.25f;

        private IObjectPool<EffectEmitter> effectEmitterPool;

        [Header("Effects Database")] [Tooltip("Add Al Effect Prefabs Here")] [SerializeField]
        private List<EffectPoolSetup> effectSetupList = new List<EffectPoolSetup>();

        private Dictionary<EffectType, IObjectPool<EffectEmitter>> effectPools =
            new Dictionary<EffectType, IObjectPool<EffectEmitter>>();

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;

            InitializePool();
        }

        private void OnEnable()
        {
        }

        private void InitializePool()
        {
            if (poolContainer == null) poolContainer = new GameObject("EffectEmitter_Pool").transform;
            poolContainer.SetParent(this.transform);

            foreach (var setup in effectSetupList)
            {
                var pool = new ObjectPool<EffectEmitter>(
                    () => CreateEffectEmitter(setup.EmitterPrefab),
                    OnTakeFromPool,
                    OnReturnedToPool,
                    OnDestroyPoolObject,
                    true,
                    defaultCapacity,
                    maxPoolSize
                );
                effectPools.Add(setup.EffectType, pool);
            }
        }

        private EffectEmitter CreateEffectEmitter(EffectEmitter prefab)
        {
            EffectEmitter emitter = Instantiate(prefab, poolContainer);
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


        public void PlayEffect(EffectType type, Vector3 position, Transform parent = null, float? customScale = null)
        {
            if (!effectPools.TryGetValue(type, out var pool)) return;

            float finalScale = customScale ?? defaultScale;
            
            EffectData data = new EffectData(
                position: position,
                rotation: Quaternion.identity,
                scale: Vector3.one * finalScale,
                parentTransform: parent);

            EffectEmitter emitter = pool.Get();
            emitter.Initialize(data, pool);
            emitter.Play();
        }

        private void OnDisable()
        {
        }
    }
}