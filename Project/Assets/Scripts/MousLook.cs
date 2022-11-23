using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MousLook : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    public Camera camera;
    

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
       if (isLocalPlayer)
        {
        
        }
        
    }
}
