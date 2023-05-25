using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static SysManager;

public class CLSCTooltip
{
    public GameObject tooltipObj;
    readonly RectTransform tf, icon_tf, title_tf, price_tf, desc_tf, extra_1_tf, extra_2_tf;
    Image icon;
    Text title, desc, price, count, value, extra_1, extra_2;
    DisplayFormat format;
    string targetName;



    public CLSCTooltip(Transform parentTf)
    {
        GameObject tt_icon, tt_title, tt_price, tt_desc, tt_extra_1, tt_extra_2;

        format = DisplayFormat.NONE;

        tooltipObj = new GameObject("Tooltip");
        tt_icon = new GameObject("Icon");
        tt_title = new GameObject("Title");
        tt_price = new GameObject("Price");
        tt_desc = new GameObject("Desc");
        tt_extra_1 = new GameObject("Extra 1");
        tt_extra_2 = new GameObject("Extra 2");

        tooltipObj.transform.SetParent(parentTf, false);
        tt_icon.transform.SetParent(tooltipObj.transform, false);
        tt_title.transform.SetParent(tooltipObj.transform, false);
        tt_price.transform.SetParent(tooltipObj.transform, false);
        tt_desc.transform.SetParent(tooltipObj.transform, false);
        tt_extra_1.transform.SetParent(tooltipObj.transform, false);
        tt_extra_2.transform.SetParent(tooltipObj.transform, false);

        Image background = tooltipObj.AddComponent<Image>();

        tf = background.rectTransform;
        icon = tt_icon.AddComponent<Image>();
        title = tt_title.AddComponent<Text>();
        price = tt_price.AddComponent<Text>();
        desc = tt_desc.AddComponent<Text>();
        extra_1 = tt_extra_1.AddComponent<Text>();
        extra_2 = tt_extra_2.AddComponent<Text>();

        icon_tf = icon.rectTransform;
        title_tf = title.rectTransform;
        price_tf = price.rectTransform;
        desc_tf = desc.rectTransform;
        extra_1_tf = extra_1.rectTransform;
        extra_2_tf = extra_2.rectTransform;

        background.sprite = defaultBox;
        background.type = Image.Type.Sliced;
        background.pixelsPerUnitMultiplier = 15;
        background.color = new Color(1, 1, 1, 0.7f);

        background.raycastTarget = false;
        icon.raycastTarget = false;
        title.raycastTarget = false;
        price.raycastTarget = false;
        desc.raycastTarget = false;
        extra_1.raycastTarget = false;
        extra_2.raycastTarget = false;

        tooltipObj.SetActive(false);
    }

    public void AssignTooltip(GameObject obj, string name, DisplayFormat format)
    {
        EventTrigger trigger = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry enter_event = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        EventTrigger.Entry exit_event = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        enter_event.callback.AddListener((data) => { ShowTooltip(name); });
        exit_event.callback.AddListener((data) => { HideTooltip(); });
        trigger.triggers.Add(enter_event);
        trigger.triggers.Add(exit_event);
    }

    public void CheckFormat(DisplayFormat newFormat)
    {
        if (format != newFormat)
        {
            format = newFormat;
            FormatTooltip();
        }
    }

    public void ShowTooltip(string name)
    {
        targetName = name;

        if (targetName == "Fever")
        {
            CheckFormat(DisplayFormat.FEVER);
        }
        else if (activeProfile.cl.buildingsData.Exists(building => building.Name == targetName))
        {
            BuildingData building = activeProfile.cl.buildingsData.Find(b => b.Name == targetName);

            CheckFormat(DisplayFormat.BUILDING);

            title.text = building.Name;
            price.text = BitNotation.ToBitNotation(building.Price);
            desc.text = building.Desc;
            extra_1.text = $"BPS   |   {BitNotation.ToBitNotation(building.BaseValue * building.Amount)}";
            extra_2.text = $"Value   |   {BitNotation.ToBitNotation(building.AccumulativeValue)}";

            InterfaceTool.FormatRect(tf, tf.sizeDelta,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-340, 220));
        }
        else if (activeProfile.cl.upgradesData.Exists(up => up.Name == targetName))
        {
            UpgradeData upgrade = activeProfile.cl.upgradesData.Find(up => up.Name == targetName);

            CheckFormat(DisplayFormat.UPGRADE);

            title.text = upgrade.Name;
            desc.text = upgrade.Desc;
            price.text = $"$ {upgrade.Price.ToString()}";
            switch (upgrade.Type)
            {
                case UpgradeType.CP:
                    extra_1.text = "[CP]";
                    extra_2.text = $"{BitNotation.ToBitNotation(activeProfile.cl.ClickPower)}";
                    break;
                case UpgradeType.BUILDING:
                    extra_1.text = "[Building]";
                    extra_2.text = ""; // Indicates which building is being affected
                    break;
                case UpgradeType.CPS:
                    extra_1.text = "[BPS]";
                    extra_2.text = $"{BitNotation.ToBitNotation(activeProfile.cl.BitsPerSecond)}";
                    break;
                case UpgradeType.SPECIAL:
                    extra_1.text = "[Misc]";
                    extra_2.text = "Something special may happen ...";
                    break;
            }
        }

