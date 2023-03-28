using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class PauseMenu : MonoBehaviour
    {
        private static PauseMenu Instance;

        private static bool _isPaused;
        public GameObject pauseMenu;
        public static bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                if (IsPaused) Instance.PauseGame();
                else Instance.ResumeGame();
            }
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);

            pauseMenu.SetActive(false);
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }

        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            _isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}