using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CLBuildingPanel : CLSubPanel
{
    const int BUILDING_SPACING = 210,
              LIST_LEN = 310;

    Image panelBG;
    Button seekButtonL, seekButtonR;
    List<CLBuildingButton> buttons;


    public CLBuildingPanel(CLSystem sys, Transform parent)
        : base("Buildings Panel", sys, parent)
    {

    }

    public void UpdateSeekButtons(RectTransform tf)
    {
        int newPos = (int)tf.anchoredPosition.x;

        seekButtonL.interactable = newPos != LIST_LEN;
        seekButtonR.interactable = newPos >= 
            -(LIST_LEN + tf.sizeDelta.x);
    }

    void SeekShop(RectTransform tf, bool forward)
    {
        tf.localPosition =
            new Vector2(tf.localPosition.x +
            (forward ? -BUILDING_SPACING : BUILDING_SPACING),
            tf.localPosition.y);
        UpdateSeekButtons(tf);
    }

    protected override void Create(CLSystem sys)
    {
        Transform panelL, panelR, seekL, seekR;

        const int SHOP_HEIGHT = 200,
                  SEEK_OFFSET = 35,
                  S_FONT_SIZE = 60;

        Vector2 shopSize  = new Vector2(0, SHOP_HEIGHT),
                listSize  = new Vector2(LIST_LEN * 2, 0),
                panelSize = new Vector2(LIST_LEN, SHOP_HEIGHT),
                seekSize  = new Vector2(70, 150);

        Color   cPanel    = new Color(0.8f, 0.8f, 0.8f),
                cPanelBG  = new Color(0.6f, 0.6f, 0.6f),
                cDisabled = new Color(0.5f, 0.5f, 0.5f, 1),
                sColor    = Color.white;

        Sprite shopSprite = SysManager.uiSprites[2];
        Sprite buttonSprite = SysManager.uiSprites[3];
;

        Transform buildingShop = InterfaceTool.ImgSetup(
            "Building Shop", transform, out panelBG,
            shopSprite, true);
        InterfaceTool.FormatRectNPos(panelBG, shopSize,
            Vector2.zero, Vector2.right, Vector2.zero);
        panelBG.color = cPanelBG;
        panelBG.raycastTarget = false;

        RectTransform list = new GameObject("List")
            .AddComponent<RectTransform>();
        list.SetParent(buildingShop, false);
        InterfaceTool.FormatRect(list, listSize,
            Vector2.zero, Vector2.one, new Vector2(0, 0.5f),
            new Vector2(panelSize.x, 0));

        panelL = InterfaceTool.ImgSetup("Building Panel L",
            transform, out Image panelImgL,
            SysManager.defaultBox, true);
        InterfaceTool.FormatRectNPos(panelImgL, panelSize,
            Vector2.zero, Vector2.zero, Vector2.zero);
        panelImgL.color = cPanel;
        panelImgL.raycastTarget = false;

        panelR = InterfaceTool.ImgSetup("Building Panel R",
            transform, out Image panelImgR,
            SysManager.defaultBox, true);
        InterfaceTool.FormatRectNPos(panelImgR, panelSize,
            Vector2.right, Vector2.right, Vector2.right);
        panelImgR.color = cPanel;
        panelImgR.raycastTarget = false;

        seekL = InterfaceTool.ButtonSetup("Seek L", panelL,
            out Image seekImgL, out seekButtonL, buttonSprite, null);
        seekButtonL.onClick.AddListener(() => 
            SeekShop(list, false));
        InterfaceTool.FormatRect(seekImgL, seekSize,
            new Vector2(1, 0.5f), new Vector2(1, 0.5f),
            new Vector2(1, 0.5f), new Vector2(-SEEK_OFFSET, 0));
        InterfaceTool.CreateBody("<", seekL, S_FONT_SIZE);

        seekR = InterfaceTool.ButtonSetup("Seek R", panelR,
            out Image seekImgR, out seekButtonR, buttonSprite, null);
        seekButtonR.onClick.AddListener(() => 
            SeekShop(list, true));
        InterfaceTool.FormatRect(seekImgR, seekSize,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f),
            new Vector2(0, 0.5f), new Vector2(SEEK_OFFSET, 0));
        InterfaceTool.CreateBody(">", seekR, S_FONT_SIZE);

        ColorBlock seekColors = seekButtonL.colors;
        seekColors.disabledColor = cDisabled;

        seekButtonL.colors = seekButtonR.colors = seekColors;
        seekImgL.color = seekImgR.color = sColor;


        CreateBuildingButtons(sys, list);
        UpdateSeekButtons(list);
    }

    void CreateBuildingButtons(CLSystem sys, RectTransform parent)
    {
        List<CLBuildingData> data = sys.data.buildingData;

        if (data == null) Debug.Log("No data loaded ..");

        buttons = new List<CLBuildingButton>();

        const int OFFSET = 15;

        for (int i = 0; i < data.Count; i++)
        {
            Vector2 newPos = new Vector2(
                (i * BUILDING_SPACING) + OFFSET, 0);

            buttons.Add(new CLBuildingButton(data[i], parent,
                sys.UpdateBPS));
            buttons[i].transform.anchoredPosition = newPos;

            GameObject button = buttons[i].transform.gameObject;
            sys.AssignTooltip(button);
            button.SetActive(true);
        }
    }
}
