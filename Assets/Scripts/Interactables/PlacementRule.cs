using UnityEngine;

public class PlacementRule : MonoBehaviour
{
    public enum RuleType { None, AdjacentObject, ObjectBelow }

    [SerializeField] private RuleType ruleType = RuleType.None;

    public bool IsPlacementValid(Vector2Int bottomLeftCell, GridManager gridManager)
    {
        return ruleType switch
        {
            RuleType.None => true,
            RuleType.AdjacentObject => HasAdjacentObject(bottomLeftCell, gridManager),
            RuleType.ObjectBelow => gridManager.HasAnythingAt(bottomLeftCell + Vector2Int.down),
            _ => true
        };
    }

    private bool HasAdjacentObject(Vector2Int bottomLeftCell, GridManager gridManager)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            if (gridManager.HasAnythingAt(bottomLeftCell + dir))
                return true;
        }

        return false;
    }
}