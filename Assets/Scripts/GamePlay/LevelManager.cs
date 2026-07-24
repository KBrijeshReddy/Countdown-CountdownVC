using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public enum GamePhase { Buying, Puzzle }

    [Header("Game Phase")]
    [SerializeField] private GamePhase currentPhase = GamePhase.Buying;

    [Header("Timer")]
    [SerializeField] private float startingTime = 200f;
    [SerializeField] private TMP_Text timerText;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private Collider2D playerCollider;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;

    private float remainingTime;
    private Coroutine timerRoutine;

    public float RemainingTime => remainingTime;

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentPhase = GamePhase.Buying;
        remainingTime = startingTime;
        UpdateTimerUI();

        SetPlayerMovement(true);
        SetPlayerCollider(false);
        SetGridVisual(true);

        if (startButton != null)
            startButton.interactable = true;
    }

    public void StartPuzzle()
    {
        if (currentPhase == GamePhase.Puzzle)
            return;

        currentPhase = GamePhase.Puzzle;

        SetPlayerMovement(true);
        SetPlayerCollider(true);
        SetGridVisual(false);

        if (startButton != null)
            startButton.interactable = false;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (remainingTime > 0f)
        {
            yield return null;

            remainingTime = Mathf.Max(0f, remainingTime - Time.unscaledDeltaTime);
            UpdateTimerUI();
        }

        RestartLevel();
    }

    public bool IsBuyingPhase() => currentPhase == GamePhase.Buying;
    public bool IsPuzzlePhase() => currentPhase == GamePhase.Puzzle;

    /// Spends time only if enough remains. Used for purchases.
    public bool SpendTime(float amount)
    {
        if (amount < 0f || remainingTime < amount)
            return false;

        remainingTime -= amount;
        UpdateTimerUI();
        return true;
    }

    public void AddTime(float amount)
    {
        if (amount < 0f) return;

        remainingTime += amount;
        UpdateTimerUI();
    }

    /// Removes time unconditionally, clamped to zero. Used for hazard penalties.
    public void RemoveTime(float amount)
    {
        if (amount < 0f) return;

        remainingTime = Mathf.Max(0f, remainingTime - amount);
        UpdateTimerUI();
    }

    private void SetPlayerMovement(bool active)
    {
        // PlayerController stays enabled at all times and gates its own
        // movement internally based on game phase — this only guards
        // against it having been left disabled some other way.
        if (playerMovementScript != null && !playerMovementScript.enabled)
            playerMovementScript.enabled = true;
    }

    private void SetPlayerCollider(bool active)
    {
        if (playerCollider != null)
            playerCollider.enabled = active;
    }

    private void SetGridVisual(bool visible)
    {
        if (gridManager != null)
            gridManager.ShowGridVisual = visible;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}