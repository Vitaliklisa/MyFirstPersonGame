using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor2 : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    // run and walk
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] float runSpeed = 8f;
    CharacterController player;
    Vector3 move_Direction;

    public float speed_Current;

    // crouching
    public float initialHeight = 1.8f;
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



    public void ProcessMo(Vector2 input)
    {
        float x_Move = Input.GetAxis("Horizontal");
        float z_Move = Input.GetAxis("Vertical");
        player.Move(move_Direction * movementSpeed * Time.deltaTime);
        if (player.isGrounded)
        {
            move_Direction = new Vector3(x_Move, 0f, z_Move);
            move_Direction = transform.TransformDirection(move_Direction);
        }
        move_Direction.y -= 1;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed_Current = runSpeed;
        }
        else
        {
            speed_Current = movementSpeed;

            player.Move(move_Direction * movementSpeed * Time.deltaTime);
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(-jumpHeight * gravity);
        }
    }
}

