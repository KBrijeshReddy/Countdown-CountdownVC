using UnityEngine;

/// Persists across scene loads to carry the player's remaining time
/// into the next level, preserve the correct restart amount on
/// timeout, and accumulate total time spent across the whole run for
/// the win screen. Holds no game logic — just this handoff data.
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public bool HasCarriedTime { get; private set; }
    public float CarriedTime { get; private set; }

    public float TotalTimeSpent { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCarriedTime(float time)
    {
        CarriedTime = Mathf.Max(0f, time);
        HasCarriedTime = true;
    }

    /// Adds this level's actual time spent (buy + puzzle) to the
    /// running total shown on the win screen.
    public void AddTimeSpent(float amount)
    {
        TotalTimeSpent += Mathf.Max(0f, amount);
    }
}