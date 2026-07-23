using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 8;
    public float cellSize = 1f;

    [Header("Grid Position")]
    public Vector2 gridOrigin;

    [Header("Visual Settings")]
    public Color gridColor = new Color(
        1f,
        1f,
        1f,
        0.15f
    );

    public float lineWidth = 0.02f;

    [Header("Line Material")]
    public Material lineMaterial;

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // =============================================
        // VERTICAL LINES
        // =============================================

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

        // =============================================
        // HORIZONTAL LINES
        // =============================================

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

        // Use assigned material.
        if (lineMaterial != null)
        {
            line.material =
                lineMaterial;
        }

        // Make sure lines render above
        // the background.
        line.sortingOrder =
            10;
    }
}