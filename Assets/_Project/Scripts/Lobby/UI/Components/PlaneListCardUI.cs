using System;
using _Project.Scripts.Lobby.ScriptableObjects.Plane;
using _Project.Scripts.Lobby.Static;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Lobby.UI.Components
{
    public class PlaneListCardUI : MonoBehaviour
    {
        [field: Header("Plane")]
        [field: SerializeField]
        public PlaneBluePrintSo planeSo { get; set; }

        [Header("Card UI References")] [SerializeField]
        private Image cardImage;

        [SerializeField] private Color defaultCardColor;
        [SerializeField] private Color lockedCardColor;

        [SerializeField] private Image lockImage;
        [SerializeField] private Image planeImage;

        [SerializeField] private TextMeshProUGUI planeNameText;
        [SerializeField] private TextMeshProUGUI planeNo;

        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressSlider;

        [SerializeField] private Button selectButton;


        private int _planeId;
        public event Action<PlaneBluePrintSo, int> OnPlaneEditSelected;

        public void SetUp(PlaneBluePrintSo planeBluePrintSo)
        {
            planeNameText.text = planeBluePrintSo.planeName;
            _planeId = int.Parse(planeBluePrintSo.planeID);
            planeSo = planeBluePrintSo;
            planeImage = planeBluePrintSo.planeImage;
        }

        public void OnSelectButtonClicked()
        {
            OnPlaneEditSelected?.Invoke(planeSo, _planeId);
        }
    }
}