using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Movement : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

    public event EventHandler PlayerTooLow;

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButton("Fire1"))
            {                
                moveDirection *= speed * 2;
            }
            else
            {
                moveDirection *= speed;
            }


            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                //GetComponent<Animator>().SetTrigger("Jump");
            }

        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        float rotspeed = Input.GetAxis("Vertical") == 0 ? 300.0f : 200.0f;

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * rotspeed;
        controller.transform.Rotate(new Vector3(0, x, 0));

        GetComponent<Animator>().SetBool("MakeWalk", Input.GetAxis("Vertical") != 0);

        if (Input.GetKeyUp(KeyCode.C))
        {
            var camera = FindObjectOfType(typeof(CameraFollow)) as CameraFollow;
            camera.overview = !camera.overview;

            //GameObject.FindGameObjectsWithTag("Ceiling").ToList().ForEach(z => { z.GetComponent<MeshRenderer>().enabled = !camera.overview; });
            RenderSettings.ambientIntensity = camera.overview ? 1.5f : 0;
        }

        if (controller.transform.position.y < -5)
        {
            PlayerTooLow?.Invoke(this, null);
        }
    }

    void Start()
    {

    }
}
