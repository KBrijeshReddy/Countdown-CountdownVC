using UnityEngine;

[RequireComponent(typeof(GridFootprint))]
public class PlacementRule : MonoBehaviour
{
    public enum RuleType
    {
        None,
        RequiresFloorSupport,
        RequiresAnySupport,
        RequiresFloorAndCeilingSupport
    }

    [SerializeField] private RuleType ruleType = RuleType.None;
    [SerializeField] private LayerMask supportLayer;
    [SerializeField] private float raycastMargin = 0.05f;

    private GridFootprint footprint;
    private Collider2D[] ownColliders;

    public SurfaceSupport.Side LastDetectedSide { get; private set; } = SurfaceSupport.Side.None;

    private void Awake()
    {
        footprint = GetComponent<GridFootprint>();
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

        SetOwnCollidersEnabled(false);
        bool isValid = EvaluateRule(worldCenter, footprintWorldSize);
        SetOwnCollidersEnabled(true);

        return isValid;
    }

    private bool EvaluateRule(Vector2 worldCenter, Vector2 footprintWorldSize)
    {
        if (ruleType == RuleType.RequiresFloorAndCeilingSupport)
        {
            bool supported = SurfaceSupport.HasFloorAndCeilingSupport(worldCenter, footprintWorldSize, supportLayer, raycastMargin);
            LastDetectedSide = supported ? SurfaceSupport.Side.Floor : SurfaceSupport.Side.None;
            return supported;
        }

        LastDetectedSide = SurfaceSupport.DetectSupport(worldCenter, footprintWorldSize, supportLayer, raycastMargin);

        if (ruleType == RuleType.RequiresFloorSupport)
            return LastDetectedSide == SurfaceSupport.Side.Floor;

        return LastDetectedSide != SurfaceSupport.Side.None; // RequiresAnySupport
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