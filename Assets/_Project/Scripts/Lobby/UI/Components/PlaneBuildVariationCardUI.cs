using System;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Components
{
    public class PlaneBuildVariationCardUI : MonoBehaviour
    {
        [Header("UI References")] [Tooltip("Build Part Icon")] [SerializeField]
        private Image variationIcon;

        [Tooltip("Description text for build")] [SerializeField]
        private TextMeshProUGUI variationName;

        [Tooltip("Button for purchase")] [SerializeField]
        private Button buildButton;

        [Tooltip("Background Color")] [SerializeField]
        private Image backgroundColor;

        [Tooltip("Variation card Outline object")] [SerializeField]
        private Image variationCardOutline;

        [Tooltip("selected outline color")] [SerializeField]
        private Color selectedOutlineColor;

        [Tooltip("Default outline Color")] [SerializeField]
        private Color defaultOutlineColor;

        public event Action<PlaneBuildVariationCardUI, PlanePartSo, PlanePartVariation> OnPlaneBuildVariationSelected;

        private PlanePartSo _currentPartSo;
        private PlanePartVariation _currentPartVariation;

        public void SetUp(PlanePartSo partSo, PlanePartVariation variation)
        {
            _currentPartSo = partSo;
            _currentPartVariation = variation;
            variationIcon.sprite = variation.sprite;
            backgroundColor.color = variation.backgroundColor;
            variationName.text = variation.variationName;
            variationCardOutline.color = defaultOutlineColor;
        }

        public void OnCardButtonClicked()
        {
            variationCardOutline.color = selectedOutlineColor;

            OnPlaneBuildVariationSelected?.Invoke(this, _currentPartSo, _currentPartVariation);
        }

        public void RevertSelectedButton()
        {
            variationCardOutline.color = defaultOutlineColor;
        }
    }
}