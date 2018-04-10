using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScroll : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<CameraFollow>().offset.y += Input.GetAxis("Mouse ScrollWheel") * 10;
    }
}
