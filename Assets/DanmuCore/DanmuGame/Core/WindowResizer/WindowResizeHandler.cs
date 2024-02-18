using UnityEngine;
using UnityEngine.EventSystems;

public class WindowResizeHandler : MonoBehaviour
{
    bool isDragging = false;
    bool isInsideOfHandler = false;
    private float aspect = 16f / 9f;
    private void WindowProcess()
    {
        var max = Mathf.Max(Screen.width, Screen.height);
        var width = max / 16 * 9;
        var height = max;
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
}