using UnityEngine;

public class BuyArea : MonoBehaviour
{
    private Collider2D areaCollider;

    private void Awake()
    {
        areaCollider =
            GetComponent<Collider2D>();

        if (areaCollider == null)
        {
            Debug.LogError(
                "BuyArea needs a Collider2D!"
            );
        }
    }

    public bool IsInsideBuyArea(
        Vector2 worldPosition
    )
    {
        if (areaCollider == null)
        {
            return false;
        }

        return areaCollider.OverlapPoint(
            worldPosition
        );
    }
}