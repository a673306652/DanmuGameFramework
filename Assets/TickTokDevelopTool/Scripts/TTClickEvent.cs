using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TTClickEvent : MonoBehaviour
{
     public static TTClickEvent Instance;
     public UnityEvent OnTouchingUI;

     private void Awake()
     {
          Instance = this;
          OnTouchingUI = new UnityEvent();
     }

     private void Update()
     {
          if (Input.GetKeyDown(KeyCode.Mouse0))
          {
               
              OnTouchingUI.Invoke();
              
          }
     }

     public void AddListenerEvent(UnityAction<GameObject> oc)
     {
    
         OnTouchingUI.AddListener(() =>
         {
              
              OnTouchUI(oc).Invoke();
         }); 
     }

     public UnityAction OnTouchUI(UnityAction<GameObject> oc)
     {
          return () =>
          {
       
               oc.Invoke(GetFirstPickGameObject(Input.mousePosition));
          };
     }

     public GameObject GetFirstPickGameObject(Vector2 position)
     {
          EventSystem eventSystem = EventSystem.current;
          PointerEventData pointerEventData = new PointerEventData(eventSystem);
          pointerEventData.position = position;
          //射线检测ui
          List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
          eventSystem.RaycastAll(pointerEventData, uiRaycastResultCache);
          if (uiRaycastResultCache.Count > 0)
               return uiRaycastResultCache[0].gameObject;
          return null;
     }
}
