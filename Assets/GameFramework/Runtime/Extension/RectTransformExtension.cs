using UnityEngine;
using System.Collections;

public static class RectTransformExtension {

	public static Vector2 GetWorldSize(this RectTransform self) {
        Vector3[] corners = new Vector3[4];
        self.GetWorldCorners(corners);
        Vector2 size = corners[2] - corners[0];
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        return size;
	}

    public static Rect GetWorldRect(this RectTransform self) {
        Vector3[] corners = new Vector3[4];
        self.GetWorldCorners(corners);
        Vector2 min = new Vector2(Mathf.Min(corners[1].x, corners[3].x), Mathf.Min(corners[1].y, corners[3].y));
        Vector2 max = new Vector2(Mathf.Max(corners[1].x, corners[3].x), Mathf.Max(corners[1].y, corners[3].y));
        return new Rect(min, max - min);
    }

	public static Rect GetScreenRect(this RectTransform self, Canvas canvas = null) {
        Camera camera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
            camera = canvas.worldCamera;
        }
		Vector3[] corners = new Vector3[4];
		self.GetWorldCorners(corners);
        corners[1] = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
        corners[3] = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
        corners[1].x = Mathf.Round(corners[1].x);
		corners[1].y = Mathf.Round(corners[1].y);
		corners[3].x = Mathf.Round(corners[3].x);
        corners[3].y = Mathf.Round(corners[3].y);
        Vector2 min = new Vector2(Mathf.Min(corners[1].x, corners[3].x), Mathf.Min(corners[1].y, corners[3].y));
        Vector2 max = new Vector2(Mathf.Max(corners[1].x, corners[3].x), Mathf.Max(corners[1].y, corners[3].y));
        return new Rect(min, max - min);
	}

    public static Vector2 ConvertCenterTo(this RectTransform from, RectTransform to, Canvas fromCanvas = null, Canvas toCanvas = null) {
        Camera fromCamera = (fromCanvas != null && fromCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? fromCanvas.worldCamera: null;
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(Camera.main, from.position);
        screenP += from.rect.center.Multiple(from.lossyScale);
        Vector2 localPoint;
        Camera toCamera = (toCanvas != null && toCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? toCanvas.worldCamera : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, Camera.main, out localPoint);
        return localPoint - to.rect.center;
    }

    public static void MakeSameCenterAs(this RectTransform self, RectTransform source, Canvas fromCanvas = null, Canvas toCanvas = null) {
        Vector2 position = source.ConvertCenterTo(self.parent as RectTransform);
        self.anchoredPosition = position - self.rect.center.Multiple(self.localScale);
    }

    public static Vector2 ConvertAnchorTo(this RectTransform from, RectTransform to, Canvas fromCanvas = null, Canvas toCanvas = null) {
        Camera fromCamera = (fromCanvas != null && fromCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? fromCanvas.worldCamera: null;
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(Camera.main, from.position);
        Vector2 localPoint;
        Camera toCamera = (toCanvas != null && toCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? toCanvas.worldCamera : null;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, Camera.main, out localPoint);
        return localPoint - to.rect.center;
    }

    public static void MakeSameAnchorAs(this RectTransform self, RectTransform to, Canvas fromCanvas = null, Canvas toCanvas = null) {
        self.anchoredPosition = to.ConvertAnchorTo(self.parent as RectTransform, fromCanvas, toCanvas);
    }

    public static Vector2 ConvertSizeTo(this RectTransform from, RectTransform to) {
        return from.rect.size.Multiple(from.lossyScale).Divide(to.lossyScale);
    }

    public static void MakeSameSizeAs(this RectTransform self, RectTransform source) {
        self.sizeDelta = source.ConvertSizeTo(self);
    }

    public static Rect ScreenRectToLocalRectInRectangle(this RectTransform self, Rect rect, Canvas toCanvas = null) {
        float xMin = float.PositiveInfinity;
        float yMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        float yMax = float.NegativeInfinity;
        Camera toCamera = (toCanvas != null && toCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? toCanvas.worldCamera : null;
        foreach (Vector2 vertex in rect.GetVertices()) {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(self, vertex, toCamera, out localPoint)) {
                xMin = Mathf.Min(xMin, localPoint.x);
                yMin = Mathf.Min(yMin, localPoint.y);
                xMax = Mathf.Max(xMax, localPoint.x);
                yMax = Mathf.Max(yMax, localPoint.y);
            }
        }
        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public static Rect WorldBoundsToLocalRectInRectangle(this RectTransform self, Bounds bounds, Camera fromCamera = null, Canvas toCanvas = null) {
        if (fromCamera == null) fromCamera = Camera.main;
        Rect rect = fromCamera.WorldToScreenRect(bounds);
        return ScreenRectToLocalRectInRectangle(self, rect, toCanvas);
    }

    public static void SetSize(this RectTransform self, Vector2 newSize) {
		Vector2 oldSize = self.rect.size;
		Vector2 deltaSize = newSize - oldSize;
		self.offsetMin = self.offsetMin - new Vector2(deltaSize.x * self.pivot.x, deltaSize.y * self.pivot.y);
		self.offsetMax = self.offsetMax + new Vector2(deltaSize.x * (1f - self.pivot.x), deltaSize.y * (1f - self.pivot.y));
	}

    public static bool HitTest(this RectTransform self, Vector2 screenPosition) {
        Rect screenRect = self.GetScreenRect();
        return screenRect.Contains(screenPosition);
    }

}