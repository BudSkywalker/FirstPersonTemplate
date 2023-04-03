using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Menu
{
    public class SettingsField : MonoBehaviour
    {
        public SettingsMenu settingsMenu;

        public FieldInfo setting;
    }

    [CustomEditor(typeof(SettingsField))]
    public class SettingsFieldEditor : Editor
    {
        private Dictionary<SettingsMenu, Type> menu = new()
        {
            { SettingsMenu.Video, typeof(VideoSettings) },
            { SettingsMenu.Audio, typeof(AudioSettings) },
            { SettingsMenu.Gameplay, typeof(GameplaySettings) },
            { SettingsMenu.Keybinds, typeof(KeybindSettings) }
        };

        public override void OnInspectorGUI()
        {
            SettingsField settingsField = target as SettingsField;

            GUILayoutOption[] options = { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true) };
            EditorGUILayout.EnumPopup("Settings Menu", settingsField.settingsMenu, options);
        }
    }

    public enum SettingsMenu
    {
        Video,
        Audio,
        Gameplay,
        Keybinds
    }
}