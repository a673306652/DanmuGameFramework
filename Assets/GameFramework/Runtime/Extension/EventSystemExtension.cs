using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class EventSystemExtension
{

    public static GameObject GetPointerOverUIObject(this EventSystem self)
    {
        PointerEventData currentPositionEventData = new PointerEventData(self);
        currentPositionEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        self.RaycastAll(currentPositionEventData, results);
        for (int i = 0; i < results.Count; ++i)
        {
            if (results[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return results[i].gameObject;
            }
        }
        return null;
    }

    public static bool IsPointerOverUIObject(this EventSystem self)
    {
        PointerEventData currentPositionEventData = new PointerEventData(self);
        currentPositionEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        self.RaycastAll(currentPositionEventData, results);
        for (int i = 0; i < results.Count; ++i)
        {
            if (results[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsPointerOverUIObject(this EventSystem self, GameObject uiObject)
    {
        PointerEventData currentPositionEventData = new PointerEventData(self);
        currentPositionEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        self.RaycastAll(currentPositionEventData, results);
        for (int i = 0; i < results.Count; ++i)
        {
            if (results[i].gameObject == uiObject)
            {
                return true;
            }
        }
        return false;
    }
}
