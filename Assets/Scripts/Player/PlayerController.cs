using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControl = 0.8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private PlayerInputActions inputActions;

    private Vector2 moveInput;

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;

        inputActions.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        CheckGround();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleBetterGravity();
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed;

        float currentAcceleration;

        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            currentAcceleration = acceleration;
        }
        else
        {
            currentAcceleration = deceleration;
        }

        // Less control while in the air
        if (!isGrounded)
        {
            currentAcceleration *= airControl;
        }

        float newVelocityX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            currentAcceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newVelocityX,
            rb.linearVelocity.y
        );
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        // Cut jump short when player releases jump
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    private void HandleBetterGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}