﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public float fadeTimer = 0f;
    public bool fadingOut = false;
    public bool fadingIn = false;
    private string sceneToLoad = "";
    public Material fadeMat;
    public Image image;

    // This function is called from the UI button, with scene name as parameter
    public void FadedLoadScene(string targetScene)
    {
        sceneToLoad = targetScene;
        fadeTimer = 0f;
        fadingOut = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        FadedLoadScene("MainMenu");
    }

    void Update()
    {
        // fade to black (leaving scene)
        if (fadingOut)
        {
            fadeTimer += Time.deltaTime;
            Color col = Color.black;
            col.a = fadeTimer;
            fadeMat.color = col;

            if (image != null)
                image.color = col;
        }
        // 1 second passed while fading out (now 100% black) - start actual scene load.
        if (fadingOut && fadeTimer >= 1f)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            fadingOut = false;
        }

        // fade to transparent (entering scene)
        if (fadingIn)
        {
            fadeTimer -= Time.deltaTime;
            Color col = Color.black;
            col.a = fadeTimer;
            fadeMat.color = col;

            if (image != null)
                image.color = col;
        }
        // 1 second passed while fading in (now 100% transparent) - scene load is done.
        if (fadingIn && fadeTimer <= 0f)
        {
            fadingIn = false;
        }

        if (!fadingIn && image != null)
        {
            image.gameObject.SetActive(false);
        }
    }

    // Scene was loaded, fade in:
    void Start()
    {
        fadingIn = true;
        fadeTimer = 1f;

        if (image != null)
        {
            image.gameObject.SetActive(true);
        }
    }
}
