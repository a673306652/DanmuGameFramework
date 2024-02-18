using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
public static class Vibration
{

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#elif UNITY_IOS || UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern bool _HasVibrator();

    [DllImport("__Internal")]
    private static extern void _Vibrate();

    [DllImport("__Internal")]
    private static extern void _VibratePop();

    [DllImport("__Internal")]
    private static extern void _VibratePeek();

    [DllImport("__Internal")]
    private static extern void _VibrateNope();
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif

    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        vibrator.Call("vibrate");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        _Vibrate();
#elif UNITY_STANDALONE_WIN
#else
        Handheld.Vibrate();
#endif       
    }

    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        vibrator.Call("vibrate", milliseconds);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        if (WEAK.Equals(milliseconds))
        {
            _VibratePop();
        }
        else if (MEDIUM.Equals(milliseconds))
        {
            _VibratePeek();
        }
        else if (LONG.Equals(milliseconds))
        {
            _VibrateNope();
        }
#elif UNITY_STANDALONE_WIN
#else
        Handheld.Vibrate();
#endif       
    }

    public static void Vibrate(long [] pattern, int repeat)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        vibrator.Call("vibrate", pattern, repeat);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        if (repeat > 1)
        {
            _VibrateNope();
        }
        else
        {
            _VibratePop();
        }
#elif UNITY_STANDALONE_WIN
#else
        Handheld.Vibrate();
#endif       
    }

    public static bool HasVibrator()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        return _HasVibrator();
#else
        return false;
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITYEDITOR
        vibrator.Call("cancel");
#endif
    }

    private static bool IsAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    private static bool IsIOS()
    {
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    public static readonly long WEAK = 50L;
    public static readonly long MEDIUM = 200L;
    public static readonly long LONG = 500L;
    public static readonly long EXTRA = 1000L;
}