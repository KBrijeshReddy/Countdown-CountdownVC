using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("How much time is removed when the player touches the spikes.")]
    public float timePenalty = 5f;

    [Header("Teleport")]
    [Tooltip("Should the player be returned to their last safe position?")]
    public bool teleportPlayer = true;

    [Header("Trigger Cooldown")]
    [Tooltip("Prevents the spike from triggering repeatedly in rapid succession.")]
    public float triggerCooldown = 0.5f;

    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private float cooldownTimer;

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -=
                Time.deltaTime;
        }
    }

    // =========================================================
    // PLAYER ENTERS SPIKE
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // =====================================================
        // CHECK PLAYER
        // =====================================================

        if (
            !other.CompareTag("Player")
        )
        {
            return;
        }

        // =====================================================
        // CHECK COOLDOWN
        // =====================================================

        if (
            cooldownTimer > 0f
        )
        {
            return;
        }

        cooldownTimer =
            triggerCooldown;


        // =====================================================
        // FIND LEVEL MANAGER
        // =====================================================

        LevelManager levelManager =
            FindFirstObjectByType<LevelManager>();


        // =====================================================
        // REMOVE TIME
        // =====================================================

        if (
            levelManager != null
        )
        {
            levelManager.RemoveTime(
                timePenalty
            );

            Debug.Log(
                "SPIKE: Removed " +
                timePenalty +
                " seconds."
            );
        }


        // =====================================================
        // FIND PLAYER SAFE POSITION
        // =====================================================

        PlayerSafePosition safePosition =
            other.GetComponentInParent<PlayerSafePosition>();


        // =====================================================
        // TELEPORT PLAYER
        // =====================================================

        if (
            teleportPlayer
        )
        {
            if (
                safePosition != null
            )
            {
                Debug.Log(
                    "SPIKE: Teleporting player to safe position."
                );

                safePosition.TeleportToLastSafePosition();
            }
            else
            {
                Debug.LogError(
                    "SPIKE ERROR: PlayerSafePosition component was not found on the Player or its parent!"
                );
            }
        }


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "PLAYER HIT SPIKE!"
        );
    }
}