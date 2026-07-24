using UnityEngine;
using UnityEngine.SceneManagement;

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
        LoadNextScene();
    }

    private void SetVisual(bool unlocked)
    {
        if (visual != null)
            visual.color = unlocked ? unlockedColor : lockedColor;
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            Debug.LogWarning($"{name}: No next scene in Build Settings to load.");
    }
}