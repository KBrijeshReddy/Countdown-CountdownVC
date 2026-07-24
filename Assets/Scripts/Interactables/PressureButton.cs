using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private float activeDuration = 3f;
    [SerializeField] private Door connectedDoor;
    [SerializeField] private PlayerTriggerZone triggerZone;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.green;

    private bool isPressed;
    private float activationTimer;

    private void Awake() => SetPressed(false);

    private void OnEnable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered += OnPlayerEntered;
    }

    private void OnDisable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered -= OnPlayerEntered;
    }

    private void Update()
    {
        if (!isPressed) return;

        activationTimer -= Time.deltaTime;
        if (activationTimer <= 0f)
            SetPressed(false);
    }

    private void OnPlayerEntered(Collider2D player)
    {
        if (isPressed) return;
        activationTimer = activeDuration;
        SetPressed(true);
    }

    private void SetPressed(bool pressed)
    {
        isPressed = pressed;

        if (visual != null)
            visual.color = pressed ? pressedColor : normalColor;

        connectedDoor?.SetOpen(pressed);
    }
}