using UnityEngine;

public class PlaceableLifetimeTrigger : MonoBehaviour
{
    [SerializeField] private PlayerTriggerZone triggerZone;
    [SerializeField] private PlaceableLifetime lifetime;

    private void OnEnable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered += OnPlayerEntered;
    }

    private void OnDisable()
    {
        if (triggerZone != null) triggerZone.PlayerEntered -= OnPlayerEntered;
    }

    private void OnPlayerEntered(Collider2D player)
    {
        if (LevelManager.Instance != null && !LevelManager.Instance.IsPuzzlePhase()) return;
        lifetime.StartTimer();
    }
}