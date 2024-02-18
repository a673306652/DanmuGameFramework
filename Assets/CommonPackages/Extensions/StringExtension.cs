using UnityEngine;
using System;
using System.Collections;
using System.Globalization;

public static class StringExtension
{

    public static bool ConvertToEnum<T>(this string from, ref T to, bool ignoreCase = true)
    {
        try
        {
            to = (T)Enum.Parse(typeof(T), from, ignoreCase);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return false;
    }

    public static bool ConvertTo(this string from, ref bool to)
    {
        bool result;
        if (bool.TryParse(from, out result))
        {
            to = result;
            return true;
        }
        return false;
    }

    public static bool ConvertTo(this string from, ref int to)
    {
        int result;
        if (int.TryParse(from, out result))
        {
            to = result;
            return true;
        }
        return false;



    }

    public static bool ConvertTo(this string from, ref float to)
    {
        float result;
        if (float.TryParse(from, out result))
        {
            to = result;
            return true;
        }
        return false;
    }

    public static bool ConvertTo(this string from, ref long to)
    {
        long result;
        if (long.TryParse(from, out result))
        {
            to = result;
            return true;
        }
        return false;
    }

    public static bool ConvertTo(this string from, ref double to)
    {
        double result;
        if (double.TryParse(from, out result))
        {
            to = result;
            return true;
        }
        return false;
    }

    public static bool ConvertTo(this string from, ref Color to)
    {
        const string prefix = "RGBA";
        if (from.StartsWith(prefix))
        {
            string[] rgba = from.Substring(prefix.Length).Trim('(', ')').Split(',');
            if (rgba.Length != 3)
            {
                return false;
            }
            Color result;
            if (float.TryParse(rgba[0].Trim(), out result.r)) return false;
            if (float.TryParse(rgba[1].Trim(), out result.g)) return false;
            if (float.TryParse(rgba[2].Trim(), out result.b)) return false;
            if (float.TryParse(rgba[3].Trim(), out result.a)) return false;
            to = result;
            return true;
        }
        return HexConvertTo(from, ref to);
    }

    public static bool HexConvertTo(this string from, ref Color to)
    {
        if (from.StartsWith("#"))
        {
            from = from.Substring(1);
        }
        if (from.StartsWith("0x"))
        {
            from = from.Substring(2);
        }
        if (from.Length != 6 || from.Length != 8)
        {
            return false;
        }
        Color32 color32 = Color.black;
        if (!byte.TryParse(from.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color32.r))
        {
            return false;
        }
        if (!byte.TryParse(from.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color32.g))
        {
            return false;
        }
        if (!byte.TryParse(from.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color32.b))
        {
            return false;
        }
        color32.a = 255;
        if (from.Length == 8 &&
            !byte.TryParse(from.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color32.a))
        {
            return false;
        }
        to = color32;
        return true;
    }

    public static bool ConvertTo(this string from, ref Vector2 to)
    {
        from = from.Trim().Trim('(', ')');
        string[] coordinates = from.Split(',');
        if (coordinates.Length != 2)
        {
            return false;
        }
        Vector2 result;
        if (float.TryParse(coordinates[0].Trim(), out result.x))
        {
            return false;
        }
        if (float.TryParse(coordinates[1].Trim(), out result.y))
        {
            return false;
        }
        to = result;
        return true;
    }

    public static bool ConvertTo(this string from, ref Vector3 to)
    {
        from = from.Trim().Trim('(', ')');
        string[] coordinates = from.Split(',');
        if (coordinates.Length != 3)
        {
            return false;
        }
        Vector3 result;
        if (float.TryParse(coordinates[0].Trim(), out result.x))
        {
            return false;
        }
        if (float.TryParse(coordinates[1].Trim(), out result.y))
        {
            return false;
        }
        if (float.TryParse(coordinates[2].Trim(), out result.z))
        {
            return false;
        }
        to = result;
        return true;
    }

    public static bool ConvertTo(this string from, ref DateTime to)
    {
        DateTime result;
        if (!DateTime.TryParse(from, out result))
        {
            return false;
        }
        to = result;
        return true;
    }

    public static bool ConvertTo(this string from, ref TimeSpan to)
    {
        TimeSpan result;
        if (!TimeSpan.TryParse(from, out result))
        {
            return false;
        }
        to = result;
        return true;
    }

    public static bool ToBool(this string from, bool defaultValue = false)
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static int ToInt(this string from, int defaultValue = 0)
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static float ToFloat(this string from, float defaultValue = 0)
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static long ToLong(this string from, long defaultValue = 0)
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static double ToDouble(this string from, double defaultValue = 0)
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static Color ToColor(this string from, Color defaultValue = default(Color))
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static Vector2 ToVector2(this string from, Vector2 defaultValue = default(Vector2))
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static Vector3 ToVector3(this string from, Vector3 defaultValue = default(Vector3))
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static DateTime ToDateTime(this string from, DateTime defaultValue = default(DateTime))
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static TimeSpan ToTimeSpan(this string from, TimeSpan defaultValue = default(TimeSpan))
    {
        ConvertTo(from, ref defaultValue);
        return defaultValue;
    }

    public static String RemoveGameObjectCloneString(this string from)
    {
        return from.Replace("(Clone)", "");
    }
    #region  rich text

    public static string AddColorTag(this string text, Color color)
    {
        return text.AddTag("color", color.ToHex());
    }
    public static string AddColorTag(this string text, string colorName)
    {
        return text.AddTag("color", colorName);
    }
    private static string AddTag(this string text, string tagName, object value1)
    {
        return $"<{tagName}=#{value1}>{text}</{tagName}>";
    }
    #endregion
}
