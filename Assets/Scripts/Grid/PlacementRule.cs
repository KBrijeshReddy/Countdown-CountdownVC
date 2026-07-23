using UnityEngine;

public class PlacementRule : MonoBehaviour
{
    public enum RuleType
    {
        None,
        AdjacentObject,
        ObjectBelow
    }

    [Header("Placement Rule")]
    public RuleType ruleType =
        RuleType.None;

    private GridObject gridObject;

    private void Awake()
    {
        gridObject =
            GetComponent<GridObject>();
    }

    // =========================================================
    // CHECK RULE
    // =========================================================

    public bool IsPlacementValid(
        Vector2Int bottomLeftCell,
        GridManager gridManager
    )
    {
        switch (ruleType)
        {
            case RuleType.None:

                return true;

            case RuleType.AdjacentObject:

                return HasAdjacentObject(
                    bottomLeftCell,
                    gridManager
                );

            case RuleType.ObjectBelow:

                return HasObjectBelow(
                    bottomLeftCell,
                    gridManager
                );
        }

        return true;
    }

    // =========================================================
    // BUTTON
    // =========================================================

    private bool HasAdjacentObject(
        Vector2Int bottomLeftCell,
        GridManager gridManager
    )
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (
            Vector2Int direction
            in directions
        )
        {
            Vector2Int checkCell =
                bottomLeftCell +
                direction;

            // IMPORTANT:
            // Don't require the adjacent cell
            // to be inside the playable grid.
            //
            // We only care if there is something
            // actually occupying that cell.

            if (
                gridManager.HasAnythingAt(
                    checkCell
                )
            )
            {
                return true;
            }
        }

        return false;
    }

    // =========================================================
    // DOOR
    // =========================================================

    private bool HasObjectBelow(
    Vector2Int bottomLeftCell,
    GridManager gridManager
)
{
    Vector2Int cellBelow =
        new Vector2Int(
            bottomLeftCell.x,
            bottomLeftCell.y - 1
        );

    return gridManager.HasAnythingAt(
        cellBelow
    );
}
}