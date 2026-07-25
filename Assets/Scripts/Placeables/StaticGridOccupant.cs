using UnityEngine;

/// Reserves grid cells for a level-design object placed directly in
/// the scene rather than through PlaceableDragHandler (e.g. a
/// pre-existing button or door). Reads the object's actual collider
/// bounds to find which cell it occupies — this works no matter
/// where the object's transform pivot is, since it never assumes
/// the pivot is centered or aligned to anything.
[RequireComponent(typeof(GridFootprint))]
public class StaticGridOccupant : MonoBehaviour
{
    [Tooltip("The collider that defines this object's footprint. Defaults to the first collider found on this object or its children if left empty.")]
    [SerializeField] private Collider2D referenceCollider;

    private GridFootprint footprint;
    private Vector2Int gridPosition;
    private bool isRegistered;

    private void Start()
    {
        footprint = GetComponent<GridFootprint>();

        if (referenceCollider == null)
            referenceCollider = GetComponentInChildren<Collider2D>();

        if (referenceCollider == null)
        {
            Debug.LogError($"{name}: needs a Collider2D (on itself or a child) to determine its grid cell.");
            return;
        }

        if (GridManager.Instance == null)
        {
            Debug.LogError($"{name}: GridManager.Instance is missing, cannot register static occupant.");
            return;
        }

        // Bottom-left corner of the actual collider, in world space —
        // this is the one point we can be certain of without knowing
        // anything about the object's pivot.
        Vector2 bottomLeftWorld = referenceCollider.bounds.min;

        gridPosition = GridManager.Instance.WorldToGrid(bottomLeftWorld);
        GridManager.Instance.PlaceObject(gridPosition, footprint);
        isRegistered = true;

        // Debug.Log($"{name}: registered as occupying grid cell {gridPosition}.");
    }

    private void OnDestroy()
    {
        if (isRegistered && GridManager.Instance != null)
            GridManager.Instance.RemoveObject(gridPosition, footprint);
    }
}