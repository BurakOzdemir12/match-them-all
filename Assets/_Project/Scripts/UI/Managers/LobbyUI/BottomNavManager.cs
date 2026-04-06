using System;
using System.Collections.Generic;
using _Project.Scripts.UI.Components.LobbyUI;
using UnityEngine;

namespace _Project.Scripts.UI.Managers.LobbyUI
{
    public class BottomNavManager : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private List<BottomNavItemUI> navItemList = new List<BottomNavItemUI>();

        private BottomNavItemUI _currentActiveTab;

        private void Start()
        {
            foreach (var tab in navItemList)
            {
                tab.TabButton.onClick.AddListener(() => OnNavClicked(tab));
                tab.LayoutElement.preferredWidth = tab.NormalWidth;
                tab.DeselectTab();
            }

            if (navItemList.Count > 0)
            {
                OnNavClicked(navItemList[0]);
            }
        }

        private void OnNavClicked(BottomNavItemUI clickedTab)
        {
            if (_currentActiveTab == clickedTab) return;

            if (_currentActiveTab != null)
            {
                _currentActiveTab.DeselectTab();
            }

            _currentActiveTab = clickedTab;
            _currentActiveTab.SelectTab();
        }
    }
}