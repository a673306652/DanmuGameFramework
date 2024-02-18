using UnityEngine;
using System.Collections;

public static class CameraExtension {

    public static Rect GetWorldRect(this Camera camera) {
        float maxX = camera.orthographicSize * camera.aspect;
        float maxY = camera.orthographicSize;
        return new Rect(-maxX, -maxY, maxX * 2, maxY * 2);
    }

    public static Rect GetScreenWorldRect(this Camera camera) {
        Vector2 lb = camera.ViewportToWorldPoint(Vector2.zero);
        Vector2 rt = camera.ViewportToWorldPoint(Vector2.one);
        return new Rect(lb, rt - lb);
    }

    public static Rect WorldToScreenRect(this Camera camera, Bounds bounds) {
        float xMin = float.PositiveInfinity;
        float yMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        float yMax = float.NegativeInfinity;
        foreach (Vector3 vertex in bounds.GetVertices()) {
            Vector2 screenPos = camera.WorldToScreenPoint(vertex);
            xMin = Mathf.Min(xMin, screenPos.x);
            yMin = Mathf.Min(yMin, screenPos.y);
            xMax = Mathf.Max(xMax, screenPos.x);
            yMax = Mathf.Max(yMax, screenPos.y);
        }
        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static Vector3 WorldToCanvasPoint(this Camera camera, RectTransform canvas, Vector3 position) {
        position = camera.WorldToViewportPoint(position);
        Vector3 size = canvas.sizeDelta;
        position.x *= size.x;
        position.y *= size.y;
        position -= size / 2;
        return position;
    }

    public static Vector3 CanvasToWorldPoint(this Camera camera, RectTransform canvas, Vector3 position) {
        Vector3 size = canvas.sizeDelta;
        position.x /= size.x;
        position.y /= size.y;
        position += size / 2;
        return camera.ViewportToWorldPoint(position);
    }
	
}
