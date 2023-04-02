using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Menu
{
    public class SettingsMenuHandler : MonoBehaviour
    {
        private RectTransform[] childrenPanels;

        private void Start()
        {
            childrenPanels = GetComponentsInChildren<RectTransform>(true).Where(x => x.CompareTag("Panel")).ToArray();
            foreach (Selectable s in GetComponentsInChildren<Selectable>(true))
            {
                
            }
            SelectPanel(childrenPanels[0].gameObject);
        }

        public void SelectPanel(GameObject panel)
        {
            foreach (RectTransform go in childrenPanels) go.gameObject.SetActive(go.gameObject == panel);
        }

        private UnityAction SaveSettings()
        {
            return null;
        }

    }
}