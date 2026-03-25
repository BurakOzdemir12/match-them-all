using System;
using System.Collections.Generic;
using _Project.Scripts.Audio.ScriptableObects;
using _Project.Scripts.Enums;
using _Project.Scripts.Interfaces;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.Static;
using _Project.Scripts.Structs;
using _Project.Scripts.UI.Components;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Data")] [SerializeField]
        private AudioLibrarySo audioLibrary;

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

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            if (Instance != null && Instance != this) Destroy(this.gameObject);
            Instance = this;

            InitializePool();
        }

        private void OnEnable()
        {
            InputManager.OnItemSelected += HandleItemSelected;
            ItemSpotsManager.ItemCollected += HandleItemCollected;
            TimeManager.OnTimeFreezeStarted += HandleTimeFreezeStarted;
        }

        private void HandleTimeFreezeStarted(float obj)
        {
            PlaySoundByType(SoundType.TimeFreezeBooster, _camera.transform.position);
        }

        private void HandleItemCollected(ItemType type)
        {
            PlaySoundByType(SoundType.ItemCollected, _camera.transform.position);
        }

        private void HandleItemSelected(IInteractable interactable)
        {
            PlaySoundByType(SoundType.ItemSelected, _camera.transform.position);
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
        public SoundEmitter PlaySound(SoundData data)
        {
            if (!data.Clip) return null;

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

            return emitter;
        }

        public SoundEmitter PlaySoundByType(SoundType soundType, Vector3 pos)
        {
            SoundEntry entry = audioLibrary.GetSound(soundType);

            if (entry.clip == null) return null;

            SoundData soundData = new SoundData(
                clip: entry.clip,
                position: pos,
                volume: entry.volume,
                pitch: entry.pitch,
                isFrequent: true
            );

            return PlaySound(soundData);
        }


        private void OnDisable()
        {
            InputManager.OnItemSelected -= HandleItemSelected;
            ItemSpotsManager.ItemCollected -= HandleItemCollected;
            TimeManager.OnTimeFreezeStarted -= HandleTimeFreezeStarted;
        }
    }
}