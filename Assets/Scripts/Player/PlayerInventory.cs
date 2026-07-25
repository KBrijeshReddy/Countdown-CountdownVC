using UnityEngine;
using System;

/// Tracks items the player has collected. Lives on the Player so key
/// state persists naturally with the player, not the level.
public class PlayerInventory : MonoBehaviour
{
    public bool HasKey { get; private set; }

    public event Action KeyCollected;
    public event Action InventoryReset;

    private LevelManager levelManager;

    private void OnEnable() => SubscribeToLevelManager();
    private void Start() => SubscribeToLevelManager();

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.LevelRestarted -= ResetInventory;
    }

    private void SubscribeToLevelManager()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;

        if (levelManager == null)
            return;

        levelManager.LevelRestarted -= ResetInventory;
        levelManager.LevelRestarted += ResetInventory;
    }

    public void CollectKey()
    {
        if (HasKey)
            return;

        HasKey = true;
        KeyCollected?.Invoke();
    }

    private void ResetInventory()
    {
        if (!HasKey)
            return;

        HasKey = false;
        InventoryReset?.Invoke();
    }
}