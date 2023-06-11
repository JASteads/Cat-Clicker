using UnityEngine;
using UnityEngine.UI;

public class CLClickPanel : CLSubPanel
{
    Text bitCounter, bpsCounter;


    public CLClickPanel(CLSystem sys, Transform parent)
        : base("Click Panel", sys, parent)
    {

    }

    void UpdateText(CLSystem sys)
    {
        bitCounter.text = $"Bits : " +
            BitNotation.Format(sys.GetCurrencyCurrent());
        bpsCounter.text = $"Bits per second : " +
            BitNotation.Format(sys.GetBPS(false));
    }

    protected override void Create(CLSystem sys)
    {
        const int FONT_SIZE = 42;

        Vector2   bpsCSize  = new Vector2(530, 40),
                  clickSize = new Vector2(270, 90),
                  clickPos  = new Vector2(180, -300),
                  bpsPos    = new Vector2(0, 30),
                  bitPos    = new Vector2(-80, 0);

        int bpsFontSize = Mathf.RoundToInt(FONT_SIZE * 0.75f),
            bitCHeight  = Mathf.RoundToInt(bpsCSize.y * 1.5f);

        Transform click = InterfaceTool.ButtonSetup("Click Button",
            transform, out Image clickImg, out Button clickButton,
            SysManager.uiSprites[3], sys.OnClick);
        InterfaceTool.FormatRect(clickImg, clickSize,
            Vector2.up, Vector2.up, Vector2.up, clickPos);
        InterfaceTool.CreateBody("CLICK", click, FONT_SIZE);

        bpsCounter = InterfaceTool.CreateHeader("", click, 
            (int)bpsCSize.y, bpsPos, bpsFontSize);
        bpsCounter.rectTransform.sizeDelta = bpsCSize;

        bitCounter = InterfaceTool.CreateHeader("",
            bpsCounter.transform, bitCHeight, bitPos, FONT_SIZE);
        bitCounter.fontStyle = FontStyle.Bold;

        onRefresh = () => UpdateText(sys);
    }
}
