using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class EventTriggerExtension {

    public static EventTrigger.Entry FindEntry(this EventTrigger self, EventTriggerType type) {
        return self.triggers.Find(entry => entry.eventID == type);
    }

    public static void BindEvent(this EventTrigger self, EventTriggerType type, UnityAction<BaseEventData> action) {
        EventTrigger.Entry entry = self.FindEntry(type);
        if (entry == null) {
            entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback = new EventTrigger.TriggerEvent();
            self.triggers.Add(entry);
        }
        entry.callback.AddListener(action);
    }

    public static void UnbindEvent(this EventTrigger self, EventTriggerType type, UnityAction<BaseEventData> action) {
        EventTrigger.Entry entry = self.FindEntry(type);
        if (entry != null) {
            entry.callback.RemoveListener(action);
        }
    }
}
