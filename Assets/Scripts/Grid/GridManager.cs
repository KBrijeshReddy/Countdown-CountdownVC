using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 8;
    public float cellSize = 1f;

    [Header("Grid Position")]
    public Vector2 gridOrigin;

    [Header("Blocked Tilemap")]
    public Tilemap blockedTilemap;

    [Header("Grid Visual")]
    public bool showGridVisual = true;

    public Color gridColor = new Color(1f, 1f, 1f, 0.15f);

    // Stores placeable objects.
    private GridObject[,] occupiedCells;

    private void Awake()
    {
        occupiedCells = new GridObject[
            gridWidth,
            gridHeight
        ];
    }

    // =========================================================
    // WORLD → GRID
    // =========================================================

    public Vector2Int WorldToGrid(
        Vector2 worldPosition
    )
    {
        int x = Mathf.FloorToInt(
            (worldPosition.x - gridOrigin.x)
            / cellSize
        );

        int y = Mathf.FloorToInt(
            (worldPosition.y - gridOrigin.y)
            / cellSize
        );

        return new Vector2Int(x, y);
    }

    // =========================================================
    // GRID → WORLD
    // =========================================================

    public Vector2 GridToWorld(
        Vector2Int gridPosition
    )
    {
        return new Vector2(
            gridOrigin.x +
            (gridPosition.x + 0.5f)
            * cellSize,

            gridOrigin.y +
            (gridPosition.y + 0.5f)
            * cellSize
        );
    }

    // =========================================================
    // OBJECT GRID → WORLD
    // =========================================================

    public Vector2 ObjectGridToWorld(
        Vector2Int bottomLeftCell,
        GridObject gridObject
    )
    {
        Vector2Int size =
            gridObject.GetSize();

        float objectWidth =
            size.x * cellSize;

        float objectHeight =
            size.y * cellSize;

        return new Vector2(
            gridOrigin.x +
            bottomLeftCell.x
            * cellSize
            + objectWidth / 2f,

            gridOrigin.y +
            bottomLeftCell.y
            * cellSize
            + objectHeight / 2f
        );
    }

    // =========================================================
    // WORLD → OBJECT GRID
    // =========================================================

    public Vector2Int WorldToObjectGrid(
        Vector2 worldPosition,
        GridObject gridObject
    )
    {
        Vector2Int size =
            gridObject.GetSize();

        float objectWidth =
            size.x * cellSize;

        float objectHeight =
            size.y * cellSize;

        Vector2 bottomLeftWorld =
            worldPosition -
            new Vector2(
                objectWidth / 2f,
                objectHeight / 2f
            );

        return WorldToGrid(
            bottomLeftWorld
        );
    }

    // =========================================================
    // CHECK TILEMAP CELL
    // =========================================================

    private bool IsTilemapCellBlocked(
        int gridX,
        int gridY
    )
    {
        if (blockedTilemap == null)
            return false;

        // Convert our grid cell into
        // a world position.
        Vector2 worldPosition =
            GridToWorld(
                new Vector2Int(
                    gridX,
                    gridY
                )
            );

        // Convert world position
        // into Tilemap cell coordinates.
        Vector3Int tilePosition =
            blockedTilemap.WorldToCell(
                worldPosition
            );

        // Check if a tile exists there.
        return blockedTilemap.HasTile(
            tilePosition
        );
    }

    // =========================================================
    // CHECK VALID PLACEMENT
    // =========================================================

    public bool CanPlaceObject(
        Vector2Int bottomLeftCell,
        GridObject gridObject
    )
    {
        Vector2Int size =
            gridObject.GetSize();

        // Check EVERY cell occupied
        // by this object.
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int checkX =
                    bottomLeftCell.x + x;

                int checkY =
                    bottomLeftCell.y + y;

                // =================================================
                // CHECK GRID BOUNDARY
                // =================================================

                if (
                    checkX < 0 ||
                    checkX >= gridWidth ||
                    checkY < 0 ||
                    checkY >= gridHeight
                )
                {
                    return false;
                }

                // =================================================
                // CHECK TILEMAP
                // =================================================

                if (
                    IsTilemapCellBlocked(
                        checkX,
                        checkY
                    )
                )
                {
                    return false;
                }

                // =================================================
                // CHECK OTHER PLACEABLE OBJECT
                // =================================================

                GridObject occupyingObject =
                    occupiedCells[
                        checkX,
                        checkY
                    ];

                if (
                    occupyingObject != null &&
                    occupyingObject != gridObject
                )
                {
                    return false;
                }
            }
        }

        return true;
    }

    // =========================================================
    // PLACE OBJECT
    // =========================================================

    public void PlaceObject(
        Vector2Int bottomLeftCell,
        GridObject gridObject
    )
    {
        Vector2Int size =
            gridObject.GetSize();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int cellX =
                    bottomLeftCell.x + x;

                int cellY =
                    bottomLeftCell.y + y;

                if (
                    cellX >= 0 &&
                    cellX < gridWidth &&
                    cellY >= 0 &&
                    cellY < gridHeight
                )
                {
                    occupiedCells[
                        cellX,
                        cellY
                    ] = gridObject;
                }
            }
        }
    }

    // =========================================================
    // REMOVE OBJECT
    // =========================================================

    public void RemoveObject(
        Vector2Int bottomLeftCell,
        GridObject gridObject
    )
    {
        Vector2Int size =
            gridObject.GetSize();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int cellX =
                    bottomLeftCell.x + x;

                int cellY =
                    bottomLeftCell.y + y;

                if (
                    cellX >= 0 &&
                    cellX < gridWidth &&
                    cellY >= 0 &&
                    cellY < gridHeight
                )
                {
                    if (
                        occupiedCells[
                            cellX,
                            cellY
                        ] == gridObject
                    )
                    {
                        occupiedCells[
                            cellX,
                            cellY
                        ] = null;
                    }
                }
            }
        }
    }

    // =========================================================
    // DRAW GRID
    // =========================================================

    private void OnDrawGizmos()
{
    // Don't draw grid if disabled.
    if (!showGridVisual)
        return;

    Gizmos.color =
        gridColor;

    // Vertical lines
    for (int x = 0; x <= gridWidth; x++)
    {
        Vector3 start =
            new Vector3(
                gridOrigin.x +
                x * cellSize,

                gridOrigin.y,

                0f
            );

        Vector3 end =
            new Vector3(
                gridOrigin.x +
                x * cellSize,

                gridOrigin.y +
                gridHeight * cellSize,

                0f
            );

        Gizmos.DrawLine(
            start,
            end
        );
    }

    // Horizontal lines
    for (int y = 0; y <= gridHeight; y++)
    {
        Vector3 start =
            new Vector3(
                gridOrigin.x,

                gridOrigin.y +
                y * cellSize,

                0f
            );

        Vector3 end =
            new Vector3(
                gridOrigin.x +
                gridWidth * cellSize,

                gridOrigin.y +
                y * cellSize,

                0f
            );

        Gizmos.DrawLine(
            start,
            end
        );
    }
}
}