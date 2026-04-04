using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.LevelDesign.ScriptableObjects;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Mechanics.Boosters
{
    public class WindBoosterMechanic : MonoBehaviour
    {
        public static WindBoosterMechanic Instance { get; private set; }

        [Header("References")] [Tooltip("The fan prefab that it will spawn")] [SerializeField]
        private GameObject heliPrefab;

        [Header("Spawn and Animation Settings")] [Tooltip("Fan scale in game")] [SerializeField]
        private float heliScale;

        [Tooltip("Heli Spawn Pos")] [SerializeField]
        private GameObject heliSpawnPos;

        [Tooltip("Helicopter stop and hold position")] [SerializeField]
        private GameObject heliHoldPos;

        [Tooltip("Heli End Pos")] [SerializeField]
        private GameObject heliEndPos;

        [Tooltip("Heli move animation duration -> (Speed)")] [SerializeField]
        private float heliAnimDuration;

        [Tooltip("Heli rotate animation duration -> (Rotate Speed)")] [SerializeField]
        private float heliRotateDuration;

        [Space(10)] [Tooltip("Heli move Y position")] [SerializeField]
        private float heliMoveYPos;

        [Tooltip("Heli yoyo Animation duration")] [SerializeField]
        private float heliYoyoDuration;

        [Tooltip("Loops Count")] [SerializeField]
        private int loopsCount = 4;

        [Space(5)] [Header("Object Wind Settings")] [Tooltip("How strong the wind pushes items")] [SerializeField]
        private float windForce;

        [Tooltip("How fast items spin in the ai")] [SerializeField]
        private float windSpinTorque;

        [Tooltip("How much the items are pulled towards the center of the hose")] [SerializeField]
        private float pullMultiplier = 0.3f;

        [Tooltip("How much the items swirl around the center of the hose")] [SerializeField]
        private float swirlMultiplier = 0.8f;

        [Tooltip("Wind Effect Scale")] [SerializeField]
        private float windScale = 3.1f;

        private Camera _mainCamera;

        private bool _isWindBlowing = false;
        private float _currentWindMultiplier = 0f;
        private List<Item> _targets = new List<Item>();

        private bool _isWorking = false;
        private GameObject _activeWind;
        private Sequence _activeSequence;
        private SoundEmitter _heliSoundEmitter = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            if (!_isWindBlowing || _targets == null || _targets.Count == 0) return;
            Vector3 windCenter = new Vector3(0, 0, 0);

            foreach (var item in _targets)
            {
                if (!item || !item.gameObject || !item.Rigidbody) continue;

                //? Lifting Force (to overcome Gravity)
                Vector3 liftForce = Vector3.up * (windForce * _currentWindMultiplier);

                //? Hose-Vorty Force (Circular Rotation)
                Vector3 directionFromCenter = item.transform.position - windCenter;
                directionFromCenter.y = 0;

                //? Cross finds the direction that will rotate the object AROUND the center rather than towards it.
                Vector3 swirlDirection = Vector3.Cross(directionFromCenter.normalized, Vector3.up);
                Vector3 swirlForce = swirlDirection * (windForce * swirlMultiplier * _currentWindMultiplier);

                //?Drawing Force (To prevent objects from moving too far left and right, but to gather in the center of the hose)
                Vector3 pullForce = -directionFromCenter.normalized *
                                    (windForce * pullMultiplier * _currentWindMultiplier);

                //? Applying all forces
                item.Rigidbody.AddForce(liftForce + swirlForce + pullForce, ForceMode.Force);

                //? Add Torque to Objects 
                item.Rigidbody.AddTorque(Random.onUnitSphere * (windSpinTorque * _currentWindMultiplier),
                    ForceMode.Force);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnLevelStarted += HandleLevelStarted;
            GameEvents.OnGameRevived += HandleGameRevived;
        }

        private void HandleLevelStarted(LevelDataSo data) => AbortAndRefund();
        private void HandleGameRevived(FailType type) => AbortAndRefund();

        public void PlayWindBoost(Vector3 pos)
        {
            _isWorking = true;
            GameEvents.TriggerBoosterAnimationStarted(ResourceType.WindBooster);

            _activeWind = Instantiate(heliPrefab, heliSpawnPos.transform.position, Quaternion.identity);
            _activeWind.transform.localScale = Vector3.one * heliScale;

            _activeSequence = DOTween.Sequence().SetLink(_activeWind.gameObject);

            //? Helicopter moves to specified position
            _activeSequence.Append(_activeWind.transform.DOMove(heliHoldPos.transform.position, heliAnimDuration)
                .SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    _heliSoundEmitter = SoundManager.Instance.PlaySoundByType(SoundType.HelicopterEngine,
                        _activeWind.transform.position, followTarget: _activeWind.transform);
                })
            );
            //? X rotation sets 30f while moving.
            _activeSequence.Join(_activeWind.transform
                .DORotate(new Vector3(30f, 90f, 0f), heliRotateDuration, RotateMode.Fast)
                .SetEase(Ease.OutSine)
            );
            //? Just before the movement to hold pos, it rotates back to 0f X rotation.
            float pitchBackStartTime = heliAnimDuration - heliRotateDuration;
            _activeSequence.Insert(pitchBackStartTime,
                _activeWind.transform.DORotate(new Vector3(0f, 90f, 0f), heliRotateDuration, RotateMode.Fast)
                    .SetEase(Ease.InOutSine));

            _activeSequence.AppendCallback(() =>
            {
                //? This is the main effects for items, wind, objects flying aroun
                SoundManager.Instance.PlaySoundByType(SoundType.WindSound, _mainCamera.transform.position);

                EffectEmitter windEffectEmitter = null;
                windEffectEmitter = EffectManager.Instance.PlayEffect(EffectType.HeliPropellerWind,
                    heliHoldPos.transform.position,
                    null,
                    windScale);

                _targets = ItemSpotsManager.Instance.GetAllItemsOnTheBoard();
                if (_targets == null || _targets.Count == 0) return;

                _isWindBlowing = true;
                _currentWindMultiplier = 0f;

                float totalWindApplyDuration = heliYoyoDuration * loopsCount;
                DOVirtual.Float(0f, 1f, totalWindApplyDuration / 2f, v => _currentWindMultiplier = v)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        windEffectEmitter?.Stop();
                        _isWindBlowing = false;
                    });
            });
            //? it's lilke yoyo animation up-down up-down
            //! Remember the formula heliYoyoDuration * loopsCount gives total animation duration
            _activeSequence.Append(_activeWind.transform.DOMoveY(heliMoveYPos, heliYoyoDuration)
                .SetRelative()
                .SetLoops(loopsCount, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
            );

            //? Heli moves out of the screen with rotation.
            _activeSequence.Append(_activeWind.transform.DOMove(heliEndPos.transform.position, heliAnimDuration)
                .SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    _activeWind.transform.DORotate(new Vector3(30f, 90f, 0f), heliRotateDuration,
                        RotateMode.FastBeyond360);
                })
            );

            _activeSequence.OnComplete(() =>
            {
                _isWorking = false;
                _heliSoundEmitter?.Stop();

                Destroy(_activeWind.gameObject);
                _activeWind = null;
                GameEvents.TriggerBoosterAnimationEnded(ResourceType.WindBooster);
            });
        }

        private void AbortAndRefund()
        {
            if (_isWorking)
            {
                _isWorking = false;
                _isWindBlowing = false;

                Debug.Log("Wind Booster amount returned because didnt completed");

                EconomyManager.Instance.AddResource(ResourceType.WindBooster, 1);

                _heliSoundEmitter?.Stop();
                if (_activeSequence != null) _activeSequence.Kill();
                if (_activeWind != null) Destroy(_activeWind);

                GameEvents.TriggerBoosterAnimationEnded(ResourceType.WindBooster);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnLevelStarted -= HandleLevelStarted;
            GameEvents.OnGameRevived -= HandleGameRevived;
        }
    }
}