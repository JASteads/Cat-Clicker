using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static SysManager;

public class CLSCBuildings : MonoBehaviour
{
    CLSaveData saveData;

    public GameObject buildingsCanvas;
    public GameObject buildingShop;

    // Building Panel Objects
    RectTransform buildingListTf;
    List<CLSCBuildingButton> buildingButtons = new List<CLSCBuildingButton>();
    Image panelBackground;
    Button seekButtonL, seekButtonR;
    int panelLength;
    


    public void UpdateSystem()
    {
        float currentBits = activeProfile.cl.GetCurrencyCurrent();
        float totalBits = activeProfile.cl.GetCurrencyTotal();
        Button targetButton;

        for (int i = 0; i < buildingButtons.Count; i++)
        {
            targetButton = buildingButtons[i].button;

            if (targetButton.gameObject.activeSelf)
            {
                if (currentBits < saveData.buildingsData[i].Price)
                {
                    if (targetButton.IsInteractable())
                        targetButton.interactable = false;
                }
                else if (!targetButton.IsInteractable())
                    targetButton.interactable = true;
            }
        }

        // Skip first button; it should always be unlocked
        for (int i = 1; i < buildingButtons.Count; i++)
        {
            targetButton = buildingButtons[i].button;

            if (!targetButton.gameObject.activeSelf)
            {
                if (totalBits >= saveData.buildingsData[i].Price)
                {
                    UpdateBuildingsList();

                    buildingButtons[i].active = true;
                    targetButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public void CreateBuildingShop(Transform parentTf, CLSCTooltip tooltip)
    {
        GameObject panelL, panelR, seekL, seekR;
        GameObject seekTxtContainerL, seekTxtContainerR;

        const int BUTTON_HEIGHT = 150;

        saveData = activeProfile.cl;

        buildingsCanvas = InterfaceTool.CanvasSetup("Buildings Canvas", parentTf, out Canvas canvas);
        InterfaceTool.FormatRect(buildingsCanvas.GetComponent<RectTransform>());

        // Length of shop panels (340 * 2), plus the width of each building and respective spacing,
        // minus the spacing for the first and last button.
        panelLength = 680 + (saveData.buildingsData.Count * 210) - 20;

        buildingShop = InterfaceTool.ImgSetup("Building Shop", buildingsCanvas.transform, out panelBackground, uiSprites[2], true);
        InterfaceTool.FormatRect(panelBackground.rectTransform, new Vector2(0, 200),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 0));
        panelBackground.color = new Color(0.6f, 0.6f, 0.6f);
        panelBackground.raycastTarget = false;

        buildingListTf = new GameObject("List").AddComponent<RectTransform>();
        buildingListTf.SetParent(buildingShop.transform, false);
        InterfaceTool.FormatRect(buildingListTf, new Vector2(620, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 0.5f), new Vector2(310, 0));

        panelL = InterfaceTool.ImgSetup("Building Panel Left", buildingsCanvas.transform, out Image pan_img_l, defaultBox, true);
        InterfaceTool.FormatRect(pan_img_l.rectTransform, new Vector2(310, 200),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
        pan_img_l.color = new Color(0.8f, 0.8f, 0.8f);
        pan_img_l.raycastTarget = false;

        panelR = InterfaceTool.ImgSetup("Building Panel Right", buildingsCanvas.transform, out Image pan_img_r, defaultBox, true);
        InterfaceTool.FormatRect(pan_img_r.rectTransform, new Vector2(310, 200),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0, 0));
        pan_img_r.color = new Color(0.8f, 0.8f, 0.8f);
        pan_img_r.raycastTarget = false;

        seekL = InterfaceTool.ButtonSetup("Seek Left", panelL.transform, out Image seek_img_l, out seekButtonL, uiSprites[3], () => SeekShop(buildingListTf, false));
        InterfaceTool.FormatRect(seek_img_l.rectTransform, new Vector2(70, BUTTON_HEIGHT),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-35, 0));
        ColorBlock seek_colors_l = seekButtonL.colors;
        seek_img_l.color = Color.white;
        seek_colors_l.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1);
        seekButtonL.colors = seek_colors_l;

        seekTxtContainerL = InterfaceTool.TextSetup("Seek Button Text Left", seekL.transform, out Text seek_txt_l, false);
        InterfaceTool.FormatRect(seek_txt_l.rectTransform);
        InterfaceTool.FormatText(seek_txt_l, DEFAULT_FONT, 60, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        seek_txt_l.text = "<";
        seek_txt_l.alignByGeometry = true;

        seekR = InterfaceTool.ButtonSetup("Seek Right", panelR.transform, out Image seek_img_r, out seekButtonR, uiSprites[3], () => SeekShop(buildingListTf, true));
        InterfaceTool.FormatRect(seek_img_r.rectTransform, new Vector2(70, BUTTON_HEIGHT),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(35, 0));
        ColorBlock seek_colors_r = seekButtonR.colors;
        seek_img_r.color = Color.white;
        seek_colors_r.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1);
        seekButtonR.colors = seek_colors_r;

        seekTxtContainerR = InterfaceTool.TextSetup("Seek Button Text Right", seekR.transform, out Text seek_txt_r, false);
        InterfaceTool.FormatRect(seek_txt_r.rectTransform);
        InterfaceTool.FormatText(seek_txt_r, DEFAULT_FONT, 60, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        seek_txt_r.text = ">";
        seek_txt_r.alignByGeometry = true;

        for (int i = 0; i < saveData.buildingsData.Count; i++)
        {
            Vector2 buttonPosition = new Vector2((i * 210) + 15, 0);

            buildingButtons.Add(GenerateShopButton(activeProfile.cl.buildingsData[i], buildingListTf, buttonPosition, tooltip));

            buildingButtons[i].button.gameObject.SetActive(false);
            buildingButtons[i].name.text  = saveData.buildingsData[i].Name;
            buildingButtons[i].price.text = $"$ {BitNotation.ToBitNotation(saveData.buildingsData[i].Price)}";
            buildingButtons[i].count.text = BitNotation.ToBitNotation(saveData.buildingsData[i].Amount);
        }

        buildingButtons[0].active = true;
        buildingButtons[0].button.gameObject.SetActive(true); // First button should be active

        UpdateBuildingsList();
        UpdateSeekButtons(buildingListTf);
    }

    void UpdateBuildingsList()
    {
        int newListLength = -Screen.width;

        buildingButtons.ForEach(button =>
        {
            if (button.active)
                newListLength += 210;
        });

        buildingListTf.sizeDelta = new Vector2(newListLength + 20, 0);
    }

    CLSCBuildingButton GenerateShopButton(BuildingData targetBuilding, RectTransform tf, Vector2 pos, CLSCTooltip tooltip)
    {
        GameObject building, name, price, count;

        CLSCBuildingButton buildingButton = new CLSCBuildingButton();

        building = InterfaceTool.ButtonSetup($"{targetBuilding.Name} Button", tf,
            out buildingButton.image, out buildingButton.button, uiSprites[3], () => PurchaseBuilding(buildingButton, targetBuilding));
        InterfaceTool.FormatRect(buildingButton.image.rectTransform, new Vector2(200, 150),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), pos);

        name = InterfaceTool.TextSetup("Name", buildingButton.button.transform, out buildingButton.name, false);
        InterfaceTool.FormatRectNPos(buildingButton.name.rectTransform, new Vector2(0, 50),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
        InterfaceTool.FormatText(buildingButton.name, DEFAULT_FONT, 20, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        buildingButton.name.text = targetBuilding.Name;

        count = InterfaceTool.TextSetup("Count", buildingButton.button.transform, out buildingButton.count, false);
        InterfaceTool.FormatRect(buildingButton.count.rectTransform, new Vector2(-30, 50),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -20));
        InterfaceTool.FormatText(buildingButton.count, DEFAULT_FONT, 16, Color.black, TextAnchor.MiddleLeft, FontStyle.Normal);
        buildingButton.count.text = "0";

        price = InterfaceTool.TextSetup("Price", name.transform, out buildingButton.price, false);
        InterfaceTool.FormatRectNPos(buildingButton.price.rectTransform, new Vector2(-30, 20),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 0));
        InterfaceTool.FormatText(buildingButton.price, DEFAULT_FONT, 16, Color.black, TextAnchor.MiddleRight, FontStyle.Normal);
        buildingButton.price.text = "$ 0";

        tooltip.AssignTooltip(building, targetBuilding.Name, DisplayFormat.BUILDING);

        return buildingButton;
    }

    public void UpdateBPS()
    {
        saveData.BitsPerSecond = 0;
        saveData.buildingsData.ForEach(building =>
        saveData.BitsPerSecond += building.BaseValue * building.Amount);
    }

    public void PurchaseBuilding(CLSCBuildingButton shopItem, BuildingData targetBuilding)
    {
        if (saveData.GetCurrencyCurrent() >= targetBuilding.Price)
        {
            saveData.CurrencyCurrent -= targetBuilding.Price;

            targetBuilding.Buy();

            // Update building text
            shopItem.price.text = "$ " + BitNotation.ToBitNotation(targetBuilding.Price);
            shopItem.count.text = BitNotation.ToBitNotation(targetBuilding.Amount);

            UpdateBPS();
        }
    }

    public void UpdateSeekButtons(RectTransform tf)
    {
        int newPos = (int)tf.anchoredPosition.x;

        seekButtonL.interactable = newPos != 310;
        seekButtonR.interactable = newPos >= -(310 + tf.sizeDelta.x);
    }

    public void SeekShop(RectTransform tf, bool forward)
    {
        tf.localPosition =
            new Vector2(tf.localPosition.x + (forward ? -210 : 210), tf.localPosition.y);
        UpdateSeekButtons(tf);
    }
}
