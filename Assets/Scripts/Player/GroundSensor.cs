using UnityEngine;
using System;

/// Reusable ground-detection component. Checks a small box under the
/// player's feet rather than a single point, so standing near a
/// platform's edge or corner still registers as grounded reliably.
public class GroundSensor : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;
    [SerializeField] private Vector2 checkBoxSize = new Vector2(0.6f, 0.15f);
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    public event Action Landed;
    public event Action LeftGround;

    private void Update()
    {
        bool wasGrounded = IsGrounded;

        IsGrounded = checkPoint != null &&
            Physics2D.OverlapBox(checkPoint.position, checkBoxSize, 0f, groundLayer);

        if (IsGrounded && !wasGrounded) Landed?.Invoke();
        if (!IsGrounded && wasGrounded) LeftGround?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(checkPoint.position, checkBoxSize);
    }
}