using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;        // constant speed
    public float rotationSpeed = 30f;

    [Header("Jump Settings")]
    public float jumpHeight = 0.1f;
    public float jumpTime = 0.7f;

    [Header("Health Settings")]
    private int maxHealth = 10;
    private int minHealth = 0;
    public int currentHealth;

    [Header("References")]
    public Transform cameraTransform;

    Animator anim;
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    [Header("AUdio")]
    AudioSource AS;
    public AudioClip jumpSound;
    public AudioClip HopSound;

    private Vector2 moveInput;

    private Vector3 velocity;             // vertical only
    private Vector3 moveVelocity;         // horizontal movement (input)
    private Vector3 knockbackVelocity;    // horizontal knockback, added to movement

    private bool jumpPressed;
    private bool sprinting;

    private float gravity;
    private float timeToJumpApex;
    private float initJumpVelocity;

    public float VerticalVelocity => velocity.y;
    public event System.Action<int> OnHealthChanged;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        AS = GetComponent<AudioSource>();

        inputActions = new InputSystem_Actions();
        inputActions.Player.SetCallbacks(this);

        // calculate jump physics
        timeToJumpApex = jumpTime / 2f;
        gravity = (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        initJumpVelocity = -(gravity * timeToJumpApex);

        // initialize health
        currentHealth = maxHealth;
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        anim.SetBool("Jumping", !controller.isGrounded);
        anim.SetFloat("Velocity", moveInput.magnitude * (sprinting ? 2f : 1f));
        if(jumpPressed && controller.isGrounded)
        {
            AS.PlayOneShot(jumpSound);
        }

        Vector3 desiredMove = ProjectedMoveDirection(moveInput);

        // ---------------------------------------
        // HORIZONTAL PLAYER MOVEMENT (constant speed)
        // ---------------------------------------
        if (moveInput.sqrMagnitude > 0.01f)
            moveVelocity = desiredMove * moveSpeed;
        else
            moveVelocity = Vector3.zero;

        // ---------------------------------------
        // COMBINE MOVEMENT + KNOCKBACK
        // ---------------------------------------
        Vector3 finalHorizontal = moveVelocity + knockbackVelocity;

        velocity.x = finalHorizontal.x;
        velocity.z = finalHorizontal.z;

        // ---------------------------------------
        // GRAVITY + JUMP
        // ---------------------------------------
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = jumpPressed ? initJumpVelocity : -2f;
            jumpPressed = false;
        }

        // ---------------------------------------
        // MOVE
        // ---------------------------------------
        controller.Move(velocity * Time.deltaTime);

        // ---------------------------------------
        // DECAY KNOCKBACK
        // ---------------------------------------
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, 5f * Time.deltaTime);

        // ---------------------------------------
        // ROTATION
        // ---------------------------------------
        if (desiredMove.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

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

    // Knockback from enemy
    public void AddKnockBack(Vector3 force)
    {
        knockbackVelocity = force;
    }

    public void TakeDamage(int damageAmt)
    {
        currentHealth -= damageAmt;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            string sceneName = (SceneManager.GetActiveScene().name.Contains("Level")) ? "GameOver" : "Level";
            SceneManager.LoadScene(sceneName);
        }

        Debug.Log("Player Health: " + currentHealth);
    }

    public void BounceFromEnemy()
    {
        velocity.y = initJumpVelocity * 0.75f; // small bounce
        AS.PlayOneShot(HopSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            string sceneName = (SceneManager.GetActiveScene().name.Contains("Level")) ? "GameOver" : "Level";
            SceneManager.LoadScene(sceneName);
        }
    }

    #region Input Callbacks
    // INPUT CALLBACKS ----------------------------------------------------
    public void OnMove(InputAction.CallbackContext context)
        => moveInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
        => jumpPressed = context.ReadValueAsButton();

    public void OnSprint(InputAction.CallbackContext context)
        => sprinting = context.ReadValue<float>() > 0;

   
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    #endregion
}