        tf.gameObject.SetActive(true);
    }
    public void HideTooltip()
    {
        tf.gameObject.SetActive(false);
    }

    // BUILDING UPDATE IS SLOW
    public void UpdateTooltip(Vector2 mousePosition)
    {
        if (format == DisplayFormat.FEVER) { }
        else if (format == DisplayFormat.BUILDING)
        {
            BuildingData building = activeProfile.cl.buildingsData.Find(b => b.Name == targetName);

            if (building != null)
            {
                price.text = BitNotation.ToBitNotation(building.Price);
                extra_1.text = $"BPS   |   {BitNotation.ToBitNotation(building.BaseValue * building.Amount)}";
                extra_2.text = $"Value   |   {BitNotation.ToBitNotation(building.AccumulativeValue)}";
            }
            
        }
        else if (format == DisplayFormat.UPGRADE)
        {
            tf.position = mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCam.nearClipPlane));

            tf.localPosition = new Vector3(tf.localPosition.x - 235, tf.localPosition.y + 10, -1);
            if (tf.position.x >= Screen.width - (tf.rect.width * 0.5f) - 20)
                tf.position = new Vector3(Screen.width, tf.position.y, -1);
        }
    }

    void FormatTooltip()
    {
        switch (format)
        {
            case DisplayFormat.BUILDING:
            {
                InterfaceTool.FormatRectNPos(tf, new Vector2(550, 250),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
                InterfaceTool.FormatRect(icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -30));
                InterfaceTool.FormatRect(title_tf, new Vector2(165, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -30));
                InterfaceTool.FormatRect(desc_tf, new Vector2(490, 80),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(30, -145));
                InterfaceTool.FormatRect(price_tf, new Vector2(230, 60),
                    new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-30, -30));
                InterfaceTool.FormatRect(extra_1_tf, new Vector2(170, 40),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 25));
                InterfaceTool.FormatRect(extra_2_tf, new Vector2(170, 40),
                    new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 25));

                // Text Formatting

                InterfaceTool.FormatText(title, DEFAULT_FONT, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
                InterfaceTool.FormatText(price, DEFAULT_FONT, 24, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
                InterfaceTool.FormatText(desc, DEFAULT_FONT, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
                InterfaceTool.FormatText(extra_1, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                InterfaceTool.FormatText(extra_2, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                break;
            }
            case DisplayFormat.UPGRADE:
            {
                InterfaceTool.FormatRect(tf, new Vector2(550, 230),
                    new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-670, 310));
                InterfaceTool.FormatRect(icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -30));
                InterfaceTool.FormatRect(title_tf, new Vector2(190, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -30));
                InterfaceTool.FormatRect(desc_tf, new Vector2(490, 70),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20));
                InterfaceTool.FormatRect(price_tf, new Vector2(230, 60),
                    new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-30, -30));
                InterfaceTool.FormatRect(extra_1_tf, new Vector2(100, 40),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(115, 20));
                InterfaceTool.FormatRect(extra_2_tf, new Vector2(220, 40),
                    new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-115, 20));

                // Text Formatting

                InterfaceTool.FormatText(title, DEFAULT_FONT, 24, Color.white, TextAnchor.UpperCenter, FontStyle.Bold);
                InterfaceTool.FormatText(price, DEFAULT_FONT, 24, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
                InterfaceTool.FormatText(desc, DEFAULT_FONT, 20, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                InterfaceTool.FormatText(extra_1, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                InterfaceTool.FormatText(extra_2, DEFAULT_FONT, 18, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                break;
            }
            case DisplayFormat.FEVER:
            {
                FeverData feverData = activeProfile.cl.feverData;

                InterfaceTool.FormatRect(tf, new Vector2(550, 220),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(15, 315));
                InterfaceTool.FormatRect(icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -25));
                InterfaceTool.FormatRect(title_tf, new Vector2(300, 60),
                    new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -25));
                InterfaceTool.FormatRect(desc_tf, new Vector2(490, 70),
                    new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 25));
                InterfaceTool.FormatRect(price_tf, new Vector2(170, 40),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 5));
                InterfaceTool.FormatRect(extra_1_tf, new Vector2(160, 40),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(30, 5));
                InterfaceTool.FormatRect(extra_2_tf, new Vector2(160, 40),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-30, 5));

                // Text Formatting

                InterfaceTool.FormatText(title, DEFAULT_FONT, 36, new Color(1, 0.8f, 0), TextAnchor.UpperCenter, FontStyle.Bold);
                title.text = "FEVER BAR";

                InterfaceTool.FormatText(price, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
                price.text = $"Drain   |   {feverData.Drain * 100}%";

                InterfaceTool.FormatText(extra_1, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                extra_1.text = $"MAX   |   {(feverData.Max / feverData.Gain).ToString("#,0")}";

                InterfaceTool.FormatText(extra_2, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                extra_2.text = $"Persistence   |   {feverData.Persistence * 100}%";

                InterfaceTool.FormatText(desc, DEFAULT_FONT, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                desc.text =
                    $"<i><size=14><color=#cccccc>When active...</color></size></i>\nClick Power Multi : <color=#ffcc00>{feverData.ClickMultiplier * 100f}%</color>\n" +
                    $"BPS Multiplier      : <color=#ffcc00>{feverData.BPSMultiplier * 100f}%</color>";
                break;
            }
        }
    }
}

public enum DisplayFormat
{
    BUILDING,
    UPGRADE,
    FEVER,
    NONE
};