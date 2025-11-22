using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float rotationSpeed = 1f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

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

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        Vector3 move = ProjectedMoveDirection(moveInput);

        // Horizontal movement
        float speed = sprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 horizontalMove = move * speed;

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

        // Move the character
        controller.Move((horizontalMove + velocity) * Time.deltaTime);

        // Rotate player to face movement direction
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Project input direction relative to camera
    private Vector3 ProjectedMoveDirection(Vector2 direction)
    {
        Vector3 camRight = cameraTransform.right;
        Vector3 camForward = cameraTransform.forward;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        return camForward * direction.y + camRight * direction.x;
    }

    // --- Input Callbacks ---
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) => jumpPressed = context.ReadValueAsButton();
    public void OnSprint(InputAction.CallbackContext context) => sprinting = context.ReadValue<float>() > 0;

    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
}
