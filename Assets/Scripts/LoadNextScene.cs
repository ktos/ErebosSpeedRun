using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextScene : MonoBehaviour
{
    public GameObject sceneSwitcher;

    void OnTriggerEnter(Collider other)
    {
        sceneSwitcher.GetComponent<SceneSwitcher>().FadedLoadScene("Scene01");
    }
}
