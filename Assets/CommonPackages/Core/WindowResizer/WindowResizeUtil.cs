using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowResizeUtil : MonoBehaviour
{
    [Range(1, 100)]
    [SerializeField] private int _Height = 16;
    [Range(1, 100)]
    [SerializeField] private int _Width = 9;
    [ReadOnly]
    [SerializeField] private float _Aspect = 16f / 9f;

    private void WindowProcess()
    {
        var max = Mathf.Max(Screen.width, Screen.height);
        var width = max / _Height * _Width;
        var height = max;
        _Aspect = (float)_Height / (float)_Width;
        Screen.SetResolution(width, height, false);

        lastWidth = width;
        lastHeight = height;
    }

    private int lastWidth = 0;
    private int lastHeight = 0;

    private void OnGUI()
    {
        if (Mathf.Abs(Screen.width - lastWidth) > 50 || Mathf.Abs(Screen.height - lastHeight) > 50)
        {
            WindowProcess();
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        _Aspect = (float)_Height / (float)_Width;
    }
#endif
}