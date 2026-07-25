using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Beam")]
    [Tooltip("Beam fires along this object's local right (+X) axis. Rotate the GameObject to aim it.")]
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Cycle")]
    [SerializeField] private bool startActive = true;
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float inactiveDuration = 2f;

    [Header("Damage")]
    [SerializeField] private float timePenalty = 10f;
    [Tooltip("Minimum time between damage ticks while the player stands in the beam.")]
    [SerializeField] private float damageTickInterval = 0.5f;

    private LineRenderer lineRenderer;
    private LevelManager levelManager;

    private bool isActive;
    private float cycleTimer;
    private float damageTickTimer;

    private bool CanFire => levelManager != null && levelManager.IsPuzzlePhase();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        levelManager = FindFirstObjectByType<LevelManager>();

        isActive = startActive;
        cycleTimer = isActive ? activeDuration : inactiveDuration;

        SetBeamVisible(false);
    }

    private void Update()
    {
        if (!CanFire)
        {
            SetBeamVisible(false);
            return;
        }

        if (damageTickTimer > 0f)
            damageTickTimer -= Time.deltaTime;

        UpdateCycle();

        if (isActive)
            UpdateBeam();
    }

    private void UpdateCycle()
    {
        cycleTimer -= Time.deltaTime;

        if (cycleTimer > 0f)
            return;

        isActive = !isActive;
        cycleTimer = isActive ? activeDuration : inactiveDuration;

        SetBeamVisible(isActive);
    }

    private void UpdateBeam()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        LayerMask combinedMask = obstacleLayer | playerLayer;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxRange, combinedMask);

        Vector2 endPoint = hit.collider != null ? hit.point : origin + direction * maxRange;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
            TryDamagePlayer(hit.collider);
    }

    private void TryDamagePlayer(Collider2D playerCollider)
    {
        if (damageTickTimer > 0f)
            return;

        damageTickTimer = damageTickInterval;

        levelManager?.RemoveTime(timePenalty);

        PlayerCheckpoint checkpoint = playerCollider.GetComponentInParent<PlayerCheckpoint>();

        if (checkpoint != null)
            checkpoint.TeleportToCheckpoint();
        else
            Debug.LogError($"{name}: PlayerCheckpoint not found on '{playerCollider.name}' or its parents.");
    }

    private void SetBeamVisible(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * maxRange);
    }
}