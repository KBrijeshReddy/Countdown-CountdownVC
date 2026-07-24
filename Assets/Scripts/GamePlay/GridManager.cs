using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOrigin;

    [Header("Blocked Tilemap")]
    [SerializeField] private Tilemap blockedTilemap;

    [Header("Grid Visual (Gizmo)")]
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.15f);

    public bool ShowGridVisual { get; set; } = true;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public float CellSize => cellSize;
    public Vector2 GridOrigin => gridOrigin;

    private GridFootprint[,] occupiedCells;

    private void Awake()
    {
        occupiedCells = new GridFootprint[gridWidth, gridHeight];
    }

    public Vector2Int WorldToGrid(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector2(
            gridOrigin.x + (gridPosition.x + 0.5f) * cellSize,
            gridOrigin.y + (gridPosition.y + 0.5f) * cellSize);
    }

    public Vector2 ObjectGridToWorld(Vector2Int bottomLeftCell, GridFootprint footprint)
    {
        Vector2Int size = footprint.Size;
        float width = size.x * cellSize;
        float height = size.y * cellSize;

        return new Vector2(
            gridOrigin.x + bottomLeftCell.x * cellSize + width / 2f,
            gridOrigin.y + bottomLeftCell.y * cellSize + height / 2f);
    }

    public Vector2Int WorldToObjectGrid(Vector2 worldPosition, GridFootprint footprint)
    {
        Vector2Int size = footprint.Size;
        float width = size.x * cellSize;
        float height = size.y * cellSize;

        Vector2 bottomLeftWorld = worldPosition - new Vector2(width / 2f, height / 2f);
        return WorldToGrid(bottomLeftWorld);
    }

    public bool CanPlaceObject(Vector2Int bottomLeftCell, GridFootprint footprint)
    {
        Vector2Int size = footprint.Size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int checkX = bottomLeftCell.x + x;
                int checkY = bottomLeftCell.y + y;

                if (checkX < 0 || checkX >= gridWidth || checkY < 0 || checkY >= gridHeight)
                    return false;

                if (IsTilemapCellBlocked(checkX, checkY))
                    return false;

                GridFootprint occupying = occupiedCells[checkX, checkY];
                if (occupying != null && occupying != footprint)
                    return false;
            }
        }

        PlacementRule rule = footprint.GetComponent<PlacementRule>();
        if (rule != null && !rule.IsPlacementValid(bottomLeftCell, this))
            return false;

        return true;
    }

    public void PlaceObject(Vector2Int bottomLeftCell, GridFootprint footprint)
    {
        Vector2Int size = footprint.Size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int cellX = bottomLeftCell.x + x;
                int cellY = bottomLeftCell.y + y;

                if (cellX >= 0 && cellX < gridWidth && cellY >= 0 && cellY < gridHeight)
                    occupiedCells[cellX, cellY] = footprint;
            }
        }
    }

    public void RemoveObject(Vector2Int bottomLeftCell, GridFootprint footprint)
    {
        Vector2Int size = footprint.Size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int cellX = bottomLeftCell.x + x;
                int cellY = bottomLeftCell.y + y;

                if (cellX >= 0 && cellX < gridWidth && cellY >= 0 && cellY < gridHeight &&
                    occupiedCells[cellX, cellY] == footprint)
                {
                    occupiedCells[cellX, cellY] = null;
                }
            }
        }
    }

    public bool HasAnythingAt(Vector2Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < gridWidth &&
            gridPosition.y >= 0 && gridPosition.y < gridHeight &&
            occupiedCells[gridPosition.x, gridPosition.y] != null)
        {
            return true;
        }

        return IsTilemapCellBlocked(gridPosition.x, gridPosition.y);
    }

    private bool IsTilemapCellBlocked(int gridX, int gridY)
    {
        if (blockedTilemap == null)
            return false;

        Vector2 worldPosition = GridToWorld(new Vector2Int(gridX, gridY));
        Vector3Int tilePosition = blockedTilemap.WorldToCell(worldPosition);
        return blockedTilemap.HasTile(tilePosition);
    }

    private void OnDrawGizmos()
    {
        if (!ShowGridVisual)
            return;

        Gizmos.color = gridColor;

        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y, 0f);
            Vector3 end = new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y + gridHeight * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(gridOrigin.x, gridOrigin.y + y * cellSize, 0f);
            Vector3 end = new Vector3(gridOrigin.x + gridWidth * cellSize, gridOrigin.y + y * cellSize, 0f);
            Gizmos.DrawLine(start, end);
        }
    }
}