using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance;
	public GameObject pauseMenu;

	public static bool isPaused = false;
	public bool wasPressed = false;

	void Start()
	{
		pauseMenu.SetActive(false);
		if(Instance == null)
        {
			Instance = this;
			DontDestroyOnLoad(this);
        }
		else if(Instance != this)
        {
			Destroy(this);
        }
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
		{
			if (isPaused)
			{
				ResumeGame();
			}
			else
			{
				PauseGame();
			}
		}
	}

	public void TogglePause()
    {
		if (isPaused) ResumeGame();
		else PauseGame();
    }

	public void PauseGame()
	{
		pauseMenu.SetActive(true);
		Time.timeScale = 0f;
		isPaused = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void ResumeGame()
	{
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void MainMenu()
	{
		Time.timeScale = 1f;
		isPaused = false;
		Debug.Log("Going to main menu...");
		SceneManager.LoadScene("Menu");
	}

	public void QuitGame()
	{
		Debug.Log("Quitting Game");
		Application.Quit();
	}
}