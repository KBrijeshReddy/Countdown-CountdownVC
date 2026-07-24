using UnityEngine;

public class BuyArea : MonoBehaviour
{
    private Collider2D areaCollider;

    public static BuyArea Instance { get; private set; }

    private void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
        Instance = this;

        if (areaCollider == null)
            Debug.LogError($"{name}: BuyArea requires a Collider2D.");
    }

    public bool IsInsideBuyArea(Vector2 worldPosition)
    {
        return areaCollider != null && areaCollider.OverlapPoint(worldPosition);
    }
}
