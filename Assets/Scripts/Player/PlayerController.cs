using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(GroundSensor))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 80f;
    [SerializeField] private float deceleration = 100f;
    [SerializeField] private float damageTimer = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravityStrength = 35f;
    [SerializeField, Range(0.1f, 1f)] private float jumpCutMultiplier = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Jump Feel — Apex Hang")]
    [Tooltip("Vertical speed range around zero considered 'near the apex' of a jump.")]
    [SerializeField] private float apexThreshold = 2f;
    [Tooltip("Gravity is multiplied by this while near the apex, for a brief floaty hang at the top of the jump.")]
    [Range(0.1f, 1f)]
    [SerializeField] private float apexGravityMultiplier = 0.6f;

    [Header("Blocked-Move Feedback")]
    [SerializeField] private float blockedShakeDuration = 0.05f;
    [SerializeField] private float blockedShakeMagnitude = 0.05f;
    [SerializeField] private float blockedShakeCooldown = 0.15f;

    private Rigidbody2D rb;
    private GroundSensor groundSensor;

    private float moveInput;
    private bool jumpHeld;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float blockedShakeTimer;

    private const float GroundStickVelocity = -2f;

    private bool IsBuyPhase => LevelManager.Instance != null && LevelManager.Instance.IsBuyingPhase();
    private float currentDamageTimer = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponent<GroundSensor>();

        rb.gravityScale = 0f; // gravity is fully custom, see ApplyGravity()
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        ReadInput();
        UpdateJumpTimers();
        UpdateBlockedMoveFeedback();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        if (currentDamageTimer < 0.1f)
        {
            ApplyMovement();
        }
        else
        {
            currentDamageTimer -= Time.fixedDeltaTime;
        }
    }

    private void ReadInput()
    {
        var kb = Keyboard.current;

        moveInput = 0f;
        if (kb.aKey.isPressed) moveInput = -1f;
        if (kb.dKey.isPressed) moveInput = 1f;

        if (kb.wKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
            jumpBufferCounter = jumpBufferTime;

        jumpHeld = kb.wKey.isPressed || kb.spaceKey.isPressed;
    }

    private void ApplyMovement()
    {
        if (IsBuyPhase)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float targetSpeed = moveInput * moveSpeed;
        float currentSpeed = rb.linearVelocity.x;
        float rate;

        if (moveInput == 0f)
        {
            targetSpeed = 0f;
            rate = deceleration;
        }
        else
        {
            bool changingDirection =
                Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) &&
                Mathf.Abs(currentSpeed) > 0.1f;

            rate = changingDirection ? acceleration * 1.5f : acceleration;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void UpdateJumpTimers()
    {
        if (IsBuyPhase)
        {
            jumpBufferCounter = 0f;
            return;
        }

        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;

        coyoteCounter = groundSensor.IsGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            PerformJump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        // Variable jump height: cut upward velocity if the button was released early.
        if (!jumpHeld && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
    }

    private void PerformJump()
    {
        float jumpVelocity = Mathf.Sqrt(2f * gravityStrength * jumpHeight);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        AudioManager.Instance?.PlaySFX(SoundId.PlayerJump);
    }

    private void ApplyGravity()
    {
        if (IsBuyPhase)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            return;
        }

        if (groundSensor.IsGrounded && rb.linearVelocity.y <= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, GroundStickVelocity);
            return;
        }

        float gravity = gravityStrength;

        // Brief floaty hang right at the top of the jump, before falling
        // gravity takes over — makes the arc feel less mechanical.
        bool isNearApex = Mathf.Abs(rb.linearVelocity.y) < apexThreshold;

        if (isNearApex)
        {
            gravity *= apexGravityMultiplier;
        }
        else if (rb.linearVelocity.y < 0f)
        {
            gravity *= fallGravityMultiplier;
        }

        rb.linearVelocity += Vector2.down * gravity * Time.fixedDeltaTime;
    }

    private void UpdateBlockedMoveFeedback()
    {
        if (blockedShakeTimer > 0f)
            blockedShakeTimer -= Time.deltaTime;

        if (!IsBuyPhase || blockedShakeTimer > 0f)
            return;

        var kb = Keyboard.current;
        bool tryingToMove = kb.aKey.isPressed || kb.dKey.isPressed || kb.wKey.isPressed || kb.spaceKey.isPressed;
        if (!tryingToMove)
            return;

        CameraShake.Instance?.Shake(blockedShakeDuration, blockedShakeMagnitude);
        blockedShakeTimer = blockedShakeCooldown;
    }

    public void DisablePlayerMovement()
    {
        currentDamageTimer = damageTimer;
    }
}