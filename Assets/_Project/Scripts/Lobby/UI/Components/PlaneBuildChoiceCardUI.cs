using System;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Components
{
    public class PlaneBuildChoiceCardUI : MonoBehaviour
    {
        [Header("UI References")] [Tooltip("How many wrench require to build this part")] [SerializeField]
        private TextMeshProUGUI wrenchRequire;

        [Tooltip("Build Part Icon")] [SerializeField]
        private Image partIcon;

        [Tooltip("Description text for build")] [SerializeField]
        private TextMeshProUGUI descriptionText;

        [Tooltip("Button for purchase")] [SerializeField]
        private Button buildButton;

        private PlanePartSo _currentPartSo;

        public event Action<PlanePartSo> OnBuildChoiceSelected;

        public void Setup(PlanePartSo partSo)
        {
            _currentPartSo = partSo;
            wrenchRequire.text = partSo.requiredWrench.ToString();
            descriptionText.text = partSo.descriptionText;
            partIcon.sprite = partSo.partIcon;
        }

        public void OnBuildButtonClick()
        {
            OnBuildChoiceSelected?.Invoke(_currentPartSo);
        }
    }
}