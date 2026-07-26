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
    [SerializeField] private float idealCompletionTime = 30f;
    [SerializeField] private float baseBonusTime = 12f;
    [SerializeField] private float minimumBonusTime = 6f;
    [SerializeField] private float bonusChangePerSecond = 0.5f;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    [Header("Player")]
    [SerializeField] private PlayerController playerMovementScript;
    [SerializeField] private Collider2D playerCollider;

    [Header("Grid")]
    [SerializeField] private GridManager gridManager;

    [Header("Music")]
    [SerializeField] private AudioClip levelMusic;

    private float remainingTime;
    private float levelStartingTime;
    private float puzzlePhaseStartingTime;
    private Coroutine timerRoutine;

    public static LevelManager Instance { get; private set; }

    public float RemainingTime => remainingTime;

    public event Action PuzzlePhaseStarted;

    /// Raised when the player uses the restart button. Sends the level
/// back to the end of Buy Phase — doors, buttons, breakables, and the
/// key all reset, and the player respawns — but placed objects are
/// left untouched so the player can rework their layout.
public event Action LevelRestarted;

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
        if (levelMusic != null)
    AudioManager.Instance?.PlayMusic(levelMusic);

        currentPhase = GamePhase.Buying;

        remainingTime = GameSession.Instance != null && GameSession.Instance.HasCarriedTime
            ? GameSession.Instance.CarriedTime
            : startingTime;

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

        // Snapshot exactly how much time exists the moment the puzzle
        // begins — this is what a restart returns the player to.
        puzzlePhaseStartingTime = remainingTime;

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

    /// Sends the player back to the end of Buy Phase: timer restores to
/// what it was right after buying finished, the player respawns, and
/// doors/buttons/breakables/key all reset via LevelRestarted. Already
/// placed objects are left exactly where they are — GridManager is
/// never touched — so the player can freely rearrange, sell, or add
/// to their existing placements before starting the puzzle again.
public void RestartLevel()
{
    if (!IsPuzzlePhase())
        return;

    if (timerRoutine != null)
    {
        StopCoroutine(timerRoutine);
        timerRoutine = null;
    }

    currentPhase = GamePhase.Buying;
    remainingTime = puzzlePhaseStartingTime;
    UpdateTimerUI();

    SetPlayerCollider(false);
    SetGridVisual(true);

    if (startButton != null)
        startButton.interactable = true;

    LevelRestarted?.Invoke();
}

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
        playerMovementScript.DisablePlayerMovement();
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

    private void RespawnSameLevel()
    {
        EnsureGameSession();
        GameSession.Instance.SetCarriedTime(levelStartingTime);
        AudioManager.Instance?.PlaySFX(SoundId.Death);

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