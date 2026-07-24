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
    [SerializeField] private SpriteRenderer timerVisual;
    [SerializeField] private Animator animatorVisual;
    [SerializeField] private GameObject destructionParticlesPrefab; 
    [SerializeField] private Color startColor = Color.blue;
    [SerializeField] private Color endColor = Color.red;

    private PlaceableDragHandler dragHandler;

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

    private void Update()
    {
        if (!timerEnabled || !TimerStarted || IsUsed) return;
        if (dragHandler != null && !dragHandler.IsPlacedOnGrid) return;

        RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);
        UpdateVisual();
        UpdateTimerText();

        if (RemainingTime <= 0f){
            animatorVisual.SetBool("TileBreaking", false);
            Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            // Destroy(destructionParticlesPrefab);
        }    
    }

    public void StartTimer()
    {
        if (!timerEnabled || TimerStarted || IsUsed) return;
        if (dragHandler != null && !dragHandler.IsPlacedOnGrid) return;

        TimerStarted = true;
        RemainingTime = lifetime;
        animatorVisual.SetBool("TileBreaking", true);
        UpdateVisual();
        UpdateTimerText();
    }

    public void StopTimer() => TimerStarted = false;

    public void ResetTimer()
    {
        TimerStarted = false;
        IsUsed = false;
        RemainingTime = lifetime;
        UpdateVisual();
        UpdateTimerText();
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        TimerStarted = false;
        UpdateTimerText();
    }

    private void UpdateVisual()
    {
        if (timerVisual == null) return;
        float t = lifetime > 0f ? Mathf.Clamp01(RemainingTime / lifetime) : 0f;
        timerVisual.color = Color.Lerp(endColor, startColor, t);
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = RemainingTime.ToString("F2");
    }
}