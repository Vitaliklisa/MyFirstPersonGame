using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
   
    // run and walk
    
    public float speed_Move;
    float x_Move;
    float z_Move;
    CharacterController player;
    Vector3 move_Direction;
    public float speed_Run;
    public float speed_Current;
    // crouching
    public float initialHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTimer = 1f;
    //jump 
    public float gravity = -9.8f;
    public float jumpHeight = 3f;
    // Start is called before the first frame update
    void Start()
    {
       player = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }
    // receive the inputs for our InputManager.cs and apply them to our character controller.
    public void Move()
    {
        x_Move = Input.GetAxis("Horizontal");
        z_Move = Input.GetAxis("Vertical");
        player.Move(move_Direction * speed_Move * Time.deltaTime);
        if (player.isGrounded)
        {
            move_Direction = new Vector3(x_Move, 0f, z_Move);
            move_Direction = transform.TransformDirection(move_Direction);
        }
        move_Direction.y -= 1;
    }
    public void Jump()
    {
        if(isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(-jumpHeight * gravity);
        }
    }
}
