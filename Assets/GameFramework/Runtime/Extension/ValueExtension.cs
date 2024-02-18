//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;


//public static partial class ValueExtension
//{
//    public static bool InRange(this float value, float min, float max)
//    {
//        return value > min && value < max;
//    }

//    public static bool OutRange(this float value, float min, float max)
//    {
//        return value < min || value > max;
//    }

//    public static bool InRange(this int value, int min, int max)
//    {
//        return value > min && value < max;
//    }

//    public static bool OutRange(this int value, int min, int max)
//    {
//        return value < min && value > max;
//    }


//    public static int ToInt(this string str, bool allowException = true)
//    {
//        int reslut = -1;
//        if (allowException)
//        {
//            reslut = int.Parse(str);
//        }
//        else
//        {
//            int.TryParse(str, out reslut);
//        }
//        return reslut;
//    }
//    public static bool ToBool(this string str)
//    {
//        return bool.Parse(str);
//    }
//}
