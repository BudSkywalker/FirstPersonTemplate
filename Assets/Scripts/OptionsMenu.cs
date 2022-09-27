using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance;
    public static bool isOpen;
    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private Slider masterVolume;
    [SerializeField]
    private Slider musicVolume;
    [SerializeField]
    private Slider sfxVolume;
    [SerializeField]
    private Slider mouseSensativity;
    [SerializeField]
    private Slider controllerSensativity;
    [SerializeField]
    private Toggle toggleCrouch;
    [SerializeField]
    private Toggle toggleSprint;
    [SerializeField]
    private Toggle invertY;

    private void Awake()
    {
        float temp;
        mixer.GetFloat("Master Volume", out temp);
        PlayerPrefs.GetFloat("Master Volume", temp);
        mixer.GetFloat("Music Volume", out temp);
        PlayerPrefs.GetFloat("Music Volume", temp);
        mixer.GetFloat("SFX Volume", out temp);
        PlayerPrefs.GetFloat("SFX Volume", temp);
        

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void Start()
    {
        Set();

        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        isOpen = true;
        masterVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("Master Volume", 0));
        musicVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("Music Volume", 0));
        sfxVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFX Volume", 0));
        mouseSensativity.SetValueWithoutNotify(PlayerPrefs.GetFloat("Mouse Sensativity", 5));
        controllerSensativity.SetValueWithoutNotify(PlayerPrefs.GetFloat("Controller Sensativity", 15));
        toggleCrouch.isOn = PlayerPrefs.GetInt("Toggle Crouch", 0) == 1;
        toggleSprint.isOn = PlayerPrefs.GetInt("Toggle Sprint", 0) == 1;
        invertY.isOn = PlayerPrefs.GetInt("Invert Y", 0) == 1;

    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Master Volume", masterVolume.value);
        PlayerPrefs.SetFloat("Music Volume", musicVolume.value);
        PlayerPrefs.SetFloat("SFX Volume", sfxVolume.value);
        PlayerPrefs.SetFloat("Mouse Sensativity", mouseSensativity.value);
        PlayerPrefs.SetFloat("Controller Sensativity", controllerSensativity.value);

        if (toggleCrouch.isOn)
            PlayerPrefs.SetInt("Toggle Crouch", 1);
        else
            PlayerPrefs.SetInt("Toggle Crouch", 0);
        if (toggleSprint.isOn)
            PlayerPrefs.SetInt("Toggle Sprint", 1);
        else
            PlayerPrefs.SetInt("Toggle Sprint", 0);
        if (invertY.isOn)
            PlayerPrefs.SetInt("Invert Y", 1);
        else
            PlayerPrefs.SetInt("Invert Y", 0);

        Set();
    }

    private void Set()
    {
        mixer.SetFloat("Master Volume", PlayerPrefs.GetFloat("Master Volume"));
        mixer.SetFloat("Music Volume", PlayerPrefs.GetFloat("Music Volume"));
        mixer.SetFloat("SFX Volume", PlayerPrefs.GetFloat("SFX Volume"));

        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc != null) pc.UpdateSettings();
    }
    public void Close()
    {
        Save();
        isOpen = false;
        gameObject.SetActive(false);
    }
}
