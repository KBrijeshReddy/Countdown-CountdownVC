using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [Header("Grid Reference")]
    [SerializeField] private GridManager gridManager;

    [Header("Visual Settings")]
    [SerializeField] private Color gridColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField] private Material lineMaterial;

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError($"{name}: GridManager is not assigned.");
            return;
        }

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        int width = gridManager.GridWidth;
        int height = gridManager.GridHeight;
        float cellSize = gridManager.CellSize;
        Vector2 origin = gridManager.GridOrigin;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(origin.x + x * cellSize, origin.y, 0f);
            Vector3 end = new Vector3(origin.x + x * cellSize, origin.y + height * cellSize, 0f);
            CreateLine($"VerticalLine_{x}", start, end);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(origin.x, origin.y + y * cellSize, 0f);
            Vector3 end = new Vector3(origin.x + width * cellSize, origin.y + y * cellSize, 0f);
            CreateLine($"HorizontalLine_{y}", start, end);
        }
    }

    private void CreateLine(string lineName, Vector3 start, Vector3 end)
    {
        var lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(transform);

        var line = lineObject.AddComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = gridColor;
        line.endColor = gridColor;
        line.useWorldSpace = true;
        line.numCapVertices = 1;
        line.sortingOrder = 10;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        if (lineMaterial != null)
            line.material = lineMaterial;
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    public void ShowGrid() => gameObject.SetActive(true);
    public void HideGrid() => gameObject.SetActive(false);
}