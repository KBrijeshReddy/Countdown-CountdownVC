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

    [Header("Landing Squash")]
    [Tooltip("How wide/flat the squash goes at its strongest point (1 = no squash).")]
    [SerializeField] private float squashScaleX = 1.2f;
    [SerializeField] private float squashScaleY = 0.8f;
    [Tooltip("How long the squash takes to fully recover back to normal scale.")]
    [SerializeField] private float squashRecoverDuration = 0.12f;

    private Animator animator;
    private Rigidbody2D rb;
    private GroundSensor groundSensor;

    private Vector3 baseScale;
    private float squashTimer;
    private bool isSquashing;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        groundSensor = GetComponentInParent<GroundSensor>();

        if (rb == null || groundSensor == null)
            Debug.LogError($"{name}: PlayerAnimator needs Rigidbody2D and GroundSensor on a parent object.");

        // Preserves whatever left/right facing flip was already applied
        // to localScale.x, so squash math never overwrites facing.
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (groundSensor != null)
            groundSensor.Landed += PlayLandingSquash;
    }

    private void OnDisable()
    {
        if (groundSensor != null)
            groundSensor.Landed -= PlayLandingSquash;
    }

    private void Update()
    {
        if (rb == null || groundSensor == null)
            return;

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(SpeedParam, horizontalSpeed);
        animator.SetBool(IsGroundedParam, groundSensor.IsGrounded);

        UpdateFacing(rb.linearVelocity.x);
        UpdateSquash();

        bool isWalking = groundSensor.IsGrounded && horizontalSpeed > movementThresholdToFlip;
        AudioManager.Instance?.SetLoopingSound(SoundId.PlayerWalk, isWalking);
    }

    private void UpdateFacing(float horizontalVelocity)
    {
        if (Mathf.Abs(horizontalVelocity) < movementThresholdToFlip)
            return;

        float facingSign = horizontalVelocity > 0f ? 1f : -1f;
        baseScale.x = facingSign * Mathf.Abs(baseScale.x);
    }

    private void PlayLandingSquash()
    {
        isSquashing = true;
        squashTimer = 0f;
        AudioManager.Instance?.PlaySFX(SoundId.PlayerLand);
    }

    private void UpdateSquash()
    {
        if (!isSquashing)
        {
            transform.localScale = baseScale;
            return;
        }

        squashTimer += Time.deltaTime;
        float t = Mathf.Clamp01(squashTimer / squashRecoverDuration);

        // Starts at the squashed extreme, eases back to normal scale.
        float easedT = 1f - Mathf.Pow(1f - t, 3f); // ease-out cubic

        float scaleX = Mathf.Lerp(squashScaleX, 1f, easedT);
        float scaleY = Mathf.Lerp(squashScaleY, 1f, easedT);

        transform.localScale = new Vector3(baseScale.x * scaleX, baseScale.y * scaleY, baseScale.z);

        if (t >= 1f)
            isSquashing = false;
    }
}