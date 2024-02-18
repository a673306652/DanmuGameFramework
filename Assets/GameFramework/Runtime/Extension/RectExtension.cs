using UnityEngine;
using System.Collections;

public static class RectExtension {

    public static Vector2[] GetVertices(this Rect self) {
        Vector2[] vertices = new Vector2[4];
        vertices[0] = new Vector3(self.min.x, self.min.y);
        vertices[1] = new Vector3(self.max.x, self.min.y);
        vertices[2] = new Vector3(self.max.x, self.max.y);
        vertices[3] = new Vector3(self.min.x, self.max.y);
        return vertices;
    }

    public static Bounds ToBounds(this Rect self) {
        return new Bounds(self.center, self.size);
    }

    public static Rect NewX(this Rect self, float x) {
        return new Rect(x, self.y, self.width, self.height);
    }

    public static Rect NewY(this Rect self, float y) {
        return new Rect(self.x, y, self.width, self.height);
    }

    public static Rect NewXY(this Rect self, float x, float y) {
        return new Rect(x, y, self.width, self.height);
    }

    public static Rect NewW(this Rect self, float width) {
        return new Rect(self.x, self.y, width, self.height);
    }

    public static Rect NewH(this Rect self, float height) {
        return new Rect(self.x, self.y, self.width, height);
    }

    public static Rect NewWH(this Rect self, float width, float height) {
        return new Rect(self.x, self.y, width, height);
    }

    public static Rect NewCenter(this Rect self, Vector2 center) {
        return new Rect(center, self.size);
    }

    public static Rect NewSize(this Rect self, Vector2 size) {
        return new Rect(self.position, size);
    }

    public static Rect GetZoomed(this Rect self, float size) {
        return self.GetZoomed(new Vector2(size, size));
    }

    public static Rect GetZoomed(this Rect self, Vector2 cornerOffset) {
        return self.GetZoomed(cornerOffset, cornerOffset);
    }

    public static Rect GetZoomed(this Rect self, Vector2 minCornerOffset, Vector2 maxCornerOffset) {
        Vector2 min = self.min - minCornerOffset;
        Vector2 max = self.max + maxCornerOffset;
        Vector2 size = max - min;
        return new Rect(min, size);
    }

    public static Rect GetScaled(this Rect self, float scale) {
        return self.GetScaled(scale, scale);
    }

    public static Rect GetScaled(this Rect self, Vector2 scale) {
        return self.GetScaled(scale.x, scale.y);
    }

    public static Rect GetScaled(this Rect self, float scaleX, float scaleY) {
        float width = self.width * scaleX;
        float height = self.height * scaleY;
        float x = self.center.x - width * 0.5f;
        float y = self.center.y - height * 0.5f;
        return new Rect(x, y, width, height);
    }

    public static Rect GetMerged(this Rect self, Vector2 point) {
        float xMin = Mathf.Min(self.min.x, point.x);
        float yMin = Mathf.Min(self.min.y, point.y);
        float xMax = Mathf.Max(self.max.x, point.x);
        float yMax = Mathf.Max(self.max.y, point.y);
        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static Rect GetMerged(this Rect self, Rect rect) {
        float xMin = Mathf.Min(self.min.x, rect.min.x);
        float yMin = Mathf.Min(self.min.y, rect.min.y);
        float xMax = Mathf.Max(self.max.x, rect.max.x);
        float yMax = Mathf.Max(self.max.y, rect.max.y);
        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static bool Contains(this Rect source, Rect target) {
        return source.Contains(target.min) && source.Contains(target.max);
    }

    public static bool Intersects(this Rect source, Rect target) {
        return (source.Contains(target.min) && !source.Contains(target.max))
            || (source.Contains(target.max) && !source.Contains(target.min));
    }

    public static float GetPerimeter(this Rect self) {
        return self.width * 2 + self.height * 2;
    }

    public static Vector2 RandomPosition(this Rect self) {
        return new Vector2(Random.Range(self.xMin, self.xMax), Random.Range(self.yMin, self.yMax));
    }

}
