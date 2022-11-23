using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isDebugMode = false;
    public float speed = 12f;
    public Vector3 cameraOffset;
    public float gravity = -9.81f;
    public float jumpHeight = 3f; 
    public bool isGrounded;
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    public GameObject arm;
   





    private float invRotX = 0f;
    
    Vector3 velocity;

    private void Start()
    {
        if(isLocalPlayer)
        {
            
            Camera.main.transform.SetParent(arm.transform);
            Camera.main.transform.localPosition = cameraOffset;
            Camera.main.transform.rotation = Quaternion.identity;
            
            
        }
        
        


        
    }



    void Update()
    {
        if (!isLocalPlayer)
        {
           return;
            
        }
       
            
        if (isDebugMode)
        {
            Camera.main.transform.localPosition = cameraOffset;
        }

        if (isLocalPlayer)
        {


            


            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            float rotX = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float rotY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            transform.Rotate(0, rotY, 0);

            invRotX = rotX * -1;

            if (arm.transform.rotation.x >= 0.5 && invRotX > 0)
            {
                invRotX = 0f;
            }
            else if (arm.transform.rotation.x <= -0.5 && invRotX < 0)
            {
                invRotX = 0f;
            }


            

            
           
            
            arm.transform.Rotate(invRotX, 0, 0);
            playerBody.transform.Rotate(Vector3.up * invRotX);
            



        }

    }
}
