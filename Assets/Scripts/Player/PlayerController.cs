using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 7f;
    public float acceleration = 40f;
    public float deceleration = 50f;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float fallGravityMultiplier = 2f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private float moveInput;
    private bool jumpPressed;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // =========================
        // MOVEMENT INPUT
        // =========================

        if (Keyboard.current.aKey.isPressed)
        {
            moveInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            moveInput = 1f;
        }
        else
        {
            moveInput = 0f;
        }


        // =========================
        // JUMP INPUT
        // =========================

        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }

        jumpHeld =
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.spaceKey.isPressed;


        // =========================
        // VARIABLE JUMP
        // =========================

        // Release jump early = shorter jump
        if (!jumpHeld && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
    }


    // =========================
    // MOVEMENT
    // =========================

    private void HandleMovement()
    {
        float targetSpeed = moveInput * maxSpeed;

        float speedChange;

        if (moveInput != 0)
        {
            speedChange = acceleration;
        }
        else
        {
            speedChange = deceleration;
        }

        float newSpeed = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            speedChange * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newSpeed,
            rb.linearVelocity.y
        );
    }


    // =========================
    // JUMP
    // =========================

    private void HandleJump()
    {
        if (jumpPressed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        jumpPressed = false;
    }


    // =========================
    // GRAVITY
    // =========================

    private void HandleGravity()
    {
        // When falling, smoothly increase gravity
        // instead of instantly changing velocity.

        if (rb.linearVelocity.y < 0)
        {
            float extraGravity =
                Physics2D.gravity.y *
                (fallGravityMultiplier - 1);

            rb.AddForce(
                Vector2.up * extraGravity,
                ForceMode2D.Force
            );
        }
    }


    // =========================
    // GROUND CHECK
    // =========================

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}