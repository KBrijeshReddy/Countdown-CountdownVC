using UnityEngine;

public class ButtonObject : MonoBehaviour
{
    [Header("Activation Time")]
    public float activeDuration = 3f;

    [Header("Door")]
    public DoorObject connectedDoor;

    [Header("Visual")]
    public Color normalColor = Color.white;

    public Color pressedColor = Color.green;

    // =========================================================
    // REFERENCES
    // =========================================================

    private SpriteRenderer spriteRenderer;

    // =========================================================
    // BUTTON STATE
    // =========================================================

    private bool isPressed;

    private float activationTimer;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        SetPressed(false);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // Nothing to do if button isn't active.
        if (!isPressed)
        {
            return;
        }

        // Count down activation time.
        activationTimer -=
            Time.deltaTime;

        // =====================================================
        // TIMER FINISHED
        // =====================================================

        if (
            activationTimer <= 0f
        )
        {
            SetPressed(false);
        }
    }

    // =========================================================
    // PLAYER TOUCHES DETECTION AREA
    // =========================================================

    public void PlayerEnteredDetection(
        Collider2D playerCollider
    )
    {
        if (
            playerCollider == null
        )
        {
            return;
        }

        // Make sure only the Player
        // can activate the button.
        if (
            playerCollider.CompareTag(
                "Player"
            )
        )
        {
            ActivateButton();
        }
    }

    // =========================================================
    // ACTIVATE BUTTON
    // =========================================================

    private void ActivateButton()
    {
        // Already active.
        if (isPressed)
        {
            return;
        }

        // Activate.
        isPressed =
            true;

        // Start countdown.
        activationTimer =
            activeDuration;

        // Change Button color.
        if (
            spriteRenderer != null
        )
        {
            spriteRenderer.color =
                pressedColor;
        }

        // Open Door.
        if (
            connectedDoor != null
        )
        {
            connectedDoor.SetOpen(
                true
            );
        }
    }

    // =========================================================
    // DEACTIVATE BUTTON
    // =========================================================

    private void SetPressed(
        bool pressed
    )
    {
        isPressed =
            pressed;

        // Update Button visual.
        if (
            spriteRenderer != null
        )
        {
            spriteRenderer.color =
                isPressed
                ? pressedColor
                : normalColor;
        }

        // Update Door.
        if (
            connectedDoor != null
        )
        {
            connectedDoor.SetOpen(
                isPressed
            );
        }
    }
}