using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.Interfaces;
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

        [Header("Audio Clips")] [Tooltip("Items Merge smash audio clip")] [SerializeField]
        private AudioClip mergeSmashClip;

        [Tooltip("Goal Decrease Audio Clip")] [SerializeField]
        private AudioClip goalDecreaseClip;

        [Tooltip("Audio for when item clicked")] [SerializeField]
        private AudioClip itemSelectClip;

        [Tooltip("Audio clip for item collected")] [SerializeField]
        private AudioClip itemCollectedClip;

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
            MergeManager.OnMergeCompleted += HandleMergeCompleted;
            GoalCardUI.OnCardVisualUpdated += HandleGoalCardUpdated;
            InputManager.OnItemSelected += HandleItemSelected;
            ItemSpotsManager.ItemCollected += HandleItemCollected;
        }


        private void HandleItemCollected(ItemType type)
        {
            SoundData itemCollectedSoundData = new SoundData(
                clip: itemCollectedClip,
                position: _camera.transform.position,
                volume: 0.3f,
                pitch: Random.Range(1f, 1f),
                isFrequent: true
            );
            PlaySound(itemCollectedSoundData);
        }

        private void HandleItemSelected(IInteractable interactable)
        {
            SoundData itemSelectedSoundData = new SoundData(
                clip: itemSelectClip,
                position: _camera.transform.position,
                volume: 1,
                pitch: Random.Range(1f, 1f),
                isFrequent: true
            );
            PlaySound(itemSelectedSoundData);
        }

        private void HandleGoalCardUpdated(Vector3 pos, EffectType type, Transform uiParent)
        {
            SoundData mergeSoundData = new SoundData(
                clip: goalDecreaseClip,
                position: _camera.transform.position,
                volume: 0.1f,
                pitch: Random.Range(1f, 1f),
                isFrequent: true
            );
            PlaySound(mergeSoundData);
        }

        private void HandleMergeCompleted(Vector3 pos, ItemType itemType, EffectType effectType)
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
        public void PlaySound(SoundData data)
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
            MergeManager.OnMergeCompleted -= HandleMergeCompleted;
            GoalCardUI.OnCardVisualUpdated -= HandleGoalCardUpdated;
            InputManager.OnItemSelected -= HandleItemSelected;
            ItemSpotsManager.ItemCollected -= HandleItemCollected;
        }
    }
}