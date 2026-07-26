using UnityEngine;

public class EndDoor : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("Leave empty to load the next scene in Build Settings order.")]
    [SerializeField] private string nextSceneName;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color lockedColor = Color.white;
    [SerializeField] private Color unlockedColor = Color.green;

    private void Awake()
    {
        SetVisual(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null || !inventory.HasKey)
        {
            SetVisual(false);
            return;
        }

        SetVisual(true);

        AudioManager.Instance?.PlaySFX(SoundId.NextLevel);

        if (LevelManager.Instance != null)
            LevelManager.Instance.CompleteLevel(nextSceneName);
        else
            Debug.LogError($"{name}: LevelManager.Instance is missing, cannot complete level.");
    }

    private void SetVisual(bool unlocked)
    {
        if (visual != null)
            visual.color = unlocked ? unlockedColor : lockedColor;
    }
}