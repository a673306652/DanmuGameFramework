using UnityEngine;
public static class DebugExt
{
    public static void Network(string logStr)
    {
#if NETWORK_DEBUG
        Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", "C71585", "Network", logStr));
#endif
    }

    public static void Config(string logStr)
    {
#if CONFIG_DEBUG
        Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", "2E8B57", "Config", logStr));
#endif
    }

    public static void Ads(string logStr)
    {
#if ADS_DEBUG
        Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", "FF4500", "Ads", logStr));
#endif
    }

    public static void IAP(string logStr)
    {
#if IAP_DEBUG
        Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", "0096C7", "IAP", logStr));
#endif
    }

    public static void Temp(string logStr)
    {
#if TEMP_DEBUG
        Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", "60D499", "Temp", logStr));
#endif
    }
}