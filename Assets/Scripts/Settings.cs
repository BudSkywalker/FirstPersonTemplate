using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

/// <summary>
/// Contains all settings, as well as dealing with config file IO
/// </summary>
public static class Settings
{
    private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "Settings.config");
    private static readonly XmlSerializer Serializer = new(typeof(SettingsContainer));
    private static SettingsContainer settings;
    private static bool hasLoaded;

    /// <summary>
    /// Used to get the current loaded settings
    /// </summary>
    /// <returns>Loaded settings</returns>
    public static ref SettingsContainer GetSettings()
    {
        if (!hasLoaded) LoadSettingsFromFile();
        return ref settings;
    }

    /// <summary>
    /// Save the current settings to config file
    /// </summary>
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
    /// <summary>
    /// Stores information about the currently loaded resolution
    /// </summary>
    [XmlIgnore]
    public Resolution resolution;

    #region XML Resolution
    /// <summary>
    /// Desired screen width
    /// </summary>
    public int width;
    /// <summary>
    /// Desired screen height
    /// </summary>
    public int height;
    /// <summary>
    /// Desired screen refreshRate
    /// </summary>
    public int refreshRate;
    #endregion

    public FullScreenMode fullScreenMode;
    /// <summary>
    /// Whether to use V-Sync or not
    /// </summary>
    public bool vSync;

    /// <summary>
    /// Quality level controls settings that can't be changed at runtime, mostly dealing with shadows and PPVs
    /// </summary>
    public QualityLevel qualityLevel;
    public AnisotropicFiltering anisotropicFiltering;
    /// <summary>
    /// Level of blending between lod levels
    /// </summary>
    public float lodBias;
    /// <summary>
    /// Limits how high-poly models can get
    /// </summary>
    public int maximumLODLevel;

    public bool depthTexture;
    public AntialiasingLevel antiAliasing;
    public float shadowDistance;
    public float shadowDepthBias;
    public float shadowNormalBias;

    public delegate void OnUpdateVideoSettings();

    public event OnUpdateVideoSettings onUpdateVideoSettings;

    public VideoSettings()
    {
        resolution = Screen.currentResolution;
        width = resolution.width;
        height = resolution.height;
        refreshRate = resolution.refreshRate;
        fullScreenMode = FullScreenMode.FullScreenWindow;
        vSync = true;
        qualityLevel = QualityLevel.Ultra;
        anisotropicFiltering = AnisotropicFiltering.Enable;
        lodBias = 1;
        maximumLODLevel = 4;
        depthTexture = true;
        antiAliasing = AntialiasingLevel.x8;
        shadowDistance = 150;
        shadowDepthBias = 1f;
        shadowNormalBias = 1f;

        UpdateVideoScreenSettings();
        UpdateVideoQualitySettings();
    }

    /// <summary>
    /// Updates the screen's resolution and refresh rate, and full screen mode
    /// </summary>
    public void UpdateVideoScreenSettings()
    {
        Screen.SetResolution(width, height, fullScreenMode, refreshRate);

        onUpdateVideoSettings?.Invoke();
    }

    /// <summary>
    /// Updates all quality settings/settings not mentioned in <see cref="UpdateVideoScreenSettings"/>
    /// </summary>
    public void UpdateVideoQualitySettings()
    {
        QualitySettings.SetQualityLevel((int)qualityLevel);

        QualitySettings.anisotropicFiltering = anisotropicFiltering;
        QualitySettings.lodBias = lodBias;
        QualitySettings.maximumLODLevel = maximumLODLevel;
        QualitySettings.vSyncCount = vSync ? 1 : 0;

        UniversalRenderPipelineAsset urpAsset = UniversalRenderPipeline.asset;
        urpAsset.supportsCameraDepthTexture = depthTexture;
        urpAsset.msaaSampleCount = antiAliasing switch
        {
            AntialiasingLevel.Disabled => 0,
            AntialiasingLevel.x2 => 2,
            AntialiasingLevel.x4 => 4,
            AntialiasingLevel.x8 => 8,
            _ => urpAsset.msaaSampleCount
        };
        urpAsset.shadowDistance = shadowDistance;
        urpAsset.shadowDepthBias = shadowDepthBias;
        urpAsset.shadowNormalBias = shadowNormalBias;

        onUpdateVideoSettings?.Invoke();
    }
}

