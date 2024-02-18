using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(RectTransform rect);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;
    public RectTransform rect;
    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        listener.rect = go.GetRectTransform();
        return listener;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (this.onBeginDrag != null) this.onBeginDrag(rect);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (this.onDrag != null) this.onDrag(rect);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (this.onEndDrag != null) this.onEndDrag(rect);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(rect);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(rect);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(rect);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(rect);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(rect);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(rect);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(rect);
    }
}