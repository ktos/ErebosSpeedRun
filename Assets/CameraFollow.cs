using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool overview = true;

    public GameObject target;

    private Vector3 overviewOffset = new Vector3(0, 13f, 0);
    private Quaternion overviewRotation;

    public Vector3 offset;

    void Start()
    {        
        offset = overviewOffset;
        overviewRotation = transform.rotation;
        overview = false;
    }
    
    void LateUpdate()
    {
        var damping = 2;

        if (!overview)
        {
            transform.rotation = target.transform.rotation;
            transform.position = target.transform.position;
        }
        else
        {
            transform.rotation = overviewRotation;
            transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime * damping);
        }               
    }
}
