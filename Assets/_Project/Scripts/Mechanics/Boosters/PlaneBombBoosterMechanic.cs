using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Mechanics.Boosters
{
    public class PlaneBombBoosterMechanic : MonoBehaviour
    {
        public static PlaneBombBoosterMechanic Instance { get; private set; }

        [Header("Plane References")] [SerializeField]
        private GameObject planePrefab;

        [SerializeField] private GameObject bombPrefab;

        [Header("Plane Animation Settings")] [Tooltip("Plane starts to fly this position")] [SerializeField]
        private Transform planeFlyStartPos;

        [Tooltip("Plane fly end position")] [SerializeField]
        private Transform planeFlyEndPos;

        [Tooltip("total Flight duration")] [SerializeField]
        private float flightDuration;

        [Tooltip("Plane Rotate anim duration")] [SerializeField]
        private float planeRotateDuration;

        [Header("Bomb drop flow")] [Tooltip("Bomb Spawn position offset")] [SerializeField]
        private Vector3 bombSpawnOffset = new Vector3(0f, 0f, 0f);

        [Tooltip("Bomb Drop Duration")] [SerializeField]
        private float bombDropDuration;

        [Tooltip("Drop start time multiplier for set time for Bomb drop")] [SerializeField]
        private float dropStartTimeMultiplier;

        [Tooltip("Camera Shake duration")] [SerializeField]
        private float cameraShakeDuration = 0.3f;

        [Tooltip("Camera shake strength")] [SerializeField]
        private float cameraShakeStrength = 0.5f;

        [Tooltip("Bomb explosion anim scale")] [SerializeField]
        private float bombExplosionScale = 0.7f;

        [Tooltip("Item explosion anim scale")] [SerializeField]
        private float itemExplosionScale = 0.7f;

        [Tooltip("Force radius")] [SerializeField]
        private float forceRadius;

        [Tooltip("Explosion Force")] [SerializeField]
        private float explosionForce;

        [Tooltip("Explosion upwards modifier")] [SerializeField]
        private float upwardsModifier = 2f;


        private Camera _mainCamera;

        private readonly Collider[] _explosionCollidersBuffer = new Collider[50];

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

        public void PlayPlaneBombBoost(Vector3 pos)
        {
            List<Item> targets = ItemSpotsManager.Instance.GetRandomIdenticalItemsFromPool(3);

            GameEvents.TriggerBoosterAnimationStarted(ResourceType.PlaneBombBooster);

            GameObject plane = Instantiate(planePrefab, planeFlyStartPos.position, Quaternion.identity);
            plane.transform.localScale = Vector3.one * 0.1f;

            Sequence seq = DOTween.Sequence().SetLink(plane.gameObject);

            if (planePrefab != null && planeFlyStartPos != null && planeFlyEndPos != null)
            {
                //? Rotate Plane
                Vector3 targetPos = planeFlyEndPos.transform.position;
                Vector3 directionToTarget = (targetPos - plane.transform.position).normalized;
                directionToTarget.y = 0;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 targetEuler = lookRotation.eulerAngles;

                seq.Append(plane.transform.DORotate(targetEuler, planeRotateDuration, RotateMode.Fast)
                    .SetEase(Ease.OutQuad));

                //? Move Plane
                SoundEmitter planeEngineSound = null;
                Tween flightTween = seq.Append(plane.transform.DOMove(planeFlyEndPos.position, flightDuration)
                    .SetEase(Ease.Linear)
                    .OnStart(() =>
                    {
                        planeEngineSound = SoundManager.Instance.PlaySoundByType(SoundType.PlaneEngine,
                            _mainCamera.transform.position);
                    })
                    .OnComplete(() =>
                    {
                        if (planeEngineSound != null)
                        {
                            planeEngineSound.Stop();
                        }
                    }));

                //? Plane Bomb drop transactions
                float dropStartTime = ((flightDuration / 2) * dropStartTimeMultiplier);
                seq.InsertCallback(dropStartTime, () => { CreateBombDropSequence(targets, plane.transform); });

                seq.OnComplete(() => { Destroy(plane.gameObject); });
            }
        }

        private void CreateBombDropSequence(List<Item> targets, Transform planeTransform)
        {
            Vector3 planesPos = new Vector3(planeTransform.position.x, planeTransform.position.y,
                planeTransform.position.z);

            Vector3 bombSpawnPos = planesPos + bombSpawnOffset;
            GameObject bomb = Instantiate(bombPrefab, bombSpawnPos, Quaternion.identity);
            bomb.transform.Rotate(181, -38, -44f);

            Vector3 groundTargetPos = new Vector3(0, 0f, 0);

            Sequence bombSeq = DOTween.Sequence().SetLink(bomb.gameObject);

            SoundEmitter bombWhistleEmitter = null;
            bombSeq.Append(bomb.transform.DOMove(groundTargetPos, bombDropDuration).SetEase(Ease.InQuad) //InCubic
                .OnStart(() =>
                {
                    bombWhistleEmitter =
                        SoundManager.Instance.PlaySoundByType(SoundType.BombWhistle, _mainCamera.transform.position);
                }).OnComplete(() => { bombWhistleEmitter?.Stop(); })); 
            bombSeq.AppendCallback(() =>
            {
                //? Bomb Explosion flow
                SoundManager.Instance.PlaySoundByType(SoundType.BombExplode, _mainCamera.transform.position);
                EffectManager.Instance.PlayEffect(EffectType.BombExplode, bomb.transform.position, null,
                    bombExplosionScale);

                _mainCamera.transform.DOShakePosition(cameraShakeDuration, cameraShakeStrength, 10);

                foreach (var target in targets)
                {
                    if (target != null && target.gameObject != null)
                    {
                        EffectManager.Instance.PlayEffect(EffectType.ItemExplode, target.transform.position, null,
                            itemExplosionScale);
                        SoundManager.Instance.PlaySoundByType(SoundType.ItemExplodeWBomb,
                            _mainCamera.transform.position);

                        ItemSpotsManager.Instance.DestroySingleItemFromBoard(target);
                        GameEvents.TriggerBoosterUseRequested(ResourceType.PlaneBombBooster, target);
                    }
                }

                ApplyBombForceToObjects(groundTargetPos);

                GameEvents.TriggerBoosterAnimationEnded(ResourceType.PlaneBombBooster);

                Destroy(bomb);
            });
        }

        private void ApplyBombForceToObjects(Vector3 explosionPos)
        {
            int objectCount = Physics.OverlapSphereNonAlloc(explosionPos, forceRadius, _explosionCollidersBuffer);

            for (var i = 0; i < objectCount; i++)
            {
                Collider col = _explosionCollidersBuffer[i];
                if (!col) continue;

                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (!rb) continue;

                rb.AddExplosionForce(explosionForce, explosionPos, forceRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 explosionRadiusPos = new Vector3(0, 0, 0) + Vector3.one * forceRadius;
            Gizmos.DrawWireSphere(explosionRadiusPos, forceRadius);
        }
#endif
    }
}