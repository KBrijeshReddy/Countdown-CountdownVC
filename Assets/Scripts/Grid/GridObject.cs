using UnityEngine;

public class GridObject : MonoBehaviour
{
    [Header("Size In Grid Cells")]
    [Min(1)]
    public int width = 1;

    [Min(1)]
    public int height = 1;

    public Vector2Int GetSize()
    {
        return new Vector2Int(width, height);
    }
}