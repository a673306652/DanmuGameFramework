using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class DateTimeExtension
{
    public static string ToNormalString(this DateTime self)
    {
        return self.ToString("yyyy - MM - dd hh: mm:ss");
    }
}
