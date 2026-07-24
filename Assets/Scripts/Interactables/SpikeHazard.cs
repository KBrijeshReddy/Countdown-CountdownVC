using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float timePenalty = 5f;

    [Header("Teleport")]
    [SerializeField] private bool teleportPlayer = true;

    [Header("Trigger Cooldown")]
    [SerializeField] private float triggerCooldown = 0.5f;

    private LevelManager levelManager;
    private float cooldownTimer;

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) => TryApplyPenalty(other);
    private void OnTriggerStay2D(Collider2D other) => TryApplyPenalty(other);

    private void TryApplyPenalty(Collider2D other)
    {
        if (!other.CompareTag("Player") || cooldownTimer > 0f)
            return;

        cooldownTimer = triggerCooldown;

        levelManager?.RemoveTime(timePenalty);

        if (!teleportPlayer)
            return;

        PlayerCheckpoint checkpoint = other.GetComponentInParent<PlayerCheckpoint>();

        if (checkpoint != null)
            checkpoint.TeleportToCheckpoint();
        else
            Debug.LogError($"{name}: PlayerCheckpoint not found on '{other.name}' or its parents.");
    }
}