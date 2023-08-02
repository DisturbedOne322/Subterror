using UnityEngine;

public static class LineIntersection
{
    public static Vector2 FindIntersectionPoint(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        float denominator = ((pointA.x - pointB.x) * (pointC.y - pointD.y) - (pointA.y - pointB.y) * (pointC.x - pointD.x));
        if (denominator == 0)
            return Vector2.zero;

        float pointX = ((pointA.x * pointB.y - pointA.y * pointB.x) * (pointC.x - pointD.x) - (pointA.x - pointB.x) * (pointC.x * pointD.y - pointC.y * pointD.x))
            / denominator;

        float pointY = ((pointA.x * pointB.y - pointA.y * pointB.x) * (pointC.y - pointD.y) - (pointA.y - pointB.y) * (pointC.x * pointD.y - pointC.y * pointD.x))
            / denominator;

        return new Vector2(pointX, pointY);
    }
}
