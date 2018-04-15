using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    public GameObject pauseMenuUI;

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {           
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        var scene = FindObjectOfType<SceneSwitcher>();
        scene.MainMenu();
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        var scene = FindObjectOfType<SceneSwitcher>();
        scene.QuitGame();
    }

    public void Resume()
    {
        IsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Pause()
    {
        IsPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }
}
