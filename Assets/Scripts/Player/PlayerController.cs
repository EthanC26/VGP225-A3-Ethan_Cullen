using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Movement Settings")]
    public float initSpeed = 5f;
    public float maxSpeed = 15f;
    public float moveAccel = 0.2f;
    public float rotationSpeed = 30f;

    [Header("Jump Settings")]
    public float jumpHeight = 0.1f;
    public float jumpTime = 0.7f;

    [Header("References")]
    public Transform cameraTransform;

    // internal state
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    private Vector2 moveInput;
    private Vector3 velocity;
    private float curSpeed;

    private bool jumpPressed;
    private bool sprinting;

    // jump physics
    private float gravity;
    private float timeToJumpApex;
    private float initJumpVelocity;

    // knockback
    public Vector3 externalForces;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new InputSystem_Actions();
        inputActions.Player.SetCallbacks(this);

        // calculate jump physics
        timeToJumpApex = jumpTime / 2f;
        gravity = (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        initJumpVelocity = -(gravity * timeToJumpApex);

        curSpeed = initSpeed;
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        Vector3 desiredMoveDirection = ProjectedMoveDirection(moveInput);

        // ----------------------------------------------------------
        // MOVEMENT SPEED WITH ACCELLERATION
        // ----------------------------------------------------------
        if (moveInput == Vector2.zero)
            curSpeed = initSpeed;                // reset speed if not moving
        else
        {
            curSpeed += moveAccel * Time.deltaTime;
            curSpeed = Mathf.Clamp(curSpeed, initSpeed, maxSpeed);
        }

        // horizontal x/z velocity
        velocity.x = desiredMoveDirection.x * curSpeed;
        velocity.z = desiredMoveDirection.z * curSpeed;

        // ----------------------------------------------------------
        // APPLY EXTERNAL FORCES (knockback)
        // ----------------------------------------------------------
        velocity += externalForces;

        // ----------------------------------------------------------
        // JUMP / GRAVITY
        // ----------------------------------------------------------
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = jumpPressed ? initJumpVelocity : -controller.minMoveDistance;
            jumpPressed = false;
        }

        // ----------------------------------------------------------
        // MOVE THE PLAYER
        // ----------------------------------------------------------
        controller.Move(velocity * Time.deltaTime);

        // fade external forces
        externalForces = Vector3.Lerp(externalForces, Vector3.zero, 5f * Time.deltaTime);

        // ----------------------------------------------------------
        // ROTATION
        // ----------------------------------------------------------
        Vector3 flatDir = new Vector3(desiredMoveDirection.x, 0f, desiredMoveDirection.z);

        if (flatDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // convert input to camera-relative movement
    private Vector3 ProjectedMoveDirection(Vector2 input)
    {
        Vector3 camRight = cameraTransform.right;
        Vector3 camForward = cameraTransform.forward;

        camRight.y = 0;
        camForward.y = 0;

        camRight.Normalize();
        camForward.Normalize();

        return camForward * input.y + camRight * input.x;
    }

    // external knockback from enemy hit
    public void AddKnockBack(Vector3 force)
    {
        externalForces += force;
    }

    // INPUT CALLBACKS ---------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
        => moveInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
        => jumpPressed = context.ReadValueAsButton();

    public void OnSprint(InputAction.CallbackContext context)
        => sprinting = context.ReadValue<float>() > 0;

    // unused callbacks
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
}
