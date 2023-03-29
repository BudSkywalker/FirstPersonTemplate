using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class Settings
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "Settings.config");
    private static readonly XmlSerializer Serializer = new(typeof(SettingsContainer));
    private static SettingsContainer settings;
    private static bool hasLoaded;

    public static SettingsContainer GetSettings()
    {
        if (!hasLoaded) LoadSettingsFromFile();
        return settings;
    }

    public static void SaveSettings()
    {
        SaveSettingsToFile();
        Debug.Log("Settings saved to " + FilePath);
    }

    private static void SaveSettingsToFile()
    {
        try
        {
            FileStream stream = File.Open(FilePath, FileMode.Create, FileAccess.ReadWrite);
            Serializer.Serialize(stream, settings);
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error while trying to save settings: " + e);
        }
    }

    private static void LoadSettingsFromFile()
    {
        if (hasLoaded)
        {
            Debug.LogWarning("Files have already been loaded, we aren't going to load them again");
            return;
        }

        using FileStream stream = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        try
        {
            settings = (SettingsContainer)Serializer.Deserialize(stream);
        }
        catch (Exception e)
        {
            if (stream.Length == 0) Debug.Log("Settings.config was either empty or did not exist");
            else Debug.LogError("Error while trying to load settings: " + e);

            Debug.Log("Building new settings file from default values");

            //NOTE: Default settings are declared here
            settings = new(
                new(),
                new(),
                new(0.5f, 5f, false, false, false, true),
                new());

            stream.Close();
            SaveSettingsToFile();
        }
        finally
        {
            hasLoaded = true;
        }
    }
}

[XmlRoot("Settings")]
[Serializable]
public struct SettingsContainer
{
    public VideoSettings videoSettings;
    public AudioSettings audioSettings;
    public GameplaySettings gameplaySettings;
    public KeybindSettings keybindSettings;

    public SettingsContainer(VideoSettings video, AudioSettings audio, GameplaySettings gameplay, KeybindSettings keybind)
    {
        videoSettings = video;
        audioSettings = audio;
        gameplaySettings = gameplay;
        keybindSettings = keybind;
    }
}

[Serializable]
public struct VideoSettings
{
    //TODO
}

[Serializable]
public struct AudioSettings
{
    //TODO
}

[Serializable]
public struct GameplaySettings
{
    /// <summary>
    /// What to multiply the mouse input by when looking around
    /// </summary>
    public float mouseSensitivity;
    /// <summary>
    /// What to multiply the controller input by when looking around
    /// </summary>
    public float controllerSensitivity;
    /// <summary>
    /// Whether crouching should be trigger by holding Crouch or tapping Crouch
    /// </summary>
    public bool toggleCrouch;
    /// <summary>
    /// Whether sprinting should be trigger by holding Sprint or tapping Sprint
    /// </summary>
    public bool toggleSprint;
    /// <summary>
    /// Inverts the Y control
    /// </summary>
    public bool invertY;
    /// <summary>
    /// Should the camera FOV move when sprinting (can cause motion sickness)
    /// </summary>
    public bool fovModifier;

    public GameplaySettings(float mouseSensitivity, float controllerSensitivity, bool toggleCrouch, bool toggleSprint, bool invertY, bool fovModifier)
    {
        this.mouseSensitivity = mouseSensitivity;
        this.controllerSensitivity = controllerSensitivity;
        this.toggleCrouch = toggleCrouch;
        this.toggleSprint = toggleSprint;
        this.invertY = invertY;
        this.fovModifier = fovModifier;
    }
}

[Serializable]
public struct KeybindSettings
{
    //TODO
}