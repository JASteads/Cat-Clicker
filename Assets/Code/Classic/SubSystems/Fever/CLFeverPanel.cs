using UnityEngine;
using UnityEngine.UI;

public class CLFeverPanel : CLSubPanel
{
    public float fuelPercent, limit;
    public bool isActive;

    Image fuel;
    Text txt;

    Color minColor, maxColor, fullColor;


    public CLFeverPanel(CLSystem sys, Transform parent)
        : base("Fever Panel", sys, parent)
    {
        minColor = new Color(0.6f, 0, 0);
        maxColor = new Color(0.6f, 0.6f, 0.05f);
        fullColor = new Color(1, 1, 0.1f);
        isActive = false;
    }

    public void UpdateBar()
    {
        fuel.rectTransform.localScale = new Vector2(fuelPercent, 1);
        fuel.color = isActive ? fullColor : Color.Lerp(
            minColor, maxColor, fuelPercent);
        txt.gameObject.SetActive(isActive);
    }

    protected override void Create(CLSystem sys)
    {
        Transform meter, bar;

        Vector2 mSize = new Vector2(250, 100),
                bSize = new Vector2(-30, -30),
                tSize = new Vector2(500, 100),
                mPos  = new Vector2(0, 200),
                tPos  = new Vector2(0, 250);

        Color   cMeter = new Color(0.28f, 0.24f, 0.3f),
                cBar   = new Color(0.4f, 0.36f, 0.4f),
                cTxt   = new Color(0.9f, 0.7f, 0);

        meter = InterfaceTool.ImgSetup("Meter", transform,
            out Image mImg, true);
        InterfaceTool.FormatRect(mImg, mSize, Vector2.zero,
            Vector2.zero, Vector2.zero, mPos);
        mImg.color = cMeter;

        bar = InterfaceTool.ImgSetup("Bar", meter,
            out Image bImg, false);
        InterfaceTool.FormatRect(bImg);
        bImg.rectTransform.sizeDelta = bSize;
        bImg.color = cBar;

        InterfaceTool.ImgSetup("Fuel", bar, out fuel, false);
        InterfaceTool.FormatRect(fuel);
        fuel.rectTransform.pivot = Vector2.zero;

        InterfaceTool.TextSetup("FEVER Text", transform,
            out txt, false);
        InterfaceTool.FormatRect(txt, tSize, tPos);
        InterfaceTool.FormatText(txt, SysManager.DEFAULT_FONT, 64,
            cTxt, TextAnchor.MiddleCenter, FontStyle.Bold);
        txt.text = "FEVER";

        onRefresh = UpdateBar;
    }
}
