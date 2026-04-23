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

        public event Action<PlanePartSo, PlanePartVariation> OnBuildVariationSelected;

        private PlanePartSo _currentPartSo;
        private PlanePartVariation _currentPartVariation;

        public void SetUp(PlanePartSo partSo, PlanePartVariation variation)
        {
            _currentPartSo = partSo;
            _currentPartVariation = variation;
            variationIcon.sprite = variation.sprite;
            backgroundColor.color = variation.backgroundColor;
            variationName.text = variation.variationName;
        }

        public void OnCardButtonClicked()
        {
            OnBuildVariationSelected?.Invoke(_currentPartSo, _currentPartVariation);
        }
    }
}