using UnityEngine;

[RequireComponent(typeof(GridFootprint))]
public class PlacementRule : MonoBehaviour
{
    public enum RuleType { None, RequiresFloorSupport, RequiresAnySupport }

    [SerializeField] private RuleType ruleType = RuleType.None;
    [SerializeField] private LayerMask supportLayer;
    [SerializeField] private float raycastMargin = 0.05f;

    private GridFootprint footprint;
    private Collider2D[] ownColliders;

    public SurfaceSupport.Side LastDetectedSide { get; private set; } = SurfaceSupport.Side.None;

    private void Awake()
    {
        footprint = GetComponent<GridFootprint>();

        // Includes colliders on child objects (DragCollider, PlayerDetection, etc.)
        // so the support raycast can never register a false hit against itself.
        ownColliders = GetComponentsInChildren<Collider2D>();
    }

    public bool IsPlacementValid(Vector2Int bottomLeftCell, GridManager gridManager)
    {
        if (ruleType == RuleType.None)
        {
            LastDetectedSide = SurfaceSupport.Side.None;
            return true;
        }

        Vector2 worldCenter = gridManager.ObjectGridToWorld(bottomLeftCell, footprint);
        Vector2 footprintWorldSize = new Vector2(footprint.Size.x, footprint.Size.y) * gridManager.CellSize;

        LastDetectedSide = DetectSupportIgnoringSelf(worldCenter, footprintWorldSize);

        if (LastDetectedSide == SurfaceSupport.Side.None)
            return false;

        if (ruleType == RuleType.RequiresFloorSupport)
            return LastDetectedSide == SurfaceSupport.Side.Floor;

        return true; // RequiresAnySupport
    }

    private SurfaceSupport.Side DetectSupportIgnoringSelf(Vector2 worldCenter, Vector2 footprintWorldSize)
    {
        SetOwnCollidersEnabled(false);

        SurfaceSupport.Side result =
            SurfaceSupport.DetectSupport(worldCenter, footprintWorldSize, supportLayer, raycastMargin);

        SetOwnCollidersEnabled(true);

        return result;
    }

    private void SetOwnCollidersEnabled(bool enabled)
    {
        foreach (Collider2D collider in ownColliders)
        {
            if (collider != null)
                collider.enabled = enabled;
        }
    }
}