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

    public static ref SettingsContainer GetSettings()
    {
        if (!hasLoaded) LoadSettingsFromFile();
        return ref settings;
    }

    public static void SaveSettings()
    {
        SaveSettingsToFile();
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

            settings = new(new(), new(), new(), new());

            stream.Close();
            SaveSettingsToFile();
        }
        finally
        {
            hasLoaded = true;
            Debug.Log("Settings file located at " + FilePath);
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
public class VideoSettings
{
    //TODO
}

[Serializable]
public class AudioSettings
{
    //TODO
}

[Serializable]
public class GameplaySettings
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

    public GameplaySettings()
    {
        mouseSensitivity = 0.5f;
        controllerSensitivity = 5f;
        toggleCrouch = false;
        toggleSprint = false;
        invertY = false;
        fovModifier = true;
    }
}

[Serializable]
public class KeybindSettings
{
    //TODO
}