using UnityEngine;

[RequireComponent(typeof(GroundSensor))]
public class PlayerCheckpoint : MonoBehaviour
{
    private Rigidbody2D rb;
    private GroundSensor groundSensor;

    public Vector3 CheckpointPosition { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundSensor = GetComponent<GroundSensor>();
        CheckpointPosition = transform.position;
    }

    private void OnEnable() => groundSensor.Landed += SaveCheckpoint;
    private void OnDisable() => groundSensor.Landed -= SaveCheckpoint;

    private void SaveCheckpoint()
    {
        CheckpointPosition = transform.position;
    }

    public void TeleportToCheckpoint()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = CheckpointPosition; // must move the Rigidbody2D itself, not transform
        }
        else
        {
            transform.position = CheckpointPosition;
        }
    }
}