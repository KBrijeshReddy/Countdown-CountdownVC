using UnityEngine;

public class GridFootprint : MonoBehaviour
{
    [SerializeField, Min(1)] private int width = 1;
    [SerializeField, Min(1)] private int height = 1;

    public Vector2Int Size => new Vector2Int(width, height);
}