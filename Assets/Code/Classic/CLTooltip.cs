using UnityEngine;
using UnityEngine.EventSystems;

public class CLTooltip
{
    public void Assign(GameObject obj)
    {
        EventTrigger trigger = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry enter_event = 
            new EventTrigger.Entry 
            { eventID = EventTriggerType.PointerEnter };
        EventTrigger.Entry exit_event = 
            new EventTrigger.Entry 
            { eventID = EventTriggerType.PointerExit };
        enter_event.callback.AddListener(
            (data) => { Display(data as PointerEventData); });
        exit_event.callback.AddListener(
            (data) => { Hide(data as PointerEventData); });
        trigger.triggers.Add(enter_event);
        trigger.triggers.Add(exit_event);
    }

    public void Update(Vector3 newPos)
    {
        if (!IsActive()) return;
    }

    public bool IsActive()
    {
        return false;
    }

    void Display(PointerEventData data)
    {

    }

    void Hide(PointerEventData data)
    {

    }
}
