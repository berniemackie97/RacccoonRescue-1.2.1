using UnityEngine;

public class SplineCurve
{
    private readonly Vector2[] _points;

    public SplineCurve()
    {
        _points = new Vector2[4];
    }

    public void ApplyPoints(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        _points[0] = p1;
        _points[1] = p2;
        _points[2] = p3;
        _points[3] = p4;
    }

    public Vector2 GetPoint(float t)
    {
        return GetPoint(_points[0], _points[1], _points[2], _points[3], t);
    }

    public static Vector2 GetPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        //t==0, ret b
        //t==1, ret c
        return .5f * (
            (-a + 3f * b - 3f * c + d) * (t * t * t)
            + (2f * a - 5f * b + 4f * c - d) * (t * t)
            + (-a + c) * t
            + 2f * b
            );
    }
}
