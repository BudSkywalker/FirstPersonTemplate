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
        internal FieldInfo[] loadedFields;
        private FieldInfo setting;

        private void Awake()
        {
            GetMenuFields();
            setting = loadedFields[settingIndex];

            switch (GetComponent<Selectable>())
            {
                case Toggle toggle:
                    toggle.onValueChanged.AddListener(_ => UpdateSettings(toggle.isOn));
                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            toggle.isOn = (bool)setting.GetValue(Settings.GetSettings().videoSettings);
                            break;
                        case SettingsMenu.Audio:
                            toggle.isOn = (bool)setting.GetValue(Settings.GetSettings().audioSettings);
                            break;
                        case SettingsMenu.Gameplay:
                            toggle.isOn = (bool)setting.GetValue(Settings.GetSettings().gameplaySettings);
                            break;
                        case SettingsMenu.Keybinds:
                            toggle.isOn = (bool)setting.GetValue(Settings.GetSettings().keybindSettings);
                            break;
                        default:
                            Debug.LogError($"Unknown menu type {settingsMenu}");
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case Slider slider:
                    slider.onValueChanged.AddListener(_ => UpdateSettings(slider.value));
                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().videoSettings));
                            break;
                        case SettingsMenu.Audio:
                            slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().audioSettings));
                            break;
                        case SettingsMenu.Gameplay:
                            slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().gameplaySettings));
                            break;
                        case SettingsMenu.Keybinds:
                            slider.SetValueWithoutNotify((float)setting.GetValue(Settings.GetSettings().keybindSettings));
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
                    //TODO: The dropdowns don't work yet
                    dropdown.options = Enum.GetNames(setting.FieldType).Select(s => new TMP_Dropdown.OptionData(s)).ToList();

                    dropdown.onValueChanged.AddListener(_ => UpdateSettings(dropdown.value));
                    switch (settingsMenu)
                    {
                        case SettingsMenu.Video:
                            dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().videoSettings));
                            break;
                        case SettingsMenu.Audio:
                            dropdown.SetValueWithoutNotify((int)setting.GetValue(Settings.GetSettings().audioSettings));
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
                case TMP_InputField inputField:
                    inputField.onValueChanged.AddListener(_ => UpdateSettings(inputField.text));
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
                    setting.SetValue(Settings.GetSettings().videoSettings, value);
                    break;
                case SettingsMenu.Audio:
                    setting.SetValue(Settings.GetSettings().audioSettings, value);
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
                    loadedFields = menu[settingsMenu].GetFields().Where(x => x.FieldType.IsEnum).ToArray();
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