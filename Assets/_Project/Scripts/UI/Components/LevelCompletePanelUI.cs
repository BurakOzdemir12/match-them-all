using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Managers;
using _Project.Scripts.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class LevelCompletePanelUI : MonoBehaviour
    {
        [Header("Phase (Canvas) Groups")] [Tooltip("First phase -> Brand and plane animation")] [SerializeField]
        private CanvasGroup phase1Group;

        [Tooltip("Second phase -> stars and elapsed time shown")] [SerializeField]
        private CanvasGroup phase2Group;

        [Tooltip("Last phase -> Shows how many wrench arend")] [SerializeField]
        private CanvasGroup phase3Group;

        [Header("Phase 1 Group References")] [Space(10)] [Header("Brand Logo Animations")] [SerializeField]
        private RectTransform brandLogo;

        [SerializeField] private float logoScaleMultiplier = 1.4f;
        [SerializeField] private float logoAnimDuration;

        [Header("Plane Animations")] [SerializeField]
        private RectTransform planeObject;

        [SerializeField] private float flightDuration = 2f;
        [SerializeField] private RectTransform flightStartPoint;
        [SerializeField] private RectTransform flightEndPoint;
        [SerializeField] private int spiralCount = 1;
        [SerializeField] private float spiralDuration = 0.5f;

        [Header("Phase 2 group References")] [SerializeField]
        private TextMeshProUGUI elapsedTimeText;

        [SerializeField] private Transform headerText;

        [Space(10)] [SerializeField] private RectTransform[] starObjects = new RectTransform[3];
        [SerializeField] private RectTransform[] emptyStarSlots = new RectTransform[3];

        [SerializeField, Range(0f, 1f)] private float threeStarThreshold = 0.5f;
        [SerializeField, Range(0f, 1f)] private float twoStarThreshold = 0.25f;
        [SerializeField] private float starFlightDuration = 0.5f;
        [SerializeField] private float delayBetweenStars;
        [SerializeField] private RectTransform starAnimStartPoint;

        [SerializeField] private Vector3 punchVector = new Vector3(0.3f, -0.2f, 0f);
        [SerializeField] private float starPunchDuration = 0.3f;
        [SerializeField] private int vibrato = 5;
        [SerializeField] private Vector3 starTargetOffset;

        [Header("Phase 3 group References")] [SerializeField]
        private Image wrenchIcon;

        [SerializeField] private TextMeshProUGUI earnedWrenchText;
        [SerializeField] private int wrenchRewardAmound = 10;

        [Header("Animation Settings")] [SerializeField]
        private float fadeDuration = 0.3f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            transform.DOKill(true);

            Canvas.ForceUpdateCanvases();

            PreparePanels();
            UpdateFinishTimeUI();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (this.gameObject.activeInHierarchy)
                {
                    PlayFirstChoreography();
                }
            });
        }


        private void PreparePanels()
        {
            ResetGroup(phase1Group);
            ResetGroup(phase2Group);
            ResetGroup(phase3Group);

            brandLogo.localScale = Vector3.zero;
            if (planeObject != null)
            {
                planeObject.localScale = Vector3.one;
                if (flightStartPoint != null) planeObject.position = flightStartPoint.position;
            }

            foreach (var star in starObjects)
            {
                if (star == null) continue;
                if (starAnimStartPoint != null)
                {
                    star.position = starAnimStartPoint.position;
                }

                star.localScale = Vector3.zero;
            }

            wrenchIcon.transform.localScale = Vector3.zero;
            earnedWrenchText.transform.localScale = Vector3.zero;
        }

        private void ResetGroup(CanvasGroup canvasGroup)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void PlayFirstChoreography()
        {
            Sequence seq = DOTween.Sequence().SetLink(gameObject);

            seq.Append(phase1Group.DOFade(1f, fadeDuration));
            seq.Append(brandLogo.DOScale(Vector3.one * logoScaleMultiplier, logoAnimDuration).SetEase(Ease.OutBack));

            seq.AppendCallback(() =>
            {
                SoundManager.Instance.PlaySoundByType(SoundType.LevelCompletedMain, _mainCamera.transform.position);
            });

            if (planeObject != null && flightStartPoint != null && flightEndPoint != null)
            {
                Tween flightTween = planeObject.DOMove(flightEndPoint.position, flightDuration).SetEase(Ease.Linear);
                seq.Join(flightTween);

                seq.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.PlaneSwoosh, _mainCamera.transform.position);
                });
                // ? Plane Spiral Effect
                float spiralStartTime = fadeDuration + (flightDuration / 2f) - (spiralDuration / 2f);
                seq.Insert(spiralStartTime, planeObject
                    .DORotate(new Vector3(360 * spiralCount, 0, 0), spiralDuration, RotateMode.FastBeyond360)
                    .SetRelative(true)
                    .SetEase(Ease.InOutSine));
            }

            seq.AppendInterval(2f);
            seq.Append(phase1Group.DOFade(0f, fadeDuration));

            seq.OnComplete(PlaySecondChoreography);
        }

        private void PlaySecondChoreography()
        {
            Sequence seq = DOTween.Sequence().SetLink(gameObject);
            int earnedStars = CalculateStars();

            seq.Append(phase2Group.DOFade(1f, fadeDuration));
            seq.AppendInterval(0.1f);

            for (int i = 0; i < earnedStars; i++)
            {
                int index = i;
                RectTransform flyingStar = starObjects[index];
                RectTransform targetSlot = emptyStarSlots[index];

                seq.Append(flyingStar.DOMove(targetSlot.position + starTargetOffset, starFlightDuration)
                    .SetEase(Ease.OutBack));
                seq.Join(flyingStar.DOScale(Vector3.one, starFlightDuration).SetEase(Ease.OutBack));

                seq.AppendCallback(() =>
                {
                    SoundManager.Instance.PlaySoundByType(SoundType.StarPop, _mainCamera.transform.position);
                });

                seq.Append(flyingStar.DOPunchScale(punchVector, starPunchDuration, vibrato, 1f));
                seq.Join(targetSlot.DOPunchScale(Vector3.one * 0.15f, starPunchDuration, 2, 0.5f));

                seq.AppendInterval(delayBetweenStars);
            }

            seq.AppendInterval(1f);
            seq.Append(phase2Group.DOFade(0f, fadeDuration));

            seq.OnComplete(PlayThirdChoreography);
        }

        private void PlayThirdChoreography()
        {
            Sequence seq = DOTween.Sequence().SetLink(gameObject);
            earnedWrenchText.text = $"{wrenchRewardAmound}";

            seq.Append(phase3Group.DOFade(1f, fadeDuration));

            seq.Append(wrenchIcon.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            seq.Join(earnedWrenchText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f));

            seq.AppendCallback(() =>
            {
                SoundManager.Instance.PlaySoundByType(SoundType.RewardEarned, _mainCamera.transform.position);
            });
            seq.AppendInterval(1f);

            seq.OnComplete(() =>
            {
                phase3Group.interactable = true;
                phase3Group.blocksRaycasts = true;
            });
        }

        private int CalculateStars()
        {
            if (TimeManager.Instance == null) return 0;

            var remainingTime = TimeManager.Instance.RemainingTime;
            var totalLevelTime = TimeManager.Instance.TotalLevelTime;

            if (totalLevelTime <= 0) return 0;

            float percentage = remainingTime / totalLevelTime;

            return percentage >= threeStarThreshold ? 3 : percentage >= twoStarThreshold ? 2 : 1;
        }

        private void UpdateFinishTimeUI()
        {
            if (TimeManager.Instance == null) return;

            int finishTimeSeconds = Mathf.CeilToInt(TimeManager.Instance.RemainingTime);

            int minutes = finishTimeSeconds / 60;
            int seconds = finishTimeSeconds % 60;

            elapsedTimeText.text = $"{minutes:00}:{seconds:00}";
        }

        public void OnContinueButtonClicked()
        {
            // SceneManager.LoadScene("LobbyScene");
        }
    }
}