using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Components
{
    public class BottomNavItemUI : MonoBehaviour
    {
        [Header("References")] [Tooltip("Layout Element")] [SerializeField]
        private LayoutElement layoutElement;

        public LayoutElement LayoutElement => layoutElement;

        [Tooltip("Icon rect transform")] [SerializeField]
        private RectTransform iconRect;

        [Tooltip("Text canvas group")] [SerializeField]
        private CanvasGroup textCanvasGroup;

        [Header("Settings")] [Tooltip("Layout element normal width value")] [SerializeField]
        private float normalWidth;

        public float NormalWidth => normalWidth;

        [Tooltip("Layout Element expanded width value")] [SerializeField]
        private float expandedWidth;

        [Tooltip("Animation Duration")] [SerializeField]
        private float animationDuration;

        [Tooltip("Icon Expanded width ")] [SerializeField]
        private float iconExpandedY;

        [Tooltip("Tab Button")] [SerializeField]
        private Button tabButton;

        public Button TabButton => tabButton;

        private float _iconNormalY;

        private void Awake()
        {
            _iconNormalY = iconRect.anchoredPosition.y;

            textCanvasGroup.alpha = 0f;
        }

        public void SelectTab()
        {
            layoutElement.DOKill();

            DOVirtual.Float(layoutElement.preferredHeight, expandedWidth, animationDuration,
                v => { layoutElement.preferredWidth = v; }).SetEase(Ease.OutBack);

            iconRect.DOKill();
            iconRect.DOAnchorPosY(iconExpandedY, animationDuration).SetEase(Ease.OutBack);
            iconRect.DOScale(Vector3.one * 1.1f, animationDuration).SetEase(Ease.OutBack);

            textCanvasGroup.DOKill();
            textCanvasGroup.DOFade(1f, animationDuration);
        }

        public void DeselectTab()
        {
            layoutElement.DOKill();
            DOVirtual.Float(layoutElement.preferredWidth, normalWidth, animationDuration,
                v => { layoutElement.preferredWidth = v; }).SetEase(Ease.OutQuad);

            iconRect.DOKill();
            iconRect.DOAnchorPosY(_iconNormalY, animationDuration).SetEase(Ease.OutQuad);
            iconRect.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutQuad);

            textCanvasGroup.DOKill();
            textCanvasGroup.DOFade(0f, animationDuration);
        }

        private void OnDisable()
        {
            layoutElement.DOKill();
            iconRect.DOKill();
            textCanvasGroup.DOKill();
        }
    }
}