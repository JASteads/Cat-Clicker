using UnityEngine;

public abstract class CLSubPanel
{
    public Transform transform;
    public System.Action onRefresh;

    public CLSubPanel(string name, CLSystem sys, Transform parent)
    {
        transform = InterfaceTool.CanvasSetup(name, parent, out _);
        InterfaceTool.FormatRect(transform
            .GetComponent<RectTransform>());
        
        onRefresh = null;
        Create(sys);
    }

    protected abstract void Create(CLSystem sys);
}