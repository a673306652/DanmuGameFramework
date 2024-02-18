using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringFormatter 
{
    
    private static string[] unitA = new []{"","万","亿","兆","八嘎"};

    /// <summary>
    /// 数字加单位
    /// </summary>
    public static string FormatCurrency(float num, int digit = 2)
    {
        var unitList = unitA;
        float tempNum = num;
        long v = 10000;//几位一个单位
        int unitIndex = 0;
        while (tempNum >= v)
        {
            unitIndex++;
            tempNum /= v;
        }
 
        string str = "";
        if (unitIndex >= unitList.Length)
        {
            Debug.LogError("超出单位表中的最大单位");
            str = num.ToString();
        }
        else
        {
            tempNum = Round(tempNum, digit);
            str = $"{tempNum}{unitList[unitIndex]}";
        }
        return str;
    }
 
    /// <summary>
    /// 四舍五入
    /// </summary>
    /// digits:保留几位小数
    public static float Round(float value, int digits = 1)
    {
        float multiple = Mathf.Pow(10, digits);
        float tempValue = value * multiple + 0.5f;
        tempValue = Mathf.FloorToInt(tempValue);
        return tempValue / multiple;
    }
    public class ColorFormat
    {
        public const string Green = "00BB00";
        public const string Red = "FF0000";
        public const string Purple = "BA55D3";
        public const string Blue = "6495ED";
        public const string White = "FFFAFA";
        public const string SP = "98F5FF";
        public const string Dark = "821A7D";
    }
    public static string FormatSkillName(string name, string fmt)
    {
        return $"<color=#{fmt}>{name}</color>";
    }
}
