using UnityEngine;

public class CLClickSys : CLSubSys
{
    public CLClickSys(CLSystem sys, Transform parent)
        : base(sys, parent)
    {
        
    }

    protected override void Init(CLSystem sys, Transform parent)
    {
        panel = new CLClickPanel(sys, parent);
        onUpdate = panel.onRefresh;
        onClick = (amount) => ClickPopup(amount);
    }

    public void ClickPopup(double bitAmount)
    {
        
    }
}