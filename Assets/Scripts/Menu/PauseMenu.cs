using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class PauseMenu : MonoBehaviour
    {
        private static PauseMenu Instance;

        private static bool isPaused;
        public GameObject pauseMenu;
        public static bool IsPaused
        {
            get => isPaused;
            set
            {
                isPaused = value;
                if (IsPaused) Instance.PauseGame();
                else Instance.ResumeGame();
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
            {
                Destroy(this);
            }

            pauseMenu.SetActive(false);
        }

        private void Update()
        {
            //TODO: Update controls
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        private void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        [Obsolete]
        public void MainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("Going to main menu...");
            SceneManager.LoadScene("Menu");
        }

        [Obsolete]
        public void QuitGame()
        {
            Debug.Log("Quitting Game");
            Application.Quit();
        }
    }
}