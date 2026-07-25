using UnityEngine;

/// Shared physics-based surface detection. Used by PlacementRule to
/// validate an object has something to rest against, and by
/// SurfaceMountRotator to orient a visual to match that surface.
public static class SurfaceSupport
{
    public enum Side { None, Floor, Ceiling, LeftWall, RightWall }

    public static Side DetectSupport(
        Vector2 worldCenter,
        Vector2 footprintWorldSize,
        LayerMask supportLayer,
        float checkDistance)
    {
        float halfWidth = footprintWorldSize.x / 2f;
        float halfHeight = footprintWorldSize.y / 2f;

        // Priority order: floor first (most common case), then walls, then ceiling.
        if (Physics2D.Raycast(worldCenter, Vector2.down, halfHeight + checkDistance, supportLayer))
            return Side.Floor;

        if (Physics2D.Raycast(worldCenter, Vector2.left, halfWidth + checkDistance, supportLayer))
            return Side.LeftWall;

        if (Physics2D.Raycast(worldCenter, Vector2.right, halfWidth + checkDistance, supportLayer))
            return Side.RightWall;

        if (Physics2D.Raycast(worldCenter, Vector2.up, halfHeight + checkDistance, supportLayer))
            return Side.Ceiling;

        return Side.None;
    }

    public static Quaternion GetRotationForSide(Side side)
    {
        return side switch
        {
            Side.Floor => Quaternion.identity,
            Side.RightWall => Quaternion.Euler(0f, 0f, 90f),
            Side.Ceiling => Quaternion.Euler(0f, 0f, 180f),
            Side.LeftWall => Quaternion.Euler(0f, 0f, -90f),
            _ => Quaternion.identity
        };
    }
}