using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class FreezeScreenEffectController : MonoBehaviour
    {
        [Header("Shader References")] [Tooltip("Material that uses the freeze screen shader")] [SerializeField]
        private Material freezeScreenMaterial;

        [Tooltip("Shader Graph reference name")] [SerializeField]
        private string intensityPropertyName = "_VignetteIntensity";

        [Header("Animation Settings")] [SerializeField]
        private float targetIntensity = 1f;

        [SerializeField] private float fadeDuration = 0.5f;

        private void OnEnable()
        {
            ResetMaterial();

            TimeManager.OnTimeFreezeStarted += HandleTimeFreezeStarted;
            TimeManager.OnTimeFreezeEnded += HandleTimeFreezeEnded;
        }

        private void HandleTimeFreezeEnded()
        {
            if (freezeScreenMaterial == null) return;
            freezeScreenMaterial.DOFloat(0f, intensityPropertyName, fadeDuration).SetEase(Ease.InSine);
        }

        private void HandleTimeFreezeStarted(float obj)
        {
            if (freezeScreenMaterial == null) return;
            freezeScreenMaterial.DOFloat(targetIntensity, intensityPropertyName, fadeDuration).SetEase(Ease.OutSine);
        }

        private void ResetMaterial()
        {
            if (freezeScreenMaterial != null)
            {
                freezeScreenMaterial.SetFloat(intensityPropertyName, 0f);
            }
        }

        private void OnDisable()
        {
            TimeManager.OnTimeFreezeStarted -= HandleTimeFreezeStarted;
            TimeManager.OnTimeFreezeEnded -= HandleTimeFreezeEnded;

            ResetMaterial();
        }
    }
}