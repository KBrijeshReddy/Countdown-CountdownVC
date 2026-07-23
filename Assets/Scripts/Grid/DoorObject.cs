using UnityEngine;

public class DoorObject : MonoBehaviour
{
    [Header("Door Visual")]
    public Color closedColor = Color.white;
    public Color openColor = Color.green;

    [Header("Door Collider")]
    public Collider2D doorCollider;

    private SpriteRenderer spriteRenderer;

    private bool isOpen;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        // Automatically find collider
        // if one wasn't assigned.
        if (doorCollider == null)
        {
            doorCollider =
                GetComponent<Collider2D>();
        }

        SetOpen(false);
    }

    // =========================================================
    // OPEN / CLOSE
    // =========================================================

    public void SetOpen(
        bool open
    )
    {
        isOpen =
            open;

        // Change door color.
        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                isOpen
                ? openColor
                : closedColor;
        }

        // Disable collider when open.
        if (doorCollider != null)
        {
            doorCollider.enabled =
                !isOpen;
        }
    }
}