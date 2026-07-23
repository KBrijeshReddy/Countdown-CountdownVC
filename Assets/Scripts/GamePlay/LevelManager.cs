using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    // =========================================================
    // GAME PHASE
    // =========================================================

    public enum GamePhase
    {
        Buying,
        Puzzle
    }

    [Header("Game Phase")]
    public GamePhase currentPhase =
        GamePhase.Buying;


    // =========================================================
    // TIMER
    // =========================================================

    [Header("Timer")]

    public float startingTime = 200f;

    private float remainingTime;

    private Coroutine timerCoroutine;


    // =========================================================
    // TIMER UI
    // =========================================================

    [Header("Timer UI")]

    public TMP_Text timerText;


    // =========================================================
    // START BUTTON
    // =========================================================

    [Header("Start Button")]

    public Button startButton;


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]

    public GameObject player;

    [Header("Player Movement Script")]

    public MonoBehaviour playerMovementScript;

    [Header("Player Collider")]

    public Collider2D playerCollider;


    // =========================================================
    // GRID
    // =========================================================

    [Header("Grid")]

    public GridManager gridManager;


    // =========================================================
    // REMAINING TIME
    // =========================================================

    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        Debug.Log(
            "LEVEL MANAGER AWAKE"
        );
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Start in Buying Phase.
        currentPhase =
            GamePhase.Buying;

        // Set starting time.
        remainingTime =
            startingTime;

        // Update timer display.
        UpdateTimerUI();


        // =====================================================
        // PLAYER
        // =====================================================

        // IMPORTANT:
        // Keep PlayerController ENABLED.
        //
        // PlayerController needs to remain active so it can
        // detect the player attempting to move during the
        // Buy Phase and trigger the camera shake.
        //
        // PlayerController itself will prevent actual
        // movement while IsBuyingPhase() is true.

        SetPlayerMovement(
            true
        );


        // Disable player collider.
        SetPlayerCollider(
            false
        );


        // =====================================================
        // GRID
        // =====================================================

        // Show grid during Buy Phase.
        SetGridVisual(
            true
        );


        // =====================================================
        // START BUTTON
        // =====================================================

        if (
            startButton != null
        )
        {
            startButton.interactable =
                true;
        }


        Debug.Log(
            "LEVEL MANAGER STARTED"
        );
    }


    // =========================================================
    // START PUZZLE
    // =========================================================

    public void StartPuzzle()
    {
        // Prevent starting twice.
        if (
            currentPhase ==
            GamePhase.Puzzle
        )
        {
            return;
        }


        Debug.Log(
            "START PUZZLE FUNCTION CALLED"
        );


        // =====================================================
        // CHANGE PHASE
        // =====================================================

        currentPhase =
            GamePhase.Puzzle;


        Debug.Log(
            "CURRENT PHASE IS NOW: " +
            currentPhase
        );


        // =====================================================
        // PLAYER MOVEMENT
        // =====================================================

        // Keep PlayerController ENABLED.
        //
        // It will now automatically allow movement because
        // IsBuyingPhase() returns false.

        SetPlayerMovement(
            true
        );


        // =====================================================
        // PLAYER COLLIDER
        // =====================================================

        // Enable player collider.
        SetPlayerCollider(
            true
        );


        // =====================================================
        // GRID VISUAL
        // =====================================================

        // Hide grid when puzzle begins.
        SetGridVisual(
            false
        );


        // =====================================================
        // START BUTTON
        // =====================================================

        if (
            startButton != null
        )
        {
            startButton.interactable =
                false;
        }


        // =====================================================
        // START TIMER
        // =====================================================

        if (
            timerCoroutine != null
        )
        {
            StopCoroutine(
                timerCoroutine
            );
        }

        timerCoroutine =
            StartCoroutine(
                RunTimer()
            );


        Debug.Log(
            "TIMER COROUTINE STARTED"
        );
    }


    // =========================================================
    // TIMER
    // =========================================================

    private IEnumerator RunTimer()
    {
        while (
            remainingTime > 0f
        )
        {
            // Wait one real-time frame.
            yield return null;


            // Subtract real frame time.
            remainingTime -=
                Time.unscaledDeltaTime;


            // Prevent negative values.
            if (
                remainingTime < 0f
            )
            {
                remainingTime =
                    0f;
            }


            // Update timer UI.
            UpdateTimerUI();
        }


        // =====================================================
        // TIME RAN OUT
        // =====================================================

        Debug.Log(
            "TIME RAN OUT"
        );


        remainingTime =
            0f;


        UpdateTimerUI();


        // Restart level.
        RestartLevel();
    }


    // =========================================================
    // BUYING PHASE CHECK
    // =========================================================

    public bool IsBuyingPhase()
    {
        return currentPhase ==
            GamePhase.Buying;
    }


    // =========================================================
    // PUZZLE PHASE CHECK
    // =========================================================

    public bool IsPuzzlePhase()
    {
        return currentPhase ==
            GamePhase.Puzzle;
    }


    // =========================================================
    // SPEND TIME
    // =========================================================

    public bool SpendTime(
        float amount
    )
    {
        if (
            amount < 0f
        )
        {
            return false;
        }


        if (
            remainingTime < amount
        )
        {
            return false;
        }


        remainingTime -=
            amount;


        UpdateTimerUI();


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
            amount < 0f
        )
        {
            return;
        }


        remainingTime +=
            amount;


        UpdateTimerUI();
    }


    // =========================================================
    // PLAYER MOVEMENT
    // =========================================================

    public void SetPlayerMovement(
        bool active
    )
    {
        if (
            playerMovementScript != null
        )
        {
            // IMPORTANT:
            // The PlayerController should remain enabled.
            //
            // The PlayerController itself decides whether
            // movement is allowed based on the current phase.
            //
            // Therefore, we intentionally do NOT disable
            // playerMovementScript here.

            if (
                !playerMovementScript.enabled
            )
            {
                playerMovementScript.enabled =
                    true;
            }
        }
    }


    // =========================================================
    // PLAYER COLLIDER
    // =========================================================

    private void SetPlayerCollider(
        bool active
    )
    {
        if (
            playerCollider != null
        )
        {
            playerCollider.enabled =
                active;
        }
    }


    // =========================================================
    // GRID VISUAL
    // =========================================================

    private void SetGridVisual(
        bool visible
    )
    {
        if (
            gridManager != null
        )
        {
            gridManager.showGridVisual =
                visible;
        }
    }


    // =========================================================
    // UPDATE TIMER UI
    // =========================================================

    private void UpdateTimerUI()
    {
        if (
            timerText == null
        )
        {
            return;
        }


        int seconds =
            Mathf.CeilToInt(
                remainingTime
            );


        timerText.text =
            seconds.ToString();
    }


    // =========================================================
    // RESTART LEVEL
    // =========================================================

    private void RestartLevel()
    {
        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.buildIndex
        );
    }
}