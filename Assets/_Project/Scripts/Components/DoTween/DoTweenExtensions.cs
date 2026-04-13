using DG.Tweening;
using TMPro;

namespace _Project.Scripts.Components.DoTween
{
    public static class DoTweenExtensions
    {
        public static Tweener DoCounterInt(this TextMeshProUGUI textElement, int startValue, int endValue,
            float duration, Ease easeType = Ease.OutQuad, string format = "{0}")
        {
            int currentValue = startValue;
            return DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                textElement.text = string.Format(format, currentValue);
            }, endValue, duration).SetEase(easeType);
        }
    }
}