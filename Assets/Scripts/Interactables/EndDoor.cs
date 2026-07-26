using UnityEngine;
using System.Collections;

public class EndDoor : MonoBehaviour
{
    [Header("Scene To Load")]
    [Tooltip("Leave empty to load the next scene in Build Settings order.")]
    [SerializeField] private string nextSceneName;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color lockedColor = Color.white;
    [SerializeField] private Color unlockedColor = Color.green;

    [Header("Open Animation")]
    [SerializeField] private Animator animatorVisual;
    [SerializeField] private float openDelay = 0.5f;

    private static readonly int EndDoorOpenParam = Animator.StringToHash("EndDoorOpen");

    private bool isOpening;

    private void Awake()
    {
        SetVisual(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpening)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory == null || !inventory.HasKey)
        {
            SetVisual(false);
            return;
        }

        SetVisual(true);
        StartCoroutine(OpenAndCompleteLevel());
    }

    private IEnumerator OpenAndCompleteLevel()
    {
        yield return new WaitForSeconds(openDelay);

    Debug.Log("EndDoor: delay finished, calling CompleteLevel now.");

    if (LevelManager.Instance != null)
        LevelManager.Instance.CompleteLevel(nextSceneName);
    else
        Debug.LogError($"{name}: LevelManager.Instance is missing, cannot complete level.");

        isOpening = true;

        if (animatorVisual != null)
            animatorVisual.SetBool(EndDoorOpenParam, true);

        AudioManager.Instance?.PlaySFX(SoundId.NextLevel);

        yield return new WaitForSeconds(openDelay);

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