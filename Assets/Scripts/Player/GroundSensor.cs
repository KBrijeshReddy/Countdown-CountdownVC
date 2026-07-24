using UnityEngine;
using System;

/// Reusable ground-detection component. Anything that needs to know
/// whether it's standing on the ground layer can use this instead of
/// rolling its own OverlapCircle check.
public class GroundSensor : MonoBehaviour
{
    [SerializeField] private Transform checkPoint;
    [SerializeField] private float checkRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    /// Raised the frame this transitions from airborne to grounded.
    public event Action Landed;

    /// Raised the frame this transitions from grounded to airborne.
    public event Action LeftGround;

    private void Update()
    {
        bool wasGrounded = IsGrounded;

        IsGrounded = checkPoint != null &&
            Physics2D.OverlapCircle(checkPoint.position, checkRadius, groundLayer);

        if (IsGrounded && !wasGrounded) Landed?.Invoke();
        if (!IsGrounded && wasGrounded) LeftGround?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkPoint.position, checkRadius);
    }
}