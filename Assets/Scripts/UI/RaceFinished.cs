using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceFinished : MonoBehaviour
{
    public static bool RaceFinishedProperly;
    public SceneSwitcher sceneSwitcher;

    private void Start()
    {
        RaceFinishedProperly = false;
    }

    private void OnRaceFinished()
    {
        Debug.Log("Race Finished");
        RaceFinishedProperly = true;
        sceneSwitcher.FadedLoadScene("Ending");
    }

    private void OnEnable()
    {
        Checkpoint.RaceFinished += OnRaceFinished;
    }

    private void OnDisable()
    {
        Checkpoint.RaceFinished -= OnRaceFinished;
    }
}
