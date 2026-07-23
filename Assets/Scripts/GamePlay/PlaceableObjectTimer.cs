using UnityEngine;
using TMPro;

public class PlaceableObjectTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("How many seconds the player has to use this object.")]
    public float lifetime = 5f;

    [Tooltip("If false, the timer will not start.")]
    public bool timerEnabled = true;

    [Header("Timer UI")]
    [Tooltip("TMP text used to display the countdown.")]
    public TMP_Text timerText;

    [Header("Visuals")]
    [Tooltip("The child object's SpriteRenderer whose color changes.")]
    public SpriteRenderer timerVisual;

    [Header("Colors")]
    public Color startColor = Color.blue;
    public Color endColor = Color.red;

    // =========================================================
    // INTERNAL STATE
    // =========================================================

    private float remainingTime;

    private bool timerStarted;

    private bool objectUsed;

    private DraggableObject draggableObject;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        remainingTime =
            lifetime;

        draggableObject =
            GetComponent<DraggableObject>();

        // Initial visual state.
        UpdateVisual();

        // Initial timer text.
        UpdateTimerText();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // Timer disabled.
        if (!timerEnabled)
        {
            return;
        }

        // Timer hasn't started yet.
        if (!timerStarted)
        {
            return;
        }

        // Object has already been used.
        if (objectUsed)
        {
            return;
        }

        // =====================================================
        // CHECK IF OBJECT IS ON GRID
        // =====================================================

        if (
            draggableObject != null &&
            !draggableObject.IsPlacedOnGrid()
        )
        {
            // Object is not placed yet.
            // Timer should not run.
            return;
        }

        // =====================================================
        // COUNT DOWN
        // =====================================================

        remainingTime -=
            Time.deltaTime;

        if (remainingTime < 0f)
        {
            remainingTime = 0f;
        }

        // =====================================================
        // UPDATE VISUALS
        // =====================================================

        UpdateVisual();

        UpdateTimerText();

        // =====================================================
        // DESTROY OBJECT
        // =====================================================

        if (remainingTime <= 0f)
        {
            DestroyObject();
        }
    }

    // =========================================================
    // START TIMER
    // =========================================================

    public void StartTimer()
    {
        if (!timerEnabled)
        {
            return;
        }

        // Don't start if already running.
        if (timerStarted)
        {
            return;
        }

        // Don't start if object has already been used.
        if (objectUsed)
        {
            return;
        }

        // =====================================================
        // IMPORTANT
        // ONLY START IF OBJECT IS PLACED ON GRID
        // =====================================================

        if (
            draggableObject != null &&
            !draggableObject.IsPlacedOnGrid()
        )
        {
            return;
        }

        timerStarted =
            true;

        remainingTime =
            lifetime;

        UpdateVisual();

        UpdateTimerText();

        Debug.Log(
            gameObject.name +
            " TIMER STARTED!"
        );
    }

    // =========================================================
    // STOP TIMER
    // =========================================================

    public void StopTimer()
    {
        if (!timerStarted)
        {
            return;
        }

        timerStarted =
            false;

        Debug.Log(
            gameObject.name +
            " TIMER STOPPED."
        );
    }

    // =========================================================
    // RESET TIMER
    // =========================================================

    public void ResetTimer()
    {
        timerStarted =
            false;

        objectUsed =
            false;

        remainingTime =
            lifetime;

        UpdateVisual();

        UpdateTimerText();
    }

    // =========================================================
    // MARK AS USED
    // =========================================================

    public void MarkAsUsed()
    {
        objectUsed =
            true;

        timerStarted =
            false;

        UpdateTimerText();

        Debug.Log(
            gameObject.name +
            " HAS BEEN USED."
        );
    }

    // =========================================================
    // DESTROY OBJECT
    // =========================================================

    private void DestroyObject()
    {
        Debug.Log(
            gameObject.name +
            " TIMER EXPIRED. OBJECT DESTROYED."
        );

        Destroy(
            gameObject
        );
    }

    // =========================================================
    // UPDATE VISUAL
    // =========================================================

    private void UpdateVisual()
    {
        if (timerVisual == null)
        {
            return;
        }

        if (lifetime <= 0f)
        {
            timerVisual.color =
                endColor;

            return;
        }

        // 1 = full time remaining.
        // 0 = no time remaining.

        float normalizedTime =
            Mathf.Clamp01(
                remainingTime /
                lifetime
            );

        // Blue -> Red
        timerVisual.color =
            Color.Lerp(
                endColor,
                startColor,
                normalizedTime
            );
    }

    // =========================================================
    // UPDATE TIMER TEXT
    // =========================================================

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        // Show two decimal places.
        timerText.text =
            remainingTime.ToString(
                "F2"
            );
    }

    // =========================================================
    // PUBLIC INFORMATION
    // =========================================================

    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }

    public bool TimerStarted
    {
        get
        {
            return timerStarted;
        }
    }

    public bool IsUsed
    {
        get
        {
            return objectUsed;
        }
    }
}