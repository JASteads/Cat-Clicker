using System.Collections.Generic;
using UnityEngine;

public class CLSCStatusMessagesList : MonoBehaviour
{
    RectTransform listTf;
    public List<CLSCStatusMessage> MessageList { get; set; }
    Vector2 SizeDelta { get; set; }
    int FontSize { get; set; }
    string Name  { get; set; }
    float Spacing { get; set; }


    
    public void SetupList(string name, int fontSize, 
        Transform parentTf, Vector2 sizeDelta, Vector2 position)
    {
        MessageList = new List<CLSCStatusMessage>();
        Name = name;
        SizeDelta = sizeDelta;
        FontSize = fontSize;
        Spacing = fontSize + 10;
        listTf = new GameObject($"{name} List").AddComponent<RectTransform>();

        listTf.sizeDelta = new Vector2();

        listTf.SetParent(parentTf.transform, false);
        listTf.anchoredPosition = position;
    }
    public void SetupList(string name, int fontSize, float spacing,
        Transform parentTf, Vector2 sizeDelta, Vector2 position)
    {
        MessageList = new List<CLSCStatusMessage>();
        Name = name;
        SizeDelta = sizeDelta;
        FontSize = fontSize;
        Spacing = spacing;
        listTf = new GameObject($"{name} List").AddComponent<RectTransform>();

        listTf.sizeDelta = new Vector2();

        listTf.SetParent(parentTf.transform, false);
        listTf.anchoredPosition = position;
    }

    public void Broadcast(string msg, StatusType type)
    {
        CLSCStatusMessage newMessage = new CLSCStatusMessage($"{Name} #{MessageList.Count}", msg,
            listTf, type, FontSize);

        newMessage.message.rectTransform.sizeDelta = SizeDelta;
        newMessage.message.rectTransform.anchoredPosition = new Vector2(0, Spacing * MessageList.Count);
        MessageList.Add(newMessage);
    }
    public void Broadcast(string msg, StatusType type, int duration)
    {
        CLSCStatusMessage newMessage = new CLSCStatusMessage($"{Name} #{MessageList.Count}", msg, 
            listTf, type, FontSize, duration);

        newMessage.message.rectTransform.sizeDelta = SizeDelta;
        newMessage.message.rectTransform.anchoredPosition = new Vector2(0, Spacing * MessageList.Count);
        MessageList.Add(newMessage);
    }

    void RemoveMessage(CLSCStatusMessage messageToRemove)
    {
        Destroy(messageToRemove.message.gameObject);
        MessageList.Remove(messageToRemove);
    }

    void FixedUpdate()
    {
        if (MessageList.Count != 0)
        {
            foreach(CLSCStatusMessage message in MessageList)
            {
                if (--message.Duration == 0)
                {
                    RemoveMessage(message);
                    break;
                }
            }
        }
    }
}