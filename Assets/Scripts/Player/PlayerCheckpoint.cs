using UnityEngine;

[RequireComponent(typeof(GroundSensor))]
public class PlayerCheckpoint : MonoBehaviour
{
    private Rigidbody2D rb;
    private GroundSensor groundSensor;
    private LevelManager levelManager;

    /// The fixed position the player entered this level at. Restart
    /// returns here; hazard teleports use CheckpointPosition instead.
    public Vector3 SpawnPosition { get; private set; }

    public Vector3 CheckpointPosition { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponent<GroundSensor>();

        SpawnPosition = transform.position;
        CheckpointPosition = SpawnPosition;
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
        CheckpointPosition = transform.position;
    }

    public void TeleportToCheckpoint()
    {
        Teleport(CheckpointPosition);
    }

    private void ResetToSpawn()
    {
        CheckpointPosition = SpawnPosition;
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