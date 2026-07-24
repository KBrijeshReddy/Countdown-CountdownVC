using UnityEngine;

public class BuyArea : MonoBehaviour
{
    private Collider2D areaCollider;

    private void Awake()
    {
        areaCollider = GetComponent<Collider2D>();

        if (areaCollider == null)
            Debug.LogError($"{name}: BuyArea requires a Collider2D.");
    }

    public bool IsInsideBuyArea(Vector2 worldPosition)
    {
        return areaCollider != null && areaCollider.OverlapPoint(worldPosition);
    }
}