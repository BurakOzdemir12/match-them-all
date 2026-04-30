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


        public void SetUp(PlaneBluePrintSo planeBluePrintSo)
        {
            planeNameText.text = planeBluePrintSo.planeName;
            _planeId = int.Parse(planeBluePrintSo.planeID);
        }

        public void OnSelectButtonClicked()
        {
        }
    }
}