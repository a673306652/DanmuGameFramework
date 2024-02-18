using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtension {

	public static void Save(this Texture2D tex, string path) {
		if (path.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
		    path.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase)) {
			tex.SaveToJPG(path);
		} else {
			tex.SaveToPNG(path);
		}
	}

	public static void SaveToJPG(this Texture2D tex, string path) {
		File.WriteAllBytes(path, tex.EncodeToJPG());
	}

	public static void SaveToPNG(this Texture2D tex, string path) {
		File.WriteAllBytes(path, tex.EncodeToPNG());
	}

	public static Texture2D NewCopy(this Texture2D tex) {
		Texture2D newTex = new Texture2D(tex.width, tex.height);
		newTex.SetPixels32(tex.GetPixels32());
		newTex.Apply();
		return newTex;
	}

	public static Texture2D GetResized(this Texture2D tex, int width, int height) {
		if (width == tex.width && height == tex.height) {
			return tex.NewCopy();
		}
		Texture2D newTex = new Texture2D(width, height, tex.format, false);
		for (int j = 0; j < height; ++j) {
			for (int i = 0; i < width; ++i) {
				newTex.SetPixel(i, j, tex.GetPixelBilinear((float)i / width, (float)j / height));
			}
		}
		newTex.Apply();
		return newTex;
	}

	public static Texture2D GetScaled(this Texture2D tex, float scaleX, float scaleY) {
		return tex.GetResized(Mathf.RoundToInt(tex.width * scaleX), Mathf.RoundToInt(tex.height * scaleY));
	}

	public static void CaptureScreen(this Texture2D source, Rect fromRect, int toX, int toY) {
		source.ReadPixels(fromRect, toX, toY);
		source.Apply();
	}

	public static void Overlay(this Texture2D source, Texture2D target, int x, int y) {
		source.Overlay(target, x, y, target.width, target.height);
	}

#if UNITY_2017
    public static void Overlay(this Texture2D source, Texture2D target, RectInt rect) {
        source.Overlay(target, rect.x, rect.y, rect.width, rect.height);
    }
#endif

    public static void Overlay(this Texture2D source, Texture2D target, int x, int y, int width, int height) {
		if (width != target.width || height != target.height) {
			target = target.GetResized(width, height);
		}
		for (int j = y; j < y + target.height; ++j) {
			for (int i = x; i < x + target.width; ++i) {
				Color sourceColor = source.GetPixel(i, j);
				Color targetColor = target.GetPixel(i - x, j - y);
				Color color = Color.Lerp(sourceColor, targetColor, targetColor.a);
				source.SetPixel(i, j, color);
			}
		}
		source.Apply();
	}
}
