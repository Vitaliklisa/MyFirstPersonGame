using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float walkingSpeed = 5f;
    [SerializeField] float flyingSpeed = 10f;
    [SerializeField] float climbingSpeed = 2f;
    [SerializeField] float mass = 1f;
    [SerializeField] float acceleration = 20f;

    [SerializeField] float worldBottomBoundary = -100f;

    public Transform cameraTransform;

    public bool IsGrounded => controller.isGrounded;

    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    internal float movementSpeedMultiplier;

    public State state;

    public enum State
    {
        Walking,
        Flying
    }

    CharacterController controller;
    internal Vector3 velocity;
    Vector2 look;

    (Vector3, Quaternion) initialPositionAndRotation;

    bool wasGrounded;

    PlayerInput playerInput;
    InputAction MoveAction;
    InputAction lookAction;
    InputAction flyUpDownAction;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        MoveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        flyUpDownAction = playerInput.actions["flyUpDown"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        initialPositionAndRotation = (transform.position, transform.rotation);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
        look.x = rotation.eulerAngles.y;
        look.y = rotation.eulerAngles.z;
        velocity = Vector3.zero;
    }

    void Update()
    {
        movementSpeedMultiplier = 1f;
        switch (state)
        {
            case State.Walking:
                UpdateGround();
                UpdateGravity();
                UpdateMovement();
                UpdateLook();
                CheckBounds();
                break;
            case State.Flying:
                UpdateMovementFlying();
                UpdateLook();
                break;
        }
    }

    void CheckBounds()
    {
        if (transform.position.y < worldBottomBoundary)
        {
            var (position, rotation) = initialPositionAndRotation;
            Teleport(position, rotation);
        }
    }
    void UpdateGround()
    {
        if (wasGrounded != IsGrounded)
        {
            OnGroundStateChange?.Invoke(IsGrounded);
            wasGrounded = IsGrounded;
        }
    }

    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    Vector3 GetMovementInput(float speed, bool horizontal = true)
    {
        var moveInput = MoveAction.ReadValue<Vector2>();
        var flyUpDownInput = flyUpDownAction.ReadValue<float>();
        var input = new Vector3();
        var referenceTransform = horizontal ? transform : cameraTransform;
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        if (!horizontal)
        {
            input += transform.up * flyUpDownInput;
        }
        input = Vector3.ClampMagnitude(input, 1f);
        input *= speed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement()
    {
        OnBeforeMove?.Invoke();

        var input = GetMovementInput(walkingSpeed);

        var factor = acceleration * Time.deltaTime;
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateMovementFlying()
    {
        var input = GetMovementInput(flyingSpeed, false);

        var factor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, factor);
        controller.Move(velocity * Time.deltaTime);
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

    void OnToggleFlying()
    {
        state = state == State.Flying ? State.Walking : State.Flying;
    }
}
