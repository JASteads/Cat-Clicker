using UnityEngine;
using UnityEngine.UI;

public class CLBuildingButton
{
    public CLBuildingData target;
    public RectTransform transform;
    public bool active;

    Image image;
    Button button;
    Text name, price, count;
    

    public CLBuildingButton(CLBuildingData target, Transform parent,
        UnityEngine.Events.UnityAction onBuy)
    {
        Sprite buttonSprite = SysManager.uiSprites[3];

        this.target = target;


        InterfaceTool.ButtonSetup($"{target.Name} Button",
            parent, out image, out button, buttonSprite, onBuy);
        InterfaceTool.FormatRectNPos(image, new Vector2(200, 150),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f),
            new Vector2(0, 0.5f));

        transform = image.rectTransform;

        InterfaceTool.TextSetup("Name", image.transform,
            out name, false);
        InterfaceTool.FormatRectNPos(name, new Vector2(0, 50),
            Vector2.zero, Vector2.right, new Vector2(0.5f, 0));
        InterfaceTool.FormatText(name, SysManager.DEFAULT_FONT, 20,
            Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        name.text = target.Name;

        InterfaceTool.TextSetup("Count", button.transform,
            out count, false);
        InterfaceTool.FormatRect(count, new Vector2(-30, 50),
            Vector2.up, Vector2.one, new Vector2(0.5f, 0.5f),
            new Vector2(0, -20));
        InterfaceTool.FormatText(count, SysManager.DEFAULT_FONT,
            16, Color.black, TextAnchor.MiddleLeft,
            FontStyle.Normal);
        count.text = BitNotation.Format(target.Amount);

        InterfaceTool.TextSetup("Price", name.transform,
            out price, false);
        InterfaceTool.FormatRectNPos(price, new Vector2(-30, 20),
            Vector2.up, Vector2.one, new Vector2(0.5f, 0));
        InterfaceTool.FormatText(price, SysManager.DEFAULT_FONT, 16,
            Color.black, TextAnchor.MiddleRight, FontStyle.Normal);
        price.text = "$ 0";
    }
}