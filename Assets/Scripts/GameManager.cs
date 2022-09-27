using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this) Destroy(gameObject);
        }
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
