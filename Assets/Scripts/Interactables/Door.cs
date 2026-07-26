using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color closedColor = Color.white;
    [SerializeField] private Color openColor = Color.green;

    [Header("Collider")]
    [SerializeField] private Collider2D doorCollider;

    private LevelManager levelManager;

    private bool isOpen;

    private void Awake() => SetOpen(false);

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.LevelRestarted -= ResetDoor;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelRestarted -= ResetDoor;
        levelManager.LevelRestarted += ResetDoor;
    }

    private void ResetDoor() => SetOpen(false);

    public void SetOpen(bool open)
    {
        if (open != isOpen)
        AudioManager.Instance?.PlaySFX(open ? SoundId.DoorOpen : SoundId.DoorClose);

        isOpen = open;

        if (visual != null)
            visual.color = open ? openColor : closedColor;

        if (doorCollider != null)
            doorCollider.enabled = !open;
    }
}