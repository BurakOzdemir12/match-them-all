using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("References")] [SerializeField]
        private AudioSource audioSource;

        [Space(10)] [Header("Pool Settings")] [SerializeField]
        private SoundEmitter soundEmitterPrefab;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;

        [Tooltip("Max overlapping frequent sounds (Voice Stealing Limit)")] [SerializeField]
        private int maxSoundInstances = 30;

        [SerializeField] private Transform poolContainer;

        private readonly List<SoundEmitter> activeSoundEmitters = new List<SoundEmitter>();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new LinkedList<SoundEmitter>();

        private IObjectPool<SoundEmitter> soundEmitterPool;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip mergeSmashClip;

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

        private void HandleMergeCompleted(Vector3 pos, ItemType itemType)
        {
            SoundData mergeSoundData = new SoundData(
                clip: mergeSmashClip,
                position: pos,
                volume: 1,
                pitch: Random.Range(0.9f, 1.1f),
                isFrequent: true
            );
            PlaySound(mergeSoundData);
        }

        private void InitializePool()
        {
            if (poolContainer == null) poolContainer = new GameObject("SoundEmitter_Pool").transform;
            poolContainer.SetParent(this.transform);

            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                defaultCapacity,
                maxPoolSize
            );
        }

        private SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab, poolContainer);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void OnTakeFromPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(emitter);
        }

        private void OnReturnedToPool(SoundEmitter emitter)
        {
            if (emitter.PoolNode != null)
            {
                FrequentSoundEmitters.Remove(emitter.PoolNode);
                emitter.PoolNode = null;
            }

            emitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(emitter);
        }

        private void OnDestroyPoolObject(SoundEmitter emitter)
        {
            if (emitter != null && emitter.gameObject != null)
            {
                Destroy(emitter.gameObject);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void PlaySound(SoundData data)
        {
            if (!data.Clip) return;

            if (data.IsFrequent && FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    FrequentSoundEmitters.First.Value.Stop();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            SoundEmitter emitter = soundEmitterPool.Get();
            emitter.Initialize(data, soundEmitterPool);
            emitter.Play();

            if (data.IsFrequent)
            {
                emitter.PoolNode = FrequentSoundEmitters.AddLast(emitter);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnMergeCompleted -= HandleMergeCompleted;
        }
    }
}