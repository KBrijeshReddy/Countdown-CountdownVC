using UnityEngine;

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

    /// True only if there's support directly above AND below —
    /// used for objects (like doors) that must sit wedged in a gap.
    public static bool HasFloorAndCeilingSupport(
        Vector2 worldCenter,
        Vector2 footprintWorldSize,
        LayerMask supportLayer,
        float checkDistance)
    {
        float halfHeight = footprintWorldSize.y / 2f;

        bool hasFloor = Physics2D.Raycast(worldCenter, Vector2.down, halfHeight + checkDistance, supportLayer);
        bool hasCeiling = Physics2D.Raycast(worldCenter, Vector2.up, halfHeight + checkDistance, supportLayer);

        return hasFloor && hasCeiling;
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