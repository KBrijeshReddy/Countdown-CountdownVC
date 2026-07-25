using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public enum GamePhase
    {
        Buying,
        Puzzle
    }

    [Header("Game Phase")]
    [SerializeField] private GamePhase currentPhase = GamePhase.Buying;

    [Header("Timer")]
    [Tooltip("Starting time used only if no time was carried over from a previous level (i.e. Level 1).")]
    [SerializeField] private float startingTime = 200f;
    [SerializeField] private TMP_Text timerText;

    [Header("Time Carry-Over Reward")]
    [Tooltip("Total time (buying + puzzle) a player is 'expected' to spend on this level.")]
    [SerializeField] private float idealCompletionTime = 30f;
    [Tooltip("Bonus time awarded when the level is finished in exactly the ideal time.")]
    [SerializeField] private float baseBonusTime = 12f;
    [Tooltip("Bonus time can never drop below this, no matter how long the player took.")]
    [SerializeField] private float minimumBonusTime = 6f;
    [Tooltip("How much bonus time is lost per second spent beyond the ideal time (and gained per second under it).")]
    [SerializeField] private float bonusChangePerSecond = 0.5f;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerMovementScript;
    [SerializeField] private Collider2D playerCollider;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;

    private float remainingTime;
    private float levelStartingTime;
    private Coroutine timerRoutine;

    public static LevelManager Instance { get; private set; }

    public float RemainingTime => remainingTime;

    public event Action PuzzlePhaseStarted;

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

        remainingTime = GameSession.Instance != null && GameSession.Instance.HasCarriedTime
            ? GameSession.Instance.CarriedTime
            : startingTime;

        // Snapshot the exact amount this attempt began with. This is the
        // single source of truth for both "how much did the player spend"
        // (on success) and "how much to restore" (on timeout).
        levelStartingTime = remainingTime;

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

        PuzzlePhaseStarted?.Invoke();

        timerRoutine = StartCoroutine(RunTimer());
    }

    /// Called by EndDoor when the player reaches it with the key.
    /// Computes the time-based bonus, hands the resulting total off to
    /// GameSession, and loads the next level.
    public void CompleteLevel(string nextSceneName = null)
    {
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        float timeSpent = levelStartingTime - remainingTime;
        float bonus = CalculateBonusTime(timeSpent);
        float carryOverTime = remainingTime + bonus;

        EnsureGameSession();
        GameSession.Instance.SetCarriedTime(carryOverTime);

        LoadScene(nextSceneName);
    }

    private float CalculateBonusTime(float timeSpent)
    {
        float differenceFromIdeal = timeSpent - idealCompletionTime;
        float bonus = baseBonusTime - differenceFromIdeal * bonusChangePerSecond;

        return Mathf.Max(minimumBonusTime, bonus);
    }

    private IEnumerator RunTimer()
    {
        while (remainingTime > 0f)
        {
            yield return null;

            remainingTime = Mathf.Max(
                0f,
                remainingTime - Time.unscaledDeltaTime
            );

            UpdateTimerUI();
        }

        RespawnSameLevel();
    }

    public bool IsBuyingPhase()
    {
        return currentPhase == GamePhase.Buying;
    }

    public bool IsPuzzlePhase()
    {
        return currentPhase == GamePhase.Puzzle;
    }

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
        if (amount < 0f)
            return;

        remainingTime += amount;
        UpdateTimerUI();
    }

    public void RemoveTime(float amount)
    {
        if (amount < 0f)
            return;

        remainingTime = Mathf.Max(
            0f,
            remainingTime - amount
        );

        UpdateTimerUI();
    }

    private void SetPlayerMovement(bool active)
    {
        if (playerMovementScript != null &&
            !playerMovementScript.enabled)
        {
            playerMovementScript.enabled = true;
        }
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
        {
            timerText.text = Mathf.CeilToInt(
                remainingTime
            ).ToString();
        }
    }

    /// Time ran out — reload this same level with the amount of time
    /// it originally started with, as if the attempt never happened.
    private void RespawnSameLevel()
    {
        EnsureGameSession();
        GameSession.Instance.SetCarriedTime(levelStartingTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            Debug.LogWarning($"{name}: No next scene in Build Settings to load.");
    }

    private void EnsureGameSession()
    {
        if (GameSession.Instance != null)
            return;

        var sessionObject = new GameObject("GameSession");
        sessionObject.AddComponent<GameSession>();
    }
}