using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 80f;
    [SerializeField] private float deceleration = 100f;

    [Header("JUMP")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpGravity = 35f;

    [Tooltip("How much the jump is reduced when releasing jump early.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float jumpCutMultiplier = 0.45f;

    [Tooltip("How much stronger gravity becomes while falling.")]
    [SerializeField] private float fallGravityMultiplier = 1.5f;

    [Header("JUMP ASSIST")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("GROUND CHECK")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private float moveInput;

    private bool jumpPressed;
    private bool jumpHeld;

    private float coyoteCounter;
    private float jumpBufferCounter;

    private bool isGrounded;

    private void Awake()
{
    rb = GetComponent<Rigidbody2D>();

    // We control gravity ourselves
    rb.gravityScale = 0f;

    // Prevent the player from tunneling through platforms
    rb.collisionDetectionMode =
        CollisionDetectionMode2D.Continuous;

    // Prevent unwanted rotation
    rb.constraints =
        RigidbodyConstraints2D.FreezeRotation;
}

    private void Update()
    {
        GetInput();
        CheckGround();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void GetInput()
    {
        moveInput = 0f;

        // A = Left
        if (Keyboard.current.aKey.isPressed)
        {
            moveInput = -1f;
        }

        // D = Right
        if (Keyboard.current.dKey.isPressed)
        {
            moveInput = 1f;
        }

        // Jump button pressed
        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;

            // Remember jump input briefly
            jumpBufferCounter = jumpBufferTime;
        }

        // Jump button held
        jumpHeld =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.spaceKey.isPressed;
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void HandleMovement()
    {
        float targetSpeed = moveInput * moveSpeed;

        float currentSpeed = rb.linearVelocity.x;

        // Player is pressing A or D
        if (moveInput != 0)
        {
            // If changing direction, respond quickly
            if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) &&
                Mathf.Abs(currentSpeed) > 0.1f)
            {
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    targetSpeed,
                    acceleration * 1.5f * Time.fixedDeltaTime
                );
            }
            else
            {
                // Normal acceleration
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    targetSpeed,
                    acceleration * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            // No input = quickly stop
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(
            currentSpeed,
            rb.linearVelocity.y
        );
    }

    // =========================================================
    // JUMP
    // =========================================================

    private void HandleJumpInput()
    {
        // Countdown jump buffer
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Countdown coyote time
        if (!isGrounded)
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Jump if we pressed jump recently
        // and are allowed to jump
        if (jumpBufferCounter > 0 &&
            coyoteCounter > 0)
        {
            PerformJump();

            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            jumpPressed = false;
        }

        // Release jump early
        if (!jumpHeld &&
            rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }

        jumpPressed = false;
    }

    private void PerformJump()
    {
        // Calculate the exact velocity needed
        // to reach the desired jump height.
        float jumpVelocity =
            Mathf.Sqrt(
                2f *
                jumpGravity *
                jumpHeight
            );

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpVelocity
        );
    }

    // =========================================================
    // GRAVITY
    // =========================================================

    private void HandleGravity()
{
    // If we are standing on the ground,
    // completely stop downward velocity.
    if (isGrounded && rb.linearVelocity.y <= 0)
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        return;
    }

    float gravity = jumpGravity;

    // Falling = stronger gravity
    if (rb.linearVelocity.y < 0)
    {
        gravity *= fallGravityMultiplier;
    }

    rb.linearVelocity +=
        Vector2.down *
        gravity *
        Time.fixedDeltaTime;
}

    // =========================================================
    // GROUND CHECK
    // =========================================================

    private void CheckGround()
    {
        bool previousGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Just landed or currently grounded
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }

        // Just walked off a platform
        if (previousGrounded && !isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}