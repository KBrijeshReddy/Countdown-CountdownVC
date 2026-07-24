using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Color closedColor = Color.white;
    [SerializeField] private Color openColor = Color.green;

    [Header("Collider")]
    [SerializeField] private Collider2D doorCollider;

    private void Awake() => SetOpen(false);

    public void SetOpen(bool open)
    {
        if (visual != null)
            visual.color = open ? openColor : closedColor;

        if (doorCollider != null)
            doorCollider.enabled = !open;
    }
}