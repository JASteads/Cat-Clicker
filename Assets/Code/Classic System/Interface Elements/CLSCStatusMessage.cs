using UnityEngine;
using UnityEngine.UI;

public class CLSCStatusMessage
{
    // This is 300 ms, or 5 seconds
    const int DEFAULTDURATION = 300;

    public int Duration { get; set; }
    public Text message;
    public StatusType msgType;



    public CLSCStatusMessage(string name, string msg, Transform parentTf,
        StatusType type, int fontSize)
    {
        Duration = DEFAULTDURATION;
        msgType = type;

        GameObject messageObj = InterfaceTool.Text_Setup(name, parentTf,
            out message, false);
        InterfaceTool.Format_Text(message, SysManager.defaultFont, fontSize,
            Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);

        message.text = msg;

        switch (msgType)
        {
            case StatusType.NEWS:
                message.color = Color.white;
                break;
            case StatusType.WARNING:
                message.color = Color.red;
                break;
            case StatusType.BONUS:
                message.color = Color.yellow;
                break;
        }
    }

    public CLSCStatusMessage(string name, string msg, Transform parentTf, 
        StatusType type, int fontSize, int duration)
    {
        Duration = duration;
        msgType = type;

        GameObject messageObj = InterfaceTool.Text_Setup(name, parentTf,
            out message, false);
        InterfaceTool.Format_Text(message, SysManager.defaultFont, fontSize,
            Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);

        message.text = msg;

        switch (msgType)
        {
            case StatusType.NEWS:
                message.color = Color.white;
                break;
            case StatusType.WARNING:
                message.color = Color.red;
                break;
            case StatusType.BONUS:
                message.color = Color.yellow;
                break;
        }
    }
}

public enum StatusType
{
    NEWS,
    WARNING,
    BONUS
}