using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Components
{
    public class GoalCardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI goalRemainingText;
        [SerializeField] private Image goalImage;

        public void Setup(int initialAmount, Sprite icon = null)
        {
            goalRemainingText.text = initialAmount.ToString();

            if (icon != null) goalImage.sprite = icon;
        }

        public void UpdateAmount(int currentAmount)
        {
            goalRemainingText.text = currentAmount.ToString();

            if (currentAmount <= 0)
            {
                // TODO Add Success Icon
            }
        }
    }
}