using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceFinished : MonoBehaviour
{
    public static bool RaceFinishedProperly;
    public SceneSwitcher sceneSwitcher;
    public Movement player;

    private void Start()
    {
        RaceFinishedProperly = false;
    }

    private void OnDie(object sender, System.EventArgs e)
    {
        player.PlayerTooLow -= OnDie;
        RaceFinishedProperly = false;
        sceneSwitcher.FadedLoadScene("Ending");
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
        player.PlayerTooLow += OnDie;
    }

    private void OnDisable()
    {
        Checkpoint.RaceFinished -= OnRaceFinished;
        player.PlayerTooLow -= OnDie;
    }
}
