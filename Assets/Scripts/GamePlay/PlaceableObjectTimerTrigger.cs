using UnityEngine;

public class PlaceableObjectTimerTrigger : MonoBehaviour
{
    private PlaceableObjectTimer placeableObjectTimer;

    private DraggableObject draggableObject;

    private LevelManager levelManager;

    private void Awake()
    {
        placeableObjectTimer =
            GetComponentInParent<PlaceableObjectTimer>();

        draggableObject =
            GetComponentInParent<DraggableObject>();

        levelManager =
            FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // Only react to Player.
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // =====================================================
        // ONLY WORK DURING PUZZLE PHASE
        // =====================================================

        if (
            levelManager != null &&
            !levelManager.IsPuzzlePhase()
        )
        {
            return;
        }

        // =====================================================
        // ONLY START IF OBJECT IS PLACED
        // =====================================================

        if (
            draggableObject != null &&
            !draggableObject.IsPlacedOnGrid()
        )
        {
            return;
        }

        // =====================================================
        // START TIMER
        // =====================================================

        if (placeableObjectTimer != null)
        {
            placeableObjectTimer.StartTimer();
        }
    }
}   