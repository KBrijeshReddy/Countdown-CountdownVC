using UnityEngine;

/// Drives the Animator and left/right facing for the player's visual
/// child, based on physics state read from the parent. Kept separate
/// from PlayerController since this is purely visual, not physics.
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Facing")]
    [Tooltip("The walking animation faces right by default, so moving left mirrors this transform.")]
    [SerializeField] private float movementThresholdToFlip = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;
    private GroundSensor groundSensor;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        groundSensor = GetComponentInParent<GroundSensor>();

        if (rb == null || groundSensor == null)
            Debug.LogError($"{name}: PlayerAnimator needs Rigidbody2D and GroundSensor on a parent object.");
    }

    private void Update()
    {
        if (rb == null || groundSensor == null)
            return;

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        Debug.Log(horizontalSpeed);

        animator.SetFloat(SpeedParam, horizontalSpeed);
        animator.SetBool(IsGroundedParam, groundSensor.IsGrounded);

        UpdateFacing(rb.linearVelocity.x);
    }

    private void UpdateFacing(float horizontalVelocity)
    {
        if (Mathf.Abs(horizontalVelocity) < movementThresholdToFlip)
            return;

        Vector3 scale = transform.localScale;
        float facingSign = horizontalVelocity > 0f ? 1f : -1f;

        scale.x = facingSign * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}