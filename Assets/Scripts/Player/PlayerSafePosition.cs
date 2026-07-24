using UnityEngine;

public class PlayerSafePosition : MonoBehaviour
{
    [Header("Safe Position")]
    [Tooltip("The last position where the player landed on a new surface.")]
    public Vector3 lastSafePosition;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.15f;

    [SerializeField] private LayerMask groundLayer;

    // =========================================================
    // COMPONENTS
    // =========================================================

    private Rigidbody2D rb;

    // =========================================================
    // GROUND STATE
    // =========================================================

    private bool isGrounded;

    private bool wasGrounded;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();

        // The player's starting position
        // is automatically the first safe position.
        lastSafePosition =
            transform.position;

        // We don't know the ground state yet.
        isGrounded =
            false;

        wasGrounded =
            false;

        Debug.Log(
            "SAFE POSITION INITIALIZED AT: " +
            lastSafePosition
        );
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        CheckGround();

        DetectNewLanding();
    }

    // =========================================================
    // GROUND CHECK
    // =========================================================

    private void CheckGround()
    {
        if (
            groundCheck == null
        )
        {
            return;
        }

        isGrounded =
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
    }

    // =========================================================
    // DETECT NEW LANDING
    // =========================================================

    private void DetectNewLanding()
    {
        // Player was NOT grounded before.
        // Player IS grounded now.
        //
        // This means the player has just landed
        // on a new surface.

        if (
            !wasGrounded &&
            isGrounded
        )
        {
            SaveSafePosition();
        }

        // Remember current state
        // for the next frame.

        wasGrounded =
            isGrounded;
    }

    // =========================================================
    // SAVE SAFE POSITION
    // =========================================================

    private void SaveSafePosition()
    {
        lastSafePosition =
            transform.position;

        Debug.Log(
            "NEW SAFE POSITION SAVED: " +
            lastSafePosition
        );
    }

    // =========================================================
    // TELEPORT TO LAST SAFE POSITION
    // =========================================================

    public void TeleportToLastSafePosition()
    {
        Debug.Log(
            "TELEPORT REQUEST RECEIVED!"
        );

        Debug.Log(
            "SAFE POSITION IS: " +
            lastSafePosition
        );

        // =====================================================
        // STOP MOVEMENT
        // =====================================================

        if (
            rb != null
        )
        {
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity =
                0f;
        }

        // =====================================================
        // TELEPORT
        // =====================================================

        transform.position =
            lastSafePosition;

        // =====================================================
        // RESET GROUND STATE
        // =====================================================

        wasGrounded =
            false;

        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "PLAYER TELEPORTED SUCCESSFULLY!"
        );
    }

    // =========================================================
    // RESET SAFE POSITION
    // =========================================================

    public void ResetSafePosition()
    {
        lastSafePosition =
            transform.position;

        wasGrounded =
            isGrounded;

        Debug.Log(
            "SAFE POSITION RESET: " +
            lastSafePosition
        );
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (
            groundCheck == null
        )
        {
            return;
        }

        Gizmos.color =
            Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}