using UnityEngine;

/// Rotates this GameObject to match whichever surface a placeable is
/// resting against, so wall/ceiling-mounted objects don't look like
/// they're floating in the default floor orientation.
public class SurfaceMountRotator : MonoBehaviour
{
    public void ApplyRotation(SurfaceSupport.Side side)
    {
        transform.localRotation = SurfaceSupport.GetRotationForSide(side);
    }
}