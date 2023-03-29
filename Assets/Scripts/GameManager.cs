using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this) Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        Settings.SaveSettings();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game!");
        Application.Quit();
    }
}