using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private float activeDuration = 3f;
    [SerializeField] private Door connectedDoor;
    [SerializeField] private PlayerTriggerZone triggerZone;

    [Header("Visual")]
    [SerializeField] private Animator animatorVisual;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.green;

    private LevelManager levelManager;
    private Collider2D[] ownColliders;

    private bool isPressed;
    private float activationTimer;

    private void Awake()
    {
        ownColliders = GetComponentsInChildren<Collider2D>();
        SetPressed(false);
    }

    private void OnEnable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered += OnPlayerEntered;
        SubscribeToLevelManager();
    }

    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered -= OnPlayerEntered;

        if (levelManager != null)
            levelManager.LevelRestarted -= ResetButton;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelRestarted -= ResetButton;
        levelManager.LevelRestarted += ResetButton;
    }

    private void ResetButton()
    {
        activationTimer = 0f;
        SetPressed(false);
    }

    private void Update()
    {
        if (!isPressed) return;

        activationTimer -= Time.deltaTime;
        if (activationTimer <= 0f)
        {
            animatorVisual.SetBool("ButtonPressed", false);
            animatorVisual.SetBool("ButtonStarted", true);
            SetPressed(false);
            animatorVisual.SetBool("ButtonStarted", false);
        }
    }

    private void OnPlayerEntered(Collider2D player) => Activate();

    /// Presses the button. Public so anything that can "step on" a
    /// button — the player, or a laser beam — can trigger it the same
    /// way. Refreshes the active duration if already pressed.
    public void Activate()
    {
        activationTimer = activeDuration;

        if (!isPressed)
            SetPressed(true);
    }

    private void SetPressed(bool pressed)
    {
        isPressed = pressed;

        if (animatorVisual != null)
        {
            if (pressed)
            {
                animatorVisual.SetBool("ButtonPressed", true);
                animatorVisual.SetBool("ButtonStarted", true);
            }

            if (!pressed)
            {
                animatorVisual.SetBool("ButtonPressed", false);
            }
        }

        connectedDoor?.SetOpen(pressed);
        SetCollidersEnabled(!pressed);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider2D collider in ownColliders)
        {
            if (collider != null)
                collider.enabled = enabled;
        }
    }
}