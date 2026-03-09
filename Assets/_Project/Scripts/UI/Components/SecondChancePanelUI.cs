using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Components
{
    public class SecondChancePanelUI : MonoBehaviour
    {
        [Header("Carousel Slides (RectTransforms)")] [SerializeField]
        private RectTransform step1Slide;

        [SerializeField] private RectTransform step2Slide;

        [Header("Text Updates")] [SerializeField]
        private TextMeshProUGUI warningTitleText;

        [Header("Animation Settings")] [SerializeField]
        private float slideDuration = 0.4f;

        [SerializeField] private float slideOffset = 800f;

        public event Action<FailType> OnFullyDismissed;

        private int _currentStep = 1;
        private FailType _currentFailType;

        public void Setup(FailType reason)
        {
            _currentStep = 1;
            _currentFailType = reason;

            warningTitleText.gameObject.SetActive(false);

            step1Slide.DOKill();
            step2Slide.DOKill();

            step1Slide.anchoredPosition = Vector2.zero;
            step2Slide.anchoredPosition = new Vector2(slideOffset, 0);

            this.gameObject.SetActive(true);
        }

        public void OnCloseButtonTapped()
        {
            if (_currentStep == 1)
            {
                _currentStep = 2;

                Sequence sequence = DOTween.Sequence();

                sequence.Join(
                    step1Slide.DOAnchorPosX(-slideOffset, slideDuration).SetEase(Ease.InBack)
                ).SetUpdate(true);

                sequence.Append(
                    step2Slide.DOAnchorPosX(0, slideDuration).SetEase(Ease.OutBack)
                ).SetUpdate(true);

                warningTitleText.gameObject.SetActive(true);
            }
            else if (_currentStep == 2)
            {
                this.gameObject.SetActive(false);
                OnFullyDismissed?.Invoke(_currentFailType);
            }
        }

        public void OnReviveButtonTapped()
        {
            GameEvents.TriggerReviveRequested(_currentFailType);
            this.gameObject.SetActive(false);
        }
    }
}