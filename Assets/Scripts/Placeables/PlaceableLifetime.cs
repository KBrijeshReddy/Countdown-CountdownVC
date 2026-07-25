using UnityEngine;
using TMPro;

public class PlaceableLifetime : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool timerEnabled = true;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Visual")]
    [SerializeField] private Animator animatorVisual;
    [SerializeField] private GameObject destructionParticlesPrefab;
    [SerializeField] private Color startColor = Color.blue;
    [SerializeField] private Color endColor = Color.red;

    private PlaceableDragHandler dragHandler;
    private LevelManager levelManager;

    public float RemainingTime { get; private set; }
    public bool TimerStarted { get; private set; }
    public bool IsUsed { get; private set; }

    private void Awake()
    {
        dragHandler = GetComponent<PlaceableDragHandler>();
        RemainingTime = lifetime;
        UpdateVisual();
        UpdateTimerText();
    }

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDestroy()
    {
        if (levelManager != null)
            levelManager.LevelRestarted -= ResetTimer;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        // Subscribed once and kept alive across break/restart — this object
        // is deactivated rather than destroyed when its timer runs out, so
        // it must still be able to hear the restart event while inactive.
        levelManager.LevelRestarted -= ResetTimer;
        levelManager.LevelRestarted += ResetTimer;
    }

    private void Update()
    {
        if (!timerEnabled || !TimerStarted || IsUsed) return;
        if (dragHandler != null && !dragHandler.IsPlacedOnGrid) return;

        RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);
        UpdateVisual();
        UpdateTimerText();

        if (RemainingTime <= 0f)
            BreakObject();
    }

    public void StartTimer()
    {
        if (!timerEnabled || TimerStarted || IsUsed) return;
        if (dragHandler != null && !dragHandler.IsPlacedOnGrid) return;

        TimerStarted = true;
        RemainingTime = lifetime;

        if (animatorVisual != null)
            animatorVisual.SetBool("TileBreaking", true);

        UpdateVisual();
        UpdateTimerText();
    }

    public void StopTimer() => TimerStarted = false;

    public void MarkAsUsed()
    {
        IsUsed = true;
        TimerStarted = false;
        UpdateTimerText();
    }

    /// Visually and physically "destroys" the object without actually
    /// destroying it, so a level restart can bring it back exactly as
    /// it started.
    private void BreakObject()
    {
        if (animatorVisual != null)
            animatorVisual.SetBool("TileBreaking", false);

        if (destructionParticlesPrefab != null)
            Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }

    /// Called on LevelManager.LevelRestarted. Reactivates the object if
    /// it had broken, and resets its timer back to a fresh, un-started
    /// state either way.
    private void ResetTimer()
    {
        gameObject.SetActive(true);

        TimerStarted = false;
        IsUsed = false;
        RemainingTime = lifetime;

        if (animatorVisual != null)
        {
            animatorVisual.SetBool("TileBreaking", false);
            animatorVisual.SetBool("TileBreaking", false);
        }

        UpdateVisual();
        UpdateTimerText();
    }

    private void UpdateVisual()
    {
        if (timerText == null) return;
        float t = lifetime > 0f ? Mathf.Clamp01(RemainingTime / lifetime) : 0f;
        timerText.color = Color.Lerp(endColor, startColor, t);
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = RemainingTime.ToString("F2");
    }
}