using System;
using System.Collections.Generic;
using _Project.Scripts.Enums;
using _Project.Scripts.ItemScripts;
using _Project.Scripts.Managers;
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

        [SerializeField] private float planeRotateDuration;

        private Camera _mainCamera;

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
                Tween flightTween = seq.Append(plane.transform.DOMove(planeFlyEndPos.position, flightDuration)
                    .SetEase(Ease.Linear));

                seq.InsertCallback(0f,
                    () =>
                    {
                        SoundManager.Instance.PlaySoundByType(SoundType.PlaneSwoosh, _mainCamera.transform.position);
                    });

                //? Plane Bomb drop transactions
                float dropStartTime = (flightDuration / 2f);
                seq.InsertCallback(dropStartTime, () => { CreateBombDropSequence(targets, plane.transform); });
            }
        }

        private void CreateBombDropSequence(List<Item> targets, Transform planeTransform)
        {
            Vector3 planesPos = new Vector3(planeTransform.position.x, planeTransform.position.y,
                planeTransform.position.z);

            GameObject bomb = Instantiate(bombPrefab, planesPos, Quaternion.identity);
        }
    }
}