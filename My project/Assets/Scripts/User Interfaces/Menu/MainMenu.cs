using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        Time.timeScale = 0;
    }

    public void PlayGame()
    {
        //Debug.Log("Attempting to Play Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        //Debug.Log("App has Quit");
        Application.Quit();
    }
}