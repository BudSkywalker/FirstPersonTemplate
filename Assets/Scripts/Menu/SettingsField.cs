using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Menu
{
    /// <summary>
    /// Allows you to pick a setting, and will use the Selectable that is attached to this script to update the setting's value
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class SettingsField : MonoBehaviour
    {
        [SerializeReference]
        internal SettingsMenu settingsMenu;
        [SerializeReference]
        internal int settingIndex;
        private readonly Dictionary<SettingsMenu, Type> menu = new()
        {
            { SettingsMenu.Video, typeof(VideoSettings) },
            { SettingsMenu.Audio, typeof(AudioSettings) },
            { SettingsMenu.Gameplay, typeof(GameplaySettings) },
            { SettingsMenu.Keybinds, typeof(KeybindSettings) }
        };
        private bool hasSetListeners;
        internal FieldInfo[] loadedFields;
        private FieldInfo setting;

        private void OnEnable()
        {
            UpdateField(!hasSetListeners);
            hasSetListeners = true;
            if (settingsMenu == SettingsMenu.Video) Settings.GetSettings().videoSettings.onUpdateVideoSettings += UpdateField;
        }

        private void OnDisable()
        {
            Settings.GetSettings().videoSettings.onUpdateVideoSettings -= UpdateField;
        }

        public void UpdateField()
        {
            UpdateField(false);
        }

        private void UpdateField(bool addListeners)
        {
            GetMenuFields();
            if (loadedFields.Length == 0) return;

            setting = loadedFields[settingIndex];

            switch (GetComponent<Selectable>())
            {
                case Toggle toggle:
                    if (addListeners) toggle.onValueChanged.AddListener(_ => UpdateSettings(toggle.isOn));
                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            toggle.SetIsOnWithoutNotify((bool)setting.GetValue(Settings.GetSettings().videoSettings));
                            break;
                        case SettingsMenu.Audio:
                            toggle.SetIsOnWithoutNotify((bool)setting.GetValue(Settings.GetSettings().audioSettings));
                            break;
                        case SettingsMenu.Gameplay:
                            toggle.SetIsOnWithoutNotify((bool)setting.GetValue(Settings.GetSettings().gameplaySettings));
                            break;
                        case SettingsMenu.Keybinds:
                            toggle.SetIsOnWithoutNotify((bool)setting.GetValue(Settings.GetSettings().keybindSettings));
                            break;
                        default:
                            Debug.LogError($"Unknown menu type {settingsMenu}");
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case Slider slider:
                    if (addListeners)
                    {
                        if (slider.wholeNumbers) slider.onValueChanged.AddListener(_ => UpdateSettings((int)slider.value));
                        else slider.onValueChanged.AddListener(_ => UpdateSettings(slider.value));
                    }

                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            try
                            {
                                slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().videoSettings));
                            }
                            catch (InvalidCastException)
                            {
                                slider.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().videoSettings));
                            }

                            break;
                        case SettingsMenu.Audio:
                            try
                            {
                                slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().audioSettings));
                            }
                            catch (InvalidCastException)
                            {
                                slider.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().audioSettings));
                            }

                            Settings.GetSettings().audioSettings.ApplyMixerSettings();
                            break;
                        case SettingsMenu.Gameplay:
                            try
                            {
                                slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().gameplaySettings));
                            }
                            catch (InvalidCastException)
                            {
                                slider.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().gameplaySettings));
                            }

                            break;
                        case SettingsMenu.Keybinds:
                            try
                            {
                                slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().keybindSettings));
                            }
                            catch (InvalidCastException)
                            {
                                slider.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().keybindSettings));
                            }

                            break;
                        default:
                            Debug.LogError($"Unknown menu type {settingsMenu}");
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case Button button:
                    //b.onClick.AddListener(() => UpdateSettings());
                    break;
                case TMP_Dropdown dropdown:
                    switch (setting.Name)
                    {
                        case "resolution":
                        {
                            dropdown.options = Screen.resolutions.Select(x => x.width + "x" + x.height).Distinct().Select(s => new TMP_Dropdown.OptionData(s)).ToList();
                            if (addListeners) dropdown.onValueChanged.AddListener(_ => UpdateSettings(dropdown.options[dropdown.value].text));
                            int index = Array.IndexOf(dropdown.options.ToArray(), dropdown.options.ToArray().First(x => x.text.Equals(Settings.GetSettings().videoSettings.width + "x" + Settings.GetSettings().videoSettings.height)));
                            dropdown.SetValueWithoutNotify(index);
                            break;
                        }
                        case "refreshRate":
                        {
                            dropdown.options = Screen.resolutions.Select(x => x.refreshRate).Distinct().Select(s => new TMP_Dropdown.OptionData(s.ToString())).ToList();
                            if (addListeners) dropdown.onValueChanged.AddListener(_ => UpdateSettings(dropdown.options[dropdown.value].text));
                            int index = Array.IndexOf(dropdown.options.ToArray(), dropdown.options.ToArray().First(x => x.text.Equals(Settings.GetSettings().videoSettings.refreshRate.ToString())));
                            dropdown.SetValueWithoutNotify(index);
                            break;
                        }
                        default:
                        {
                            dropdown.options = Enum.GetNames(setting.FieldType).Select(s => new TMP_Dropdown.OptionData(s)).ToList();

                            if (addListeners) dropdown.onValueChanged.AddListener(_ => UpdateSettings(setting.FieldType.GetEnumValues().GetValue(dropdown.value)));
                            switch (settingsMenu)
                            {
                                case SettingsMenu.Video:
                                    dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().videoSettings));
                                    break;
                                case SettingsMenu.Audio:
                                    dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().audioSettings));
                                    Settings.GetSettings().audioSettings.ApplySpeakerMode();
                                    break;
                                case SettingsMenu.Gameplay:
                                    dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().gameplaySettings));
                                    break;
                                case SettingsMenu.Keybinds:
                                    dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().keybindSettings));
                                    break;
                                default:
                                    Debug.LogError($"Unknown menu type {settingsMenu}");
                                    throw new ArgumentOutOfRangeException();
                            }

                            break;
                        }
                    }

                    break;
                case TMP_InputField inputField:
                    if (addListeners) inputField.onValueChanged.AddListener(_ => UpdateSettings(inputField.text));
                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            inputField.text = (string)setting.GetValue(Settings.GetSettings().videoSettings);
                            break;
                        case SettingsMenu.Audio:
                            inputField.text = (string)setting.GetValue(Settings.GetSettings().audioSettings);
                            break;
                        case SettingsMenu.Gameplay:
                            inputField.text = (string)setting.GetValue(Settings.GetSettings().gameplaySettings);
                            break;
                        case SettingsMenu.Keybinds:
                            inputField.text = (string)setting.GetValue(Settings.GetSettings().keybindSettings);
                            break;
                        default:
                            Debug.LogError($"Unknown menu type {settingsMenu}");
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    Debug.LogError("Invalid Selectable Type, please choose a valid field for a settings option");
                    break;
            }
        }

        private void UpdateSettings(object value)
        {
            switch (settingsMenu)
            {
                case SettingsMenu.Video:
                    if (setting.Name.Equals("resolution"))
                    {
                        int xIndex = Array.IndexOf(((string)value).ToCharArray(), 'x');
                        Settings.GetSettings().videoSettings.width = int.Parse(((string)value)[..xIndex]);
                        Settings.GetSettings().videoSettings.height = int.Parse(((string)value)[(xIndex + 1)..]);
                    }
                    else
                    {
                        setting.SetValue(Settings.GetSettings().videoSettings, value);
                    }

                    if (setting.Name.Equals("resolution") || setting.Name.Equals("refreshRate") || setting.Name.Equals("fullScreenMode")) Settings.GetSettings().videoSettings.UpdateVideoScreenSettings();
                    else Settings.GetSettings().videoSettings.UpdateVideoQualitySettings();
                    break;
                case SettingsMenu.Audio:
                    setting.SetValue(Settings.GetSettings().audioSettings, value);
                    Settings.GetSettings().audioSettings.ApplyMixerSettings();
                    break;
                case SettingsMenu.Gameplay:
                    setting.SetValue(Settings.GetSettings().gameplaySettings, value);
                    break;
                case SettingsMenu.Keybinds:
                    setting.SetValue(Settings.GetSettings().keybindSettings, value);
                    break;
                default:
                    Debug.LogError($"Unknown menu type {settingsMenu}");
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets all valid fields based on the selected menu and the Selectable type
        /// </summary>
        internal void GetMenuFields()
        {
            switch (GetComponent<Selectable>())
            {
                case Toggle:
                    loadedFields = menu[settingsMenu].GetFields().Where(x => x.FieldType == typeof(bool)).ToArray();
                    break;
                case Slider:
                    loadedFields = menu[settingsMenu].GetFields().Where(x => x.FieldType == typeof(float) || x.FieldType == typeof(int)).ToArray();
                    break;
                case Button:
                    break;
                case TMP_Dropdown:
                    loadedFields = menu[settingsMenu].GetFields().Where(x => x.FieldType.IsEnum || x.Name.Equals("resolution") || x.Name.Equals("refreshRate")).ToArray();
                    break;
                case TMP_InputField:
                    loadedFields = menu[settingsMenu].GetFields().Where(x => x.FieldType == typeof(string)).ToArray();
                    break;
                default:
                    Debug.LogError("Invalid Selectable Type, please choose a valid field for a settings option");
                    break;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SettingsField))]
    [CanEditMultipleObjects]
    public class SettingsFieldEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayoutOption[] options = { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
            SerializedProperty settingsMenu = serializedObject.FindProperty("settingsMenu");
            SerializedProperty selectedFieldIndex = serializedObject.FindProperty("settingIndex");
            SettingsField settingsField = target as SettingsField;
            settingsField!.GetMenuFields();

            settingsMenu.enumValueIndex = EditorGUILayout.Popup("Settings Menu", settingsMenu.enumValueIndex, Enum.GetNames(typeof(SettingsMenu)), options);

            selectedFieldIndex.intValue = EditorGUILayout.Popup("Setting", selectedFieldIndex.intValue, settingsField.loadedFields.Select(x => x.Name).ToArray(), options);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    internal enum SettingsMenu
    {
        Video,
        Audio,
        Gameplay,
        Keybinds
    }
}