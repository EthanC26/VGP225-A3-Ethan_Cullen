using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    private float jumpHeight = 2f;
    private float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool sprinting;
    private bool jumpPressed;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        inputActions.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleMovement();
        //ApplyGravity();


        if (!controller.isGrounded)
        {
            Debug.Log("not gorunded");
        }
    }

    private void HandleMovement()
    {
        // Horizontal movement (scaled by deltaTime)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        move *= speed * Time.deltaTime; // ✅ apply deltaTime here

        // Jump
        if (jumpPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal and vertical
        Vector3 finalMove = move;
        finalMove.y = velocity.y * Time.deltaTime;

        // Move the character
        controller.Move(finalMove);
    }



    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // --- Input Callbacks ---
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move input received");
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Implement camera look here if needed
    }

    public void OnJump(InputAction.CallbackContext context) => jumpPressed = context.ReadValueAsButton();

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprinting = context.ReadValue<float>() > 0;
    }

    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
}
