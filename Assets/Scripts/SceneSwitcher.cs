using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public float fadeTimer = 0f;
    public bool fading_OUT = false;
    public bool fading_IN = false;
    private string sceneToLoad = "";
    public Material fadeMat;



    
    // This function is called from the UI button, with scene name as parameter
    public void fadedLoadScene(string targetScene)
    {
        sceneToLoad = targetScene;
        fadeTimer = 0f;
        fading_OUT = true;
    }

    void Update()
    {
        // fade to black (leaving scene)
        if (fading_OUT)
        {
            fadeTimer += Time.deltaTime;
            Color col = Color.black;
            col.a = fadeTimer;
            fadeMat.color = col;
        }
        // 1 second passed while fading out (now 100% black) - start actual scene load.
        if (fading_OUT && fadeTimer >= 1f)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            fading_OUT = false;
        }

        // fade to transparent (entering scene)
        if (fading_IN)
        {
            fadeTimer -= Time.deltaTime;
            Color col = Color.black;
            col.a = fadeTimer;
            fadeMat.color = col;
        }
        // 1 second passed while fading in (now 100% transparent) - scene load is done.
        if (fading_IN && fadeTimer <= 0f)
        {
            fading_IN = false;
        }


    }

    // Scene was loaded, fade in:
    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{

    void Start() { 
        Debug.Log("bla");
        fading_IN = true;
        fadeTimer = 1f;
    }
}
