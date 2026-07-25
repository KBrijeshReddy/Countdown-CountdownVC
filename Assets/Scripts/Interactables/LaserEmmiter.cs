using System.Collections.Generic;
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

    private static readonly List<LaserEmitter> activeEmitters = new List<LaserEmitter>();

    /// All LaserEmitters currently in the scene, regardless of their
    /// on/off cycle state — used by PlayerCheckpoint to avoid saving
    /// or teleporting into a beam's path.
    public static IReadOnlyList<LaserEmitter> All => activeEmitters;

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

    private void OnEnable() => activeEmitters.Add(this);
    private void OnDisable() => activeEmitters.Remove(this);

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

    if (hit.collider == null)
        return;

    if (hit.collider.CompareTag("Player"))
    {
        TryDamagePlayer(hit.collider);
        return;
    }

    PressureButton button = hit.collider.GetComponentInParent<PressureButton>();
    button?.Activate();
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

    /// True if the given position lies within this beam's geometric
    /// path — computed against obstacles only, independent of whether
    /// the beam is currently visually on or off, since the cycle will
    /// bring it back regardless.
    public bool IsPositionInDangerZone(Vector2 position, float clearance)
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxRange, obstacleLayer);
        float beamLength = hit.collider != null ? hit.distance : maxRange;
        Vector2 endPoint = origin + direction * beamLength;

        return DistancePointToSegment(position, origin, endPoint) <= clearance;
    }

    private static float DistancePointToSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 segment = b - a;
        float sqrLength = segment.sqrMagnitude;

        if (sqrLength < 0.0001f)
            return Vector2.Distance(point, a);

        float t = Mathf.Clamp01(Vector2.Dot(point - a, segment) / sqrLength);
        Vector2 closestPoint = a + t * segment;

        return Vector2.Distance(point, closestPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * maxRange);
    }
}