using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public enum GamePhase
    {
        Buying,
        Puzzle,
        LevelComplete
    }

    [Header("Level Settings")]
    [SerializeField] private float startingTime = 200f;

    [Header("Current State")]
    [SerializeField] private GamePhase currentPhase =
        GamePhase.Buying;

    private float remainingTime;

    private bool timeOutHandled;

    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }

    public GamePhase CurrentPhase
    {
        get
        {
            return currentPhase;
        }
    }

    private void Awake()
    {
        remainingTime =
            startingTime;

        currentPhase =
            GamePhase.Buying;

        timeOutHandled =
            false;

        Time.timeScale =
            1f;
    }

    private void Update()
    {
        // Timer does NOT run during buying.
        if (
            currentPhase !=
            GamePhase.Puzzle
        )
        {
            return;
        }

        // Prevent timer from running
        // after time has already reached zero.
        if (timeOutHandled)
        {
            return;
        }

        remainingTime -=
            Time.deltaTime;

        if (
            remainingTime <= 0f
        )
        {
            remainingTime =
                0f;

            TimeRanOut();
        }
    }

    // =========================================================
    // START PUZZLE
    // =========================================================

    public void StartPuzzle()
    {
        // Can only start once.
        if (
            currentPhase !=
            GamePhase.Buying
        )
        {
            return;
        }

        currentPhase =
            GamePhase.Puzzle;

        Debug.Log(
            "PUZZLE STARTED"
        );
    }

    // =========================================================
    // SPEND TIME
    // =========================================================

    public bool SpendTime(
        float amount
    )
    {
        // Never spend negative time.
        if (
            amount <= 0f
        )
        {
            return true;
        }

        // Cannot spend more than available.
        if (
            remainingTime < amount
        )
        {
            return false;
        }

        remainingTime -=
            amount;

        return true;
    }

    // =========================================================
    // ADD TIME
    // =========================================================

    public void AddTime(
        float amount
    )
    {
        if (
            amount <= 0f
        )
        {
            return;
        }

        remainingTime +=
            amount;
    }

    // =========================================================
    // TIME RAN OUT
    // =========================================================

    private void TimeRanOut()
    {
        if (timeOutHandled)
        {
            return;
        }

        timeOutHandled =
            true;

        Debug.Log(
            "TIME RAN OUT"
        );

        SceneManager.LoadScene(
            SceneManager.GetActiveScene()
                .buildIndex
        );
    }

    // =========================================================
    // COMPLETE LEVEL
    // =========================================================

    public void CompleteLevel()
    {
        if (
            currentPhase !=
            GamePhase.Puzzle
        )
        {
            return;
        }

        currentPhase =
            GamePhase.LevelComplete;
    }

    // =========================================================
    // PHASE CHECKS
    // =========================================================

    public bool IsBuyingPhase()
    {
        return currentPhase ==
               GamePhase.Buying;
    }

    public bool IsPuzzlePhase()
    {
        return currentPhase ==
               GamePhase.Puzzle;
    }

    public bool IsLevelComplete()
    {
        return currentPhase ==
               GamePhase.LevelComplete;
    }
}