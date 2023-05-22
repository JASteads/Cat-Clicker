using UnityEngine;
using UnityEngine.UI;

using static SysManager;

public class CLSCFeverSystem
{
    // public delegate void On_Fever();
    CLSCFeverMeter feverMeter, feverMeterSecondary;

    GameObject feverCanvas;
    Text feverText;
    
    public FeverMode Mode { get; set; }

    public int BarsMax { get; set; }
    public int BarsFilled { get; set; }
    public float FeverPoints { get; set; }
    public float Duration { get; set; }
    public bool isActive;
    
    public FeverData FeverData { get; }

    const float CONSTANT_DEPLETION = 0.04f;



    public CLSCFeverSystem()
    {
        Mode = FeverMode.NORMAL;

        BarsMax = 1;
        BarsFilled = 0;

        FeverPoints = 0;
        Duration = 0;

        isActive = false;

        FeverData = profile.clscSaveData.feverData;
    }

    public void UpdateSystem()
    {
        if (FeverPoints > 0)
        {
            FeverPoints -= CONSTANT_DEPLETION;
            if (Duration <= 0)
            {
                if (BarsFilled == 0)
                {
                    if (FeverPoints - FeverData.Drain > 0)
                        FeverPoints -= FeverData.Drain;
                    else
                        FeverPoints = 0;
                }
                else
                {
                    if (FeverPoints - FeverData.Drain > 0)
                        FeverPoints -= FeverData.Drain;
                    else
                    {
                        --BarsFilled;
                        FeverPoints = FeverData.Max - 1;
                    }
                }
            }

            // Stop FEVER if below 95%.
            if (isActive && FeverPoints <= 95 && BarsFilled < 1)
                isActive = false;
        }

        if (Duration > 0)
        {
            --Duration;
        }

        feverMeter.transform.localScale = new Vector2(1, isActive ? 1 : FeverPoints * .01f);
        feverMeterSecondary.transform.localScale = new Vector2(1, BarsFilled > 0 && BarsMax > 1 ? FeverPoints * .01f : 0);

        if (isActive)
        {
            feverText.gameObject.SetActive(true);
            feverMeter.feverBar.color = (BarsFilled > 1) ?
                new Color(0.9f, 0.7f - ((BarsFilled - 1) * 0.0825f), 0) : new Color(1, 1, 0.1f);
            feverMeterSecondary.feverBar.color = new Color(0.9f, 0.7f - (BarsFilled * 0.0825f), 0);

            ++FeverData.TimeActive;
        }
        else
        {
            feverText.gameObject.SetActive(false);
            feverMeter.feverBar.color =
                new Color(0.6f + (FeverPoints * 0.003f), FeverPoints * 0.004f, 0);
        }
    }

    public void FuelFever()
    {
        FeverPoints += FeverData.Gain; // * (float)System.Math.Pow(1.1f, bars_filled);

        if (FeverPoints > FeverData.Max)
        {
            if (!isActive)
            {
                isActive = true;
            }
            if (BarsFilled < BarsMax && BarsMax > 1)
            {
                ++BarsFilled;
                FeverPoints = 0;
            }
            else
                FeverPoints = FeverData.Max;
        }

        // Reset fever drain time
        Duration = 15 * FeverData.Persistence;
    }

    public void CreateFeverSystem(Transform parentTf, CLSCTooltip tooltip)
    {
        GameObject fever_text_obj, feverBar, points, indicator, indicator2;

        feverMeter = new CLSCFeverMeter();
        feverMeterSecondary = new CLSCFeverMeter();

        feverCanvas = InterfaceTool.CanvasSetup("Fever Canvas", parentTf, out Canvas canvas);

        feverBar = new GameObject("Fever Meter");
        feverBar.transform.SetParent(feverCanvas.transform, false);
        feverBar.transform.rotation = Quaternion.Euler(0, 0, 270);
        Image bar_img = feverBar.AddComponent<Image>();
        InterfaceTool.FormatRect(bar_img.rectTransform, new Vector2(100, 250),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(125, 250));
        bar_img.color = new Color(0.28f, 0.24f, 0.3f);

        points = new GameObject("Fever Points");
        points.transform.SetParent(feverBar.transform, false);
        points.AddComponent<Image>().color = new Color(0.4f, 0.36f, 0.4f);
        InterfaceTool.FormatRectNPos(points.GetComponent<RectTransform>(), new Vector2(-30, -30),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        tooltip.AssignTooltip(points, "Fever", DisplayFormat.FEVER);

        indicator = new GameObject("Fever Bar");
        feverMeter.feverBar = indicator.AddComponent<Image>();
        feverMeter.feverBar.color = new Color(0.9f, 0.7f, 0);
        feverMeter.transform = feverMeter.feverBar.rectTransform;

        indicator2 = new GameObject("Secondary Fever Bar");
        feverMeterSecondary.feverBar = indicator2.AddComponent<Image>();
        feverMeterSecondary.feverBar.color = new Color(0.9f, 0.6f, 0);
        feverMeterSecondary.transform = feverMeterSecondary.feverBar.rectTransform;

        feverMeter.transform.SetParent(points.transform, false);
        InterfaceTool.FormatRectNPos(feverMeter.transform, new Vector2(70, 220),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        feverMeter.feverBar.raycastTarget = false;

        feverMeterSecondary.transform.SetParent(points.transform, false);
        InterfaceTool.FormatRectNPos(feverMeterSecondary.transform, new Vector2(70, 220),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        feverMeterSecondary.feverBar.raycastTarget = false;

        fever_text_obj = new GameObject("FEVER Text");
        feverText = fever_text_obj.AddComponent<Text>();
        feverText.rectTransform.SetParent(feverCanvas.transform, false);
        feverText.rectTransform.localPosition = new Vector2(0, 250);
        feverText.rectTransform.sizeDelta = new Vector2(500, 100);
        feverText.raycastTarget = false;

        InterfaceTool.FormatText(feverText, DEFAULT_FONT, 64, new Color(0.9f, 0.7f, 0), TextAnchor.MiddleCenter, FontStyle.Bold);
        feverText.text = "FEVER";
    }

}

public enum FeverMode
{
    NORMAL,
    ETHER,
    ANGELICAT,
    EVOLUTION
}