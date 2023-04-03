using System;
using System.Linq;
using OpenCover.Framework.Model;
using TMPro;
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
            childrenPanels = GetComponentsInChildren<RectTransform>(true).Where(x => x != this && x.CompareTag("Panel")).ToArray();
            foreach (Toggle t in GetComponentsInChildren<Toggle>(true)) t.onValueChanged.AddListener(_ => SaveSettings());
            foreach (Slider s in GetComponentsInChildren<Slider>(true)) s.onValueChanged.AddListener(_ => Invoke(nameof(SaveSettings), 0.1f));
            foreach (Button b in GetComponentsInChildren<Button>(true)) b.onClick.AddListener(() => Invoke(nameof(SaveSettings), 0.1f));
            foreach (TMP_Dropdown d in GetComponentsInChildren<TMP_Dropdown>(true)) d.onValueChanged.AddListener(_ => Invoke(nameof(SaveSettings), 0.1f));
            foreach (TMP_InputField i in GetComponentsInChildren<TMP_InputField>(true)) i.onValueChanged.AddListener(_ => Invoke(nameof(SaveSettings), 0.1f));
            SelectPanel(childrenPanels[0].gameObject);
        }

        public void SelectPanel(GameObject panel)
        {
            foreach (RectTransform p in childrenPanels) p.gameObject.SetActive(p.gameObject == panel);
        }

        private void SaveSettings()
        {
            Settings.SaveSettings();
        }
    }
}