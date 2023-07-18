using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;
    [SerializeField] Transform cameraTransform;

    CharacterController controller;
    Vector3 velocity;
    Vector2 look;

    PlayerInput playerInput;
    InputAction MoveAction;
    InputAction lookAction;
    InputAction jumpAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        MoveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGravity();
        UpdateMovement();
        UpdateLook();
    }

    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    Vector3 GetMovementInput()
    {
        var moveInput = MoveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed;
        return input;
    }

    void UpdateMovement()
    {
        var input = GetMovementInput();

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        var jumpInput = jumpAction.ReadValue<float>();
        if (jumpInput > 0 && controller.isGrounded)
        {
            velocity.y += jumpSpeed;
        }

        controller.Move(velocity* Time.deltaTime);
    }

    void UpdateLook()
    { 
        var lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

}
