using System;
using System.Collections;
using _Project.Scripts.Static;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Managers
{
    public class SceneLoaderManager : MonoBehaviour
    {
        public static SceneLoaderManager Instance { get; private set; }

        [Header("UI References")] [Tooltip("Loading Screen Canvas group")] [SerializeField]
        private CanvasGroup loadingScreenCanvasGroup;

        [Tooltip("Loading Bar (Slider)")] [SerializeField]
        private Slider loadingBar;

        [Tooltip("Loading Bar Percentage Text")] [SerializeField]
        private TextMeshProUGUI loadingPercentageText;

        [Tooltip("Color to indicate the slider is active.")] [SerializeField]
        private Image loadingFillImage;

        [Header("Settings")] [SerializeField] private float fadeDuration;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.transform.gameObject);
        }

        private void OnEnable()
        {
            GameEvents.OnSceneLoadRequested += HandleSceneLoadRequested;
        }

        private void Start()
        {
            loadingScreenCanvasGroup.alpha = 0;
            loadingScreenCanvasGroup.interactable = false;
            loadingScreenCanvasGroup.blocksRaycasts = false;
        }

        private void HandleSceneLoadRequested(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            loadingScreenCanvasGroup.blocksRaycasts = true;
            Tween fadeTween = loadingScreenCanvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
            yield return fadeTween.WaitForCompletion();

            if (loadingBar != null) loadingBar.value = 0;
            if (loadingPercentageText != null) loadingPercentageText.text = "%0";

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            if (operation == null) yield break;

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (loadingBar != null) loadingBar.value = progress;
                if (loadingPercentageText != null) loadingPercentageText.text = $"%{Mathf.RoundToInt(progress * 100)}";
                loadingFillImage.color = Color.Lerp(Color.gray, Color.green, progress);

                if (progress >= 0.9f)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            loadingScreenCanvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                loadingScreenCanvasGroup.interactable = false;
                loadingScreenCanvasGroup.blocksRaycasts = false;
            });
        }

        private void OnDisable()
        {
            GameEvents.OnSceneLoadRequested -= HandleSceneLoadRequested;
        }
    }
}