using UnityEngine;

namespace _Project.Scripts.Lobby.UI.Managers
{
    public class OverlayUIManager : MonoBehaviour
    {
        [Header("Blackout Settings")] public GameObject bgBlackout;

        private readonly int popupSortOrder = 11;

        public void ShowPanel(Canvas targetPanel, Canvas[] keepVisibleExceptions = null)
        {
            bgBlackout.SetActive(true);

            targetPanel.overrideSorting = true;
            targetPanel.sortingOrder = popupSortOrder;

            if (keepVisibleExceptions != null)
            {
                foreach (Canvas exception in keepVisibleExceptions)
                {
                    exception.overrideSorting = true;
                    exception.sortingOrder = popupSortOrder;
                }
            }
        }


        public void HidePanel(Canvas targetPanel, Canvas[] keepVisibleExceptions = null)
        {
            bgBlackout.SetActive(false);

            targetPanel.overrideSorting = false;

            if (keepVisibleExceptions != null)
            {
                foreach (Canvas exception in keepVisibleExceptions)
                {
                    exception.overrideSorting = false;
                }
            }
        }
    }
}