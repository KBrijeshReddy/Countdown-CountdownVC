using UnityEngine;
using System;

public class PlayerTriggerZone : MonoBehaviour
{
    public event Action<Collider2D> PlayerEntered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerEntered?.Invoke(other);
    }
}