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

    private SpriteRenderer spriteRenderer;

    private bool isPressed;

    private float activationTimer;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        SetPressed(false);
    }

    private void Update()
    {
        if (!isPressed)
            return;

        // Count down activation time.
        activationTimer -=
            Time.deltaTime;

        if (
            activationTimer <= 0f
        )
        {
            SetPressed(false);
        }
    }

    // =========================================================
    // PLAYER TOUCHES BUTTON
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (
            other.CompareTag("Player")
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
        // If already active,
        // don't activate again.
        if (isPressed)
            return;

        isPressed = true;

        // Start timer.
        activationTimer =
            activeDuration;

        // Change color.
        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                pressedColor;
        }

        // Open door.
        if (connectedDoor != null)
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

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                isPressed
                ? pressedColor
                : normalColor;
        }

        if (connectedDoor != null)
        {
            connectedDoor.SetOpen(
                isPressed
            );
        }
    }
}