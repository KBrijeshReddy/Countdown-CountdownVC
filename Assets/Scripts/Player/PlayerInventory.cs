using UnityEngine;
using System;

/// Tracks items the player has collected. Lives on the Player so key
/// state persists naturally with the player, not the level.
public class PlayerInventory : MonoBehaviour
{
    public bool HasKey { get; private set; }

    public event Action KeyCollected;

    public void CollectKey()
    {
        if (HasKey)
            return;

        HasKey = true;
        KeyCollected?.Invoke();
    }
}