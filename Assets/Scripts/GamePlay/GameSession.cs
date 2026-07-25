using UnityEngine;

/// Persists across scene loads to carry the player's remaining time
/// into the next level, and to preserve the correct restart amount
/// if a level times out. Holds no game logic — just the handoff value.
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public bool HasCarriedTime { get; private set; }
    public float CarriedTime { get; private set; }

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
}