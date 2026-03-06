using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Managers;
using _Project.Scripts.Structs;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace _Project.Scripts.UI.Components
{
    public class GoalCardUI : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TextMeshProUGUI goalRemainingText;

        [SerializeField] private Image goalImage;
        [SerializeField] private Transform checkMarkIcon;
        [SerializeField] private AudioClip goalAchievedClip;
        [SerializeField] private AudioClip cardWhooshClip;
        [SerializeField] private ParticleSystem goalAchievedParticles;

        [Header("Layout Settings")] [SerializeField]
        private LayoutElement layoutElement;

        [Space(10)] [Header("Animation Settings")] [SerializeField]
        private float animationDuration = 0.3f;

        [Space(10)] [SerializeField] private float cardPunchScale = 0.5f;
        [SerializeField] private float punchDuration = 0.3f;

        [Space(10)] [SerializeField] private float shakeDuration = 2f;
        [SerializeField] private Vector3 cardShakeScale = new Vector3(0, 0, 15f);

        [Space(10)] [SerializeField] private float scaleDownDuration;
        [SerializeField] private float scaleUpDuration = 0.2f;
        [SerializeField] private float scaleUpScale = 1.2f;
        private Camera _camera;


        public static event Action<Vector3, EffectType, Transform> OnCardVisualUpdated;

        private void Awake()
        {
            _camera = Camera.main;
        }

        public void Setup(int initialAmount, Sprite icon = null)
        {
            checkMarkIcon.gameObject.SetActive(false);

            goalRemainingText.enabled = true;
            goalRemainingText.text = initialAmount.ToString();

            if (icon != null) goalImage.sprite = icon;
        }

        public void UpdateAmount(int currentAmount)
        {
            Sequence seq = DOTween.Sequence().SetLink(this.gameObject);

            goalRemainingText.text = currentAmount.ToString();

            seq.Join(
                this.gameObject.transform.DOPunchScale(Vector3.one * cardPunchScale, punchDuration,
                    vibrato: 1,
                    elasticity: 1f)
            );

            OnCardVisualUpdated?.Invoke(goalRemainingText.transform.position, EffectType.UIDecreaseSparks,
                transform.parent.parent);

            //? Goal Archived Phase Animations
            if (currentAmount <= 0)
            {
                goalRemainingText.enabled = false;
                checkMarkIcon.gameObject.SetActive(true);

                //? Play Sprinkles Particles 
                ParticleSystem spawnedParticle = Instantiate(goalAchievedParticles, transform.position,
                    Quaternion.identity,
                    transform.parent.parent);
                spawnedParticle.transform.position = transform.position;
                spawnedParticle.transform.SetSiblingIndex(transform.parent.GetSiblingIndex());

                //? Play goal achived audio clip
                SoundData soundData = new SoundData(
                    clip: goalAchievedClip,
                    position: _camera.transform.position,
                    volume: 1,
                    pitch: 1,
                    isFrequent: true
                );
                SoundManager.Instance.PlaySound(soundData);

                //? Shake Anim and scale up together
                seq.Append(transform.DOScale(Vector3.one * scaleUpScale, scaleUpDuration));
                seq.Join(transform.DOShakeRotation(shakeDuration, cardShakeScale, vibrato: 10));

                seq.Append(transform.DORotate(new Vector3(0, 540, 0), animationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.InOutSine));

                //?  append scale down
                seq.Append(transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(Ease.InBack));

                if (layoutElement != null)
                {
                    seq.Join(DOTween.To(
                        () => layoutElement.preferredWidth,
                        x => layoutElement.preferredWidth = x,
                        0f,
                        scaleDownDuration
                    ).SetEase(Ease.InBack));
                }

                //? play wind/whoosh sound effect while in half scaledown anim

                float exactTime = seq.Duration() - (scaleDownDuration / 2f);
                seq.InsertCallback(exactTime, () =>
                {
                    SoundData soundData = new SoundData(
                        clip: cardWhooshClip,
                        position: _camera.transform.position,
                        volume: 1,
                        pitch: 1,
                        isFrequent: true
                    );
                    SoundManager.Instance.PlaySound(soundData);
                });

                seq.OnComplete(() => { Destroy(this.gameObject); });
            }
        }
    }
}