using UnityEngine;
using System.Collections;

public static class Vector2Extension
{

    public static Vector3 ToV3(this Vector2 v)
    {
        return v;
    }

    public static Vector3 XZV3(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static Vector2 NewX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector2 NewY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector2 OffsetX(this Vector2 v, float x)
    {
        return NewX(v, v.x + x);
    }

    public static Vector2 OffsetY(this Vector2 v, float y)
    {
        return NewY(v, v.y + y);
    }

    public static Vector2 ScaleX(this Vector2 v, float x)
    {
        return NewX(v, v.x * x);
    }

    public static Vector2 ScaleY(this Vector2 v, float y)
    {
        return NewY(v, v.y * y);
    }

    public static Vector2 GetScaled(this Vector2 v, Vector2 u)
    {
        return new Vector2(v.x * u.x, v.y * u.y);
    }

    public static Vector2 Multiple(this Vector2 v, Vector2 u)
    {
        return new Vector2(v.x * u.x, v.y * u.y);
    }

    public static Vector2 Divide(this Vector2 v, Vector2 u)
    {
        return new Vector2(v.x / u.x, v.y / u.y);
    }

    public static bool IsUniformScale(this Vector2 v)
    {
        return Mathf.Approximately(v.x, v.y);
    }

    public static Vector2 Lerp(this Vector2 source, Vector2 target, float time)
    {
        return Vector2.Lerp(source, target, time);
    }

    public static Vector2 SnapBy(this Vector2 v, Vector2 u)
    {
        return new Vector2(
            Mathf.Approximately(u.x, 0) ? v.x : Mathf.Round(v.x / u.x) * u.x,
            Mathf.Approximately(u.y, 0) ? v.y : Mathf.Round(v.y / u.y) * u.y);
    }

    public static float Angle(this Vector2 v)
    {
        return Vector2.right.AngleTo(v);
    }

    public static float AngleTo(this Vector2 from, Vector2 to)
    {
        Vector3 axis = Vector3.Cross(from, to);
        float angle = Vector2.Angle(from, to);
        return axis.z > 0 ? angle : -angle;
    }

    public static Vector2 RotateBy(this Vector2 v, float angle)
    {
        float rad = (v.Angle() + angle) * Mathf.Deg2Rad;
        Vector2 newV = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        return newV * v.magnitude;
    }

    public static Vector2 Clamp(this Vector2 v, Rect rect)
    {
        return v.Clamp(rect.min, rect.max);
    }

    public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
    {
        return new Vector2(
            Mathf.Clamp(v.x, min.x, max.x),
            Mathf.Clamp(v.y, min.y, max.y));
    }

    public static Vector2 PositionOnArc(this Vector2 center, float radius, float angle)
    {
        return center + (Vector2.right * radius).RotateBy(angle);
    }

    public static Vector2 RandomPositionOnArc(this Vector2 center, float radius, float startAngle = 0, float endAngle = 360)
    {
        return PositionOnArc(center, radius, Random.Range(startAngle, endAngle));
    }

    public static Vector2 RandomPositionInSector(this Vector2 center, float maxRadius, float minRadius = 0, float startAngle = 0, float endAngle = 360)
    {
        return RandomPositionOnArc(center, Random.Range(minRadius, maxRadius), startAngle, endAngle);
    }

    public static float RandomBetweenXY(this Vector2 vect)
    {
        return Random.Range(vect.x, vect.y);
    }

    public static Transform GetNearest(this Vector2 center, float radius, int layerMask)
    {
        Transform nearest = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(center, collider.transform.position);
            if (distance < minDistance)
            {
                nearest = collider.transform;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform GetNearestNonAlloc(this Vector2 center, float radius, Collider2D[] colliders, int layerMask)
    {
        Transform nearest = null;
        int count = Physics2D.OverlapCircleNonAlloc(center, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i)
        {
            float distance = Vector2.Distance(center, colliders[i].transform.position);
            if (distance < minDistance)
            {
                nearest = colliders[i].transform;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform GetTopMostHitSprite(this Vector2 position, int layerMask)
    {
        Transform topMost = null;
        Collider2D[] colliders = Physics2D.OverlapPointAll(position, layerMask);
        int maxSortingOrder = int.MinValue;
        foreach (Collider2D collider in colliders)
        {
            SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (spriteRenderer.sortingOrder > maxSortingOrder)
                {
                    topMost = collider.transform;
                    maxSortingOrder = spriteRenderer.sortingOrder;
                }
            }
        }
        return topMost;
    }
}
