using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextSceneTrigger : MonoBehaviour
{     
    void OnTriggerEnter(Collider other)
    {
        var sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        if (sceneSwitcher != null)
            sceneSwitcher.FadedLoadScene("Scene01");
    }
}
