using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundSensor))]
public class PlayerCheckpoint : MonoBehaviour
{
    [Header("Safety")]
    [Tooltip("Minimum distance a checkpoint must be from any laser beam's path to be considered safe.")]
    [SerializeField] private float laserClearanceRadius = 0.5f;
    [Tooltip("How many recent landings to remember when searching for a safe checkpoint.")]
    [SerializeField] private int maxHistorySize = 8;

    private Rigidbody2D rb;
    private GroundSensor groundSensor;
    private LevelManager levelManager;

    private readonly List<Vector3> checkpointHistory = new List<Vector3>();

    public Vector3 SpawnPosition { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponent<GroundSensor>();

        SpawnPosition = transform.position;
        checkpointHistory.Add(SpawnPosition);
    }

    private void OnEnable()
    {
        groundSensor.Landed += SaveCheckpoint;
        SubscribeToLevelManager();
    }

    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        groundSensor.Landed -= SaveCheckpoint;

        if (levelManager != null)
            levelManager.LevelRestarted -= ResetToSpawn;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelRestarted -= ResetToSpawn;
        levelManager.LevelRestarted += ResetToSpawn;
    }

    private void SaveCheckpoint()
    {
        checkpointHistory.Add(transform.position);

        if (checkpointHistory.Count > maxHistorySize)
            checkpointHistory.RemoveAt(0);
    }

    public void TeleportToCheckpoint()
    {
        Teleport(FindSafeCheckpoint());
    }

    /// Walks backward through recent landings and returns the most
    /// recent one that isn't inside any laser's beam path. Falls back
    /// to spawn if the entire history is somehow unsafe.
    private Vector3 FindSafeCheckpoint()
    {
        for (int i = checkpointHistory.Count - 1; i >= 0; i--)
        {
            if (!IsInsideAnyLaserPath(checkpointHistory[i]))
                return checkpointHistory[i];
        }

        return SpawnPosition;
    }

    private bool IsInsideAnyLaserPath(Vector3 position)
    {
        foreach (LaserEmitter laser in LaserEmitter.All)
        {
            if (laser != null && laser.IsPositionInDangerZone(position, laserClearanceRadius))
                return true;
        }

        return false;
    }

    private void ResetToSpawn()
    {
        checkpointHistory.Clear();
        checkpointHistory.Add(SpawnPosition);

        Teleport(SpawnPosition);
    }

    private void Teleport(Vector3 position)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = position;
        }
        else
        {
            transform.position = position;
        }
    }
}