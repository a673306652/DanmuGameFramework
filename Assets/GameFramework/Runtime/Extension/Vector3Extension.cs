using UnityEngine;
using System.Collections;

public static class Vector3Extension {

    public static Vector2 ToV2(this Vector3 v) {
        return v;
    }

	public static Vector2 XZV2(this Vector3 v) {
		return new Vector2(v.x, v.z);
	}

    public static Vector3 NewX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 NewY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 NewZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 NewXY(this Vector3 v, float x, float y) {
        return new Vector3(x, y, v.z);
    }

    public static Vector3 NewXZ(this Vector3 v, float x, float z) {
        return new Vector3(x, v.y, z);
    }

    public static Vector3 NewYZ(this Vector3 v, float y, float z) {
        return new Vector3(v.x, y, z);
    }

    public static Vector3 OffsetX(this Vector3 v, float x) {
        return NewX(v, v.x + x);
    }

    public static Vector3 OffsetY(this Vector3 v, float y) {
        return NewY(v, v.y + y);
    }

    public static Vector3 OffsetZ(this Vector3 v, float z) {
        return NewZ(v, v.z + z);
    }

    public static Vector3 OffsetXY(this Vector3 v, float x, float y) {
        return NewXY(v, v.x + x, v.y + y);
    }

    public static Vector3 OffsetXZ(this Vector3 v, float x, float z) {
        return NewXZ(v, v.x + x, v.z + z);
    }

    public static Vector3 OffsetYZ(this Vector3 v, float y, float z) {
        return NewYZ(v, v.y + y, v.z + z);
    }

    public static Vector3 ScaleX(this Vector3 v, float x) {
        return NewX(v, v.x * x);
    }

    public static Vector3 ScaleY(this Vector3 v, float y) {
        return NewY(v, v.y * y);
    }

    public static Vector3 ScaleZ(this Vector3 v, float z) {
        return NewZ(v, v.z * z);
    }

    public static Vector3 ScaleXY(this Vector3 v, float x, float y) {
        return NewXY(v, v.x * x, v.y * y);
    }

    public static Vector3 ScaleXZ(this Vector3 v, float x, float z) {
        return NewXZ(v, v.x * x, v.z * z);
    }

    public static Vector3 ScaleYZ(this Vector3 v, float y, float z) {
        return NewYZ(v, v.y * y, v.z * z);
    }

    public static Vector3 GetScaled(this Vector3 v, Vector3 u) {
        return new Vector3(v.x * u.x, v.y * u.y, v.z * u.z);
    }

    public static Vector3 Multiple(this Vector3 v, Vector3 u) {
        return new Vector3(v.x * u.x, v.y * u.y, v.z * u.z);
    }

    public static Vector3 Divide(this Vector3 v, Vector3 u) {
        return new Vector3(v.x / u.x, v.y / u.y, v.z / u.z);
    }

    public static bool IsUniformScale(this Vector3 v) {
        return Mathf.Approximately(v.x, v.y) && Mathf.Approximately(v.x, v.z);
    }

    public static Vector3 Lerp(this Vector3 source, Vector3 target, float time) {
        return Vector3.Lerp(source, target, time);
    }

    public static Vector3 SnapBy(this Vector3 v, Vector3 u) {
        return new Vector3(
            Mathf.Approximately(u.x, 0) ? v.x : Mathf.Round(v.x / u.x) * u.x,
            Mathf.Approximately(u.y, 0) ? v.y : Mathf.Round(v.y / u.y) * u.y,
            Mathf.Approximately(u.z, 0) ? v.z : Mathf.Round(v.z / u.z) * u.z);
    }

    public static float AngleOnXZ(this Vector3 v) {
        return Vector3.right.AngleTo(v.NewY(0), Vector3.down);
    }

    public static float AngleTo(this Vector3 from, Vector3 to, Vector3 axis) {
        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
        return angle * sign;
    }

    public static Vector3 RotateBy(this Vector3 v, float angle, Vector3 axis) {
        return Quaternion.AngleAxis(angle, axis) * v;
    }

    public static Vector3 Clamp(this Vector3 v, Bounds bounds) {
        return v.Clamp(bounds.min, bounds.max);
    }

    public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) {
        return new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
    }

    public static Vector3 RandomPositionOnSphere(this Vector3 center, float radius) {
        return center + Random.onUnitSphere * radius;
    }

    public static Vector3 RandomPositionInSphere(this Vector3 center, float maxRadius, float minRadius = 0) {
        return center + Random.onUnitSphere * Random.Range(minRadius, maxRadius);
    }

    public static Vector3 PositionOnArcXZ(this Vector3 center, float radius, float angle) {
        return center.XZV2().PositionOnArc(radius, angle).XZV3().NewY(center.y);
    }

    public static Vector3 RandomPositionOnArcXZ(this Vector3 center, float radius, float startAngle = 0, float endAngle = 360) {
        return center.XZV2().RandomPositionOnArc(radius, startAngle, endAngle).XZV3().NewY(center.y);
    }

    public static Vector3 RandomPositionInSectorXZ(this Vector3 center, float maxRadius, float minRadius = 0, float startAngle = 0, float endAngle = 360) {
        return center.XZV2().RandomPositionInSector(maxRadius, minRadius, startAngle, endAngle);
    }

    public static Transform GetNearest(this Vector3 center, float radius, int layerMask) {
        Transform nearest = null;
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders) {
            float distance = Vector3.Distance(center, collider.transform.position);
            if (distance < minDistance) {
                nearest = collider.transform;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform GetNearestNonAlloc(this Vector3 center, float radius, Collider[] colliders, int layerMask) {
        Transform nearest = null;
        int count = Physics.OverlapSphereNonAlloc(center, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i) {
            float distance = Vector3.Distance(center, colliders[i].transform.position);
            if (distance < minDistance) {
                nearest = colliders[i].transform;
                minDistance = distance;
            }
        }
        return nearest;
    }
}