/// <summary>
/// This is a custom enum that needs to be manually changed to reflect the Project Settings
/// </summary>
[Serializable]
public enum QualityLevel
{
    VeryLow,
    Low,
    Medium,
    High,
    VeryHigh,
    Ultra
}

[Serializable]
public enum AntialiasingLevel
{
    Disabled,
    x2,
    x4,
    x8
}

[Serializable]
public class AudioSettings
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambienceVolume;
    public float dialogueVolume;
    public AudioSpeakerMode speakerMode;
    public bool subtitles;

    private AudioMixer mixer;

    public AudioSettings()
    {
        mixer = Resources.Load<AudioMixer>("Audio/Mixer");

        masterVolume = 0;
        musicVolume = 0;
        sfxVolume = 0;
        ambienceVolume = 0;
        dialogueVolume = 0;
        speakerMode = AudioSpeakerMode.Stereo;
        subtitles = false;
    }

    public void ApplyMixerSettings()
    {
        mixer ??= Resources.Load<AudioMixer>("Audio/Mixer");
        mixer.SetFloat("Master Volume", masterVolume <= -20 ? -80 : masterVolume);
        mixer.SetFloat("Music Volume", musicVolume <= -20 ? -80 : musicVolume);
        mixer.SetFloat("SFX Volume", sfxVolume <= -20 ? -80 : sfxVolume);
        mixer.SetFloat("Ambience Volume", ambienceVolume <= -20 ? -80 : ambienceVolume);
        mixer.SetFloat("Dialogue Volume", dialogueVolume <= -20 ? -80 : dialogueVolume);
    }

    public void ApplySpeakerMode()
    {
        AudioConfiguration config = UnityEngine.AudioSettings.GetConfiguration();
        config.speakerMode = speakerMode;
        UnityEngine.AudioSettings.Reset(config);
    }
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
    /// <summary>
    /// List of keybinds that the player has overriden from default
    /// If you want to change this list please use <see cref="AddOverride"/>
    /// </summary>
    public List<KeybindOverride> keybindOverrides = new();

    /// <summary>
    /// Loads overrides from settings file to <param name="input"></param>
    /// </summary>
    /// <param name="input">PlayerInput to have keybinds overridden</param>
    public void LoadOverrides(ref PlayerInput input)
    {
        foreach (KeybindOverride keybind in keybindOverrides)
        {
            input.currentActionMap.FindAction(keybind.actionName).ApplyBindingOverride(keybind.bindingIndex, keybind.bindingPath);
            KeyRebinder.UpdateKeybindLabels();
        }
    }

    /// <summary>
    /// Adds override to the keybindOverrides list safely, replacing a value instead of blindly adding it
    /// </summary>
    /// <param name="actionName">The ID on the Action to be overridden</param>
    /// <param name="bindingIndex">The ID of the binding to be overridden</param>
    /// <param name="bindingPath">The path the the keybind that will override the default</param>
    public void AddOverride(string actionName, int bindingIndex, string bindingPath)
    {
        foreach (KeybindOverride k in keybindOverrides.Where(k => k.actionName.Equals(actionName)))
        {
            keybindOverrides.Remove(k);
            break;
        }

        keybindOverrides.Add(new(actionName, bindingIndex, bindingPath));
    }
}

[Serializable]
public struct KeybindOverride
{
    public string actionName;
    public int bindingIndex;
    public string bindingPath;

    public KeybindOverride(string actionName, int bindingIndex, string bindingPath)
    {
        this.actionName = actionName;
        this.bindingIndex = bindingIndex;
        this.bindingPath = bindingPath;
    }

    public void SetValues(string newActionID, int newBindingID, string newBindingPath)
    {
        actionName = newActionID;
        bindingIndex = newBindingID;
        bindingPath = newBindingPath;
    }
}