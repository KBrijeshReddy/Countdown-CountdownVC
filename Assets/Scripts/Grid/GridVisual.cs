using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [Header("Grid Reference")]
    public GridManager gridManager;

    [Header("Visual Settings")]
    public Color gridColor = new Color(
        1f,
        1f,
        1f,
        0.1f
    );

    public float lineWidth = 0.02f;

    [Header("Line Material")]
    public Material lineMaterial;

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError(
                "GridVisual: GridManager is not assigned!"
            );

            return;
        }

        GenerateGrid();
    }

    // =========================================================
    // GENERATE GRID
    // =========================================================

    public void GenerateGrid()
    {
        // Make sure we don't accidentally
        // generate the grid multiple times.
        ClearGrid();

        // Get all grid information
        // directly from GridManager.
        int gridWidth =
            gridManager.gridWidth;

        int gridHeight =
            gridManager.gridHeight;

        float cellSize =
            gridManager.cellSize;

        Vector2 gridOrigin =
            gridManager.gridOrigin;

        // =====================================================
        // VERTICAL LINES
        // =====================================================

        for (int x = 0; x <= gridWidth; x++)
        {
            GameObject lineObject =
                new GameObject(
                    "VerticalLine_" + x
                );

            lineObject.transform.SetParent(
                transform
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            SetupLineRenderer(line);

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

            line.positionCount = 2;

            line.SetPosition(
                0,
                start
            );

            line.SetPosition(
                1,
                end
            );
        }

        // =====================================================
        // HORIZONTAL LINES
        // =====================================================

        for (int y = 0; y <= gridHeight; y++)
        {
            GameObject lineObject =
                new GameObject(
                    "HorizontalLine_" + y
                );

            lineObject.transform.SetParent(
                transform
            );

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            SetupLineRenderer(line);

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

            line.positionCount = 2;

            line.SetPosition(
                0,
                start
            );

            line.SetPosition(
                1,
                end
            );
        }
    }

    // =========================================================
    // SETUP LINE
    // =========================================================

    private void SetupLineRenderer(
        LineRenderer line
    )
    {
        line.startWidth =
            lineWidth;

        line.endWidth =
            lineWidth;

        line.startColor =
            gridColor;

        line.endColor =
            gridColor;

        line.useWorldSpace =
            true;

        line.numCapVertices =
            1;

        line.sortingOrder =
            10;

        if (lineMaterial != null)
        {
            line.material =
                lineMaterial;
        }
    }

    // =========================================================
    // CLEAR GRID
    // =========================================================

    public void ClearGrid()
    {
        // Delete all previously generated lines.
        for (
            int i = transform.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                transform.GetChild(i).gameObject
            );
        }
    }

    // =========================================================
    // SHOW GRID
    // =========================================================

    public void ShowGrid()
    {
        gameObject.SetActive(
            true
        );
    }

    // =========================================================
    // HIDE GRID
    // =========================================================

    public void HideGrid()
    {
        gameObject.SetActive(
            false
        );
    }
}