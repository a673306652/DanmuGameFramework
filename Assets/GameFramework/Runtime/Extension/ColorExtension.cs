using UnityEngine;
using System.Collections;

public class HSVColor
{

    public int h;
    public int s;
    public int v;

    public const int minH = 0;
    public const int maxH = 359;
    public const int minS = 0;
    public const int maxS = 255;
    public const int minV = 0;
    public const int maxV = 255;

    public Color rgb
    {
        get
        {
            return ToRGB();
        }
        set
        {
            Set(value);
        }
    }

    public float H
    {
        get { return ((float)h) / maxH; }
        set { h = Mathf.RoundToInt(value * maxH); }
    }
    public float S
    {
        get { return ((float)s) / maxS; }
        set { s = Mathf.RoundToInt(value * maxS); }
    }
    public float V
    {
        get { return ((float)v) / maxV; }
        set { v = Mathf.RoundToInt(value * maxV); }
    }

    private int signH = 1;
    private int signS = 1;
    private int signV = 1;

    public HSVColor() { }

    public HSVColor(int h, int s, int v)
    {
        Set(h, s, v);
    }

    public HSVColor(Color color)
    {
        Set(color);
    }

    public HSVColor(HSVColor color)
    {
        Set(color);
    }

    public override string ToString()
    {
        return "(" + h + ", " + s + ", " + v + ")";
    }

    public void Set(int h, int s, int v)
    {
        this.h = h;
        this.s = s;
        this.v = v;
    }

    public void Set(Color color)
    {
        float fH, fS, fV;
        Color.RGBToHSV(color, out fH, out fS, out fV);
        H = fH;
        S = fS;
        V = fV;
    }

    public void Set(HSVColor color)
    {
        Set(color.h, color.s, color.v);
    }

    public Color ToRGB()
    {
        return Color.HSVToRGB(H, S, V);
    }

    public void LoopH(int delta, int min = minH, int max = maxH)
    {
        h += delta;
        int size = max - min;
        while (h > max) h -= size;
        while (h < min) h += size;
    }

    public void PingpongH(int delta, int min = minH, int max = maxH)
    {
        int size = max - min;
        signH = ((delta / size) % 2 == 0) ? 1 : -1;
        delta = delta % size;
        h += signH * Mathf.Abs(delta);
    }

    public void LoopS(int delta, int min = minS, int max = maxS)
    {
        s += delta;
        int size = max - min;
        while (s > max) s -= size;
        while (s < min) s += size;
    }

    public void PingpongS(int delta, int min = minS, int max = maxS)
    {
        int size = max - min;
        signS = ((delta / size) % 2 == 0) ? 1 : -1;
        delta = delta % size;
        s += signS * Mathf.Abs(delta);
    }

    public void LoopV(int delta, int min = minV, int max = maxV)
    {
        v += delta;
        int size = max - min;
        while (v > max) v -= size;
        while (v < min) v += size;
    }

    public void PingpongV(int delta, int min = minV, int max = maxV)
    {
        int size = max - min;
        signV = ((delta / size) % 2 == 0) ? 1 : -1;
        delta = delta % size;
        v += signV * Mathf.Abs(delta);
    }

}

public static class ColorExtension
{

    public static Color NewR(this Color color, float r)
    {
        return new Color(r, color.g, color.b, color.a);
    }

    public static Color NewG(this Color color, float g)
    {
        return new Color(color.r, g, color.b, color.a);
    }

    public static Color NewB(this Color color, float b)
    {
        return new Color(color.r, color.g, b, color.a);
    }

    public static Color NewA(this Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }

    public static HSVColor ToHSV(this Color color)
    {
        return new HSVColor(color);
    }

    public static Color NewH(this Color color, int h)
    {
        HSVColor hsvColor = color.ToHSV();
        hsvColor.h = h;
        return hsvColor.rgb;
    }

    public static Color NewS(this Color color, int s)
    {
        HSVColor hsvColor = color.ToHSV();
        hsvColor.s = s;
        return hsvColor.rgb;
    }

    public static Color NewV(this Color color, int v)
    {
        HSVColor hsvColor = color.ToHSV();
        hsvColor.v = v;
        return hsvColor.rgb;
    }

    public static Color Fade(this Color from, float a, float t)
    {
        return NewA(from, Mathf.Lerp(from.a, a, t));
    }

    public static Color Tint(this Color from, Color to, float t)
    {
        return new Color(
            Mathf.Lerp(from.r, to.r, t),
            Mathf.Lerp(from.g, to.g, t),
            Mathf.Lerp(from.b, to.b, t),
            Mathf.Lerp(from.a, to.a, t));
    }

    public static string ColorToHex(this Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }
}
