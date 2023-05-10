using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Automatically updates the <see cref="PlayerSettings.bundleVersion"/> based upon the Git pushes if this is a Git project,
/// otherwise it simply updates the version to a Major.minor sequence instead of Major.minor.commits
/// </summary>
[UsedImplicitly]
public class AutoVersioning : IPrebuildSetup
{
    /// <summary>
    /// The <see cref="VersionSettings"/> file name
    /// </summary>
    private const string FILE = "Version.asset";
    /// <summary>
    /// The folder which stores the <see cref="VersionSettings"/> file
    /// </summary>
    private const string DIRECTORY = "Assets/Scripts/Editor/";
    /// <summary>
    /// The <see cref="VersionSettings"/> relative file path, based on the <see cref="DIRECTORY"/> and <see cref="FILE"/>
    /// </summary>
    private const string PATH = DIRECTORY + FILE;
    /// <summary>
    /// Reference to the <see cref="VersionSettings"/> <see cref="ScriptableObject"/> that is currently loaded
    /// </summary>
    public static VersionSettings Settings { get; private set; }

    public void Setup()
    {
        UpdateVersion();
    }
    
    /// <summary>
    /// Updates the <see cref="PlayerSettings.bundleVersion"/> based upon the current <see cref="Settings"/>
    /// </summary>
    /// <remarks>
    /// Happens every time the scripts reload, the settings are changed, and before every build
    /// </remarks>
    [DidReloadScripts]
    public static void UpdateVersion()
    {
        Settings = LoadVersionInfoFile();

        #region Get Git Commit Count
        Process process = new()
        {
            StartInfo = new()
            {
                FileName = "cmd.exe",
                WorkingDirectory = Application.dataPath,
                Arguments = "/c git rev-list --all --count",
                CreateNoWindow = false,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        process.Start();
        process.WaitForExit(10000);
        string num = process.StandardOutput.ReadLine();
        process.Close();
        #endregion

        // Checks to see if the Git Commit Count is something,
        // if so, set to Major.minor.count, otherwise just Major.minor
        PlayerSettings.bundleVersion = string.IsNullOrEmpty(num) ? $"{(int)Settings.majorVersion}.{Settings.minorVersion}" : $"{(int)Settings.majorVersion}.{Settings.minorVersion}.{num}";
    }

    /// <summary>
    /// Tries to load the <see cref="VersionSettings"/> file from <see cref="PATH"/>,
    /// and creates a new file there if there is no file
    /// </summary>
    /// <returns><see cref="Settings"/></returns>
    private static VersionSettings LoadVersionInfoFile()
    {
        Directory.CreateDirectory(DIRECTORY);
        Settings = AssetDatabase.LoadAssetAtPath<VersionSettings>(PATH);

        if (Settings != null) return Settings;

        Settings = ScriptableObject.CreateInstance<VersionSettings>();
        Settings.majorVersion = VersionSettings.MajorVersion.PreRelease;
        Settings.minorVersion = 1;
        AssetDatabase.CreateAsset(Settings, PATH);

        return Settings;
    }
}

/// <summary>
/// Creates a new window in the Project Settings under Player to help streamline the version naming process
/// </summary>
/// <remarks>Version settings can also be changed by editing the <see cref="ScriptableObject"/> directly</remarks>
internal static class VersionSettingsWindow
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        SettingsProvider provider = new("Project/Player/Version", SettingsScope.Project)
        {
            label = "Version",
            guiHandler = _ =>
            {
                SerializedObject settings = new(AutoVersioning.Settings);

                settings.FindProperty("majorVersion").enumValueIndex = EditorGUILayout.Popup("Major Version", settings.FindProperty("majorVersion").enumValueIndex, settings.FindProperty("majorVersion").enumDisplayNames);
                settings.FindProperty("minorVersion").intValue = EditorGUILayout.IntField("Minor Version", settings.FindProperty("minorVersion").intValue);

                settings.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            },
            keywords = new HashSet<string>(new[] { "Version" })
        };

        return provider;
    }
}