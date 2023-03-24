using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menu
{
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
        private Slider mouseSensitivity;
        [SerializeField]
        private Slider controllerSensitivity;
        [SerializeField]
        private Toggle toggleCrouch;
        [SerializeField]
        private Toggle toggleSprint;
        [SerializeField]
        private Toggle invertY;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this) Destroy(this);
            }

            float temp;
            mixer.GetFloat("Master Volume", out temp);
            PlayerPrefs.GetFloat("Master Volume", temp);
            mixer.GetFloat("Music Volume", out temp);
            PlayerPrefs.GetFloat("Music Volume", temp);
            mixer.GetFloat("SFX Volume", out temp);
            PlayerPrefs.GetFloat("SFX Volume", temp);
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
            mouseSensitivity.SetValueWithoutNotify(PlayerPrefs.GetFloat("Mouse Sensitivity", 5));
            controllerSensitivity.SetValueWithoutNotify(PlayerPrefs.GetFloat("Controller Sensitivity", 15));
            toggleCrouch.isOn = PlayerPrefs.GetInt("Toggle Crouch", 0) == 1;
            toggleSprint.isOn = PlayerPrefs.GetInt("Toggle Sprint", 0) == 1;
            invertY.isOn = PlayerPrefs.GetInt("Invert Y", 0) == 1;
        }

        public void Save()
        {
            //Sound
            PlayerPrefs.SetFloat("Master Volume", masterVolume.value);
            PlayerPrefs.SetFloat("Music Volume", musicVolume.value);
            PlayerPrefs.SetFloat("SFX Volume", sfxVolume.value);

            //Player
            PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity.value);
            PlayerPrefs.SetFloat("Controller Sensitivity", controllerSensitivity.value);
            PlayerPrefs.SetInt("Toggle Crouch", toggleCrouch.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Toggle Sprint", toggleSprint.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Invert Y", invertY.isOn ? 1 : 0);

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
}