using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtension {

    public static Vector3[] GetVertices(this Bounds self) {
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(self.min.x, self.min.y, self.min.z);
        vertices[1] = new Vector3(self.max.x, self.min.y, self.min.z);
        vertices[2] = new Vector3(self.max.x, self.max.y, self.min.z);
        vertices[3] = new Vector3(self.min.x, self.max.y, self.min.z);
        vertices[4] = new Vector3(self.min.x, self.min.y, self.max.z);
        vertices[5] = new Vector3(self.max.x, self.min.y, self.max.z);
        vertices[6] = new Vector3(self.max.x, self.max.y, self.max.z);
        vertices[7] = new Vector3(self.min.x, self.max.y, self.max.z);
        return vertices;
    }

    public static Rect ToRect(this Bounds self) {
        return new Rect(self.center, self.size);
    }

    public static Bounds NewX(this Bounds self, float x) {
        return new Bounds(self.center.NewX(x), self.size);
    }

    public static Bounds NewY(this Bounds self, float y) {
        return new Bounds(self.center.NewY(y), self.size);
    }

    public static Bounds NewZ(this Bounds self, float z) {
        return new Bounds(self.center.NewZ(z), self.size);
    }

    public static Bounds NewXY(this Bounds self, float x, float y) {
        return new Bounds(self.center.NewXY(x, y), self.size);
    }

    public static Bounds NewXZ(this Bounds self, float x, float z) {
        return new Bounds(self.center.NewXZ(x, z), self.size);
    }

    public static Bounds NewYZ(this Bounds self, float y, float z) {
        return new Bounds(self.center.NewYZ(y, z), self.size);
    }

    public static Bounds NewW(this Bounds self, float width) {
        return new Bounds(self.center, self.size.NewX(width));
    }

    public static Bounds NewH(this Bounds self, float height) {
        return new Bounds(self.center, self.size.NewY(height));
    }

    public static Bounds NewD(this Bounds self, float depth) {
        return new Bounds(self.center, self.size.NewZ(depth));
    }

    public static Bounds NewWH(this Bounds self, float width, float height) {
        return new Bounds(self.center, self.size.NewXY(width, height));
    }

    public static Bounds NewWD(this Bounds self, float width, float depth) {
        return new Bounds(self.center, self.size.NewXZ(width, depth));
    }

    public static Bounds NewHD(this Bounds self, float height, float depth) {
        return new Bounds(self.center, self.size.NewYZ(height, depth));
    }

    public static Bounds NewCenter(this Bounds self, Vector3 center) {
        return new Bounds(center, self.size);
    }

    public static Bounds NewSize(this Bounds self, Vector3 size) {
        return new Bounds(self.center, size);
    }

    public static Bounds GetZoomed(this Bounds self, float offset) {
        return self.GetZoomed(Vector3.one * offset);
    }

    public static Bounds GetZoomed(this Bounds self, Vector3 cornerOffset) {
        return self.GetZoomed(cornerOffset, cornerOffset);
    }

    public static Bounds GetZoomed(this Bounds self, Vector3 minCornerOffset, Vector3 maxCornerOffset) {
        Vector3 min = self.min - minCornerOffset;
        Vector3 max = self.max + maxCornerOffset;
        Vector3 size = max - min;
        Vector3 center = min + size * 0.5f;
        return new Bounds(center, size);
    }

    public static Bounds GetScaled(this Bounds self, float scale) {
        return self.GetScaled(scale, scale, scale);
    }

    public static Bounds GetScaled(this Bounds self, Vector3 scale) {
        return self.GetScaled(scale);
    }

    public static Bounds GetScaled(this Bounds self, float scaleX, float scaleY, float scaleZ) {
        float width = self.size.x * scaleX;
        float height = self.size.y * scaleY;
        float depth = self.size.z * scaleZ;
        float x = self.center.x - width * 0.5f;
        float y = self.center.y - height * 0.5f;
        float z = self.center.z - depth * 0.5f;
        return new Bounds(new Vector3(x, y, z), new Vector3(width, height, depth));
    }

    public static Bounds GetMerged(this Bounds self, Vector3 point) {
        Bounds merged = new Bounds(self.center, self.size);
        merged.Encapsulate(point);
        return merged;
    }

    public static Bounds GetMerged(this Bounds self, Bounds bounds) {
        Bounds merged = new Bounds(self.center, self.size);
        merged.Encapsulate(bounds);
        return merged;
    }

    public static bool Contains(this Bounds source, Bounds target) {
        return source.Contains(target.min) && source.Contains(target.max);
    }

    public static float GetPerimeter(this Bounds self) {
        return (self.size.x + self.size.y + self.size.z) * 4;
    }

    public static Vector3 RandomPosition(this Bounds self) {
        return new Vector3(
            Random.Range(self.min.x, self.max.x),
            Random.Range(self.min.y, self.max.y),
            Random.Range(self.min.z, self.max.z));
    }

    public static Vector3 RandomPositionOnEdge(this Bounds self) {
        Vector3 position = self.RandomPosition();
        int edge = Random.Range(0, 6);
        switch (edge) {
            case 0: position.x = self.max.x; break;
            case 1: position.y = self.max.y; break;
            case 2: position.z = self.max.z; break;
            case 3: position.x = self.min.x; break;
            case 4: position.y = self.min.y; break;
            case 5: position.y = self.min.z; break;
        }
        return position;
    }

    public static Vector3 RandomPositionOnEdgeWithoutY(this Bounds self)
    {
        Vector3 position = self.RandomPosition();
        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: position.x = self.max.x; break;
            case 1: position.z = self.max.z; break;
            case 2: position.x = self.min.x; break;
            case 3: position.z = self.min.z; break;
        }
        return position;
    }
}
