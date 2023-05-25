using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsInterface
{
    const int BLOCK_HEIGHT = 185;

    readonly Transform transform;
    RectTransform aList;
    ABlock[] aBlocks;

    public AchievementsInterface(List<AchievementInfo> db)
    {
        transform = InterfaceTool.CanvasSetup("Achievements Canvas",
            null, out _).transform;
        transform.gameObject.SetActive(false);

        CreateInterface(db);
        CreateBlocks(db);
    }

    public void Display()
    {
        for (int i = 0; i < aBlocks.Length; i++)
            aBlocks[i].UpdateBlock();
        transform.gameObject.SetActive(true);
    }

    void HideInterface()
    {
        transform.gameObject.SetActive(false);
    }

    void CreateBlocks(List<AchievementInfo> db)
    {
        aBlocks = new ABlock[db.Count];

        for (int i = 0; i < db.Count; i++)
        {
            aBlocks[i] = new ABlock(aList, db[i], BLOCK_HEIGHT);
            aBlocks[i].transform.anchoredPosition =
                new Vector2(0, -BLOCK_HEIGHT * i);
        }
    }

    void CreateInterface(List<AchievementInfo> db)
    {
        Vector2 panelSize   = new Vector2(1300, 900),
                bodySize    = new Vector2(-90, -90),
                headerSize  = new Vector2(760, 120),
                bButtonSize = new Vector2(70, 70);

        Color   cBG    = new Color(0, 0, 0, 0.3f),
                cPanel = Color.white,
                cBody  = new Color(0.29f, 0.27f, 0.3f);


        GameObject backdrop = InterfaceTool.ImgSetup("Backdrop",
            transform, out Image backdropImg, true);
        InterfaceTool.FormatRect(backdropImg.rectTransform);
        backdropImg.color = cBG;

        GameObject aPanel = InterfaceTool.ImgSetup("Panel",
            transform, out Image panelImg,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(panelImg.rectTransform,
            panelSize);
        panelImg.color = cPanel;

        GameObject aBody = InterfaceTool.ImgSetup("Body",
            aPanel.transform, out Image bodyImg, false);
        InterfaceTool.FormatRect(bodyImg.rectTransform,
            bodySize, Vector2.zero, Vector2.one,
            new Vector2(-15, -15));
        aBody.AddComponent<RectMask2D>();
        bodyImg.color = cBody;

        aList = new GameObject("List").AddComponent<RectTransform>();
        aList.transform.SetParent(aBody.transform, false);
        InterfaceTool.FormatRectNPos(aList, new Vector2(
            0, BLOCK_HEIGHT * db.Count),
            Vector2.up, Vector2.one, new Vector2(0.5f, 1));

        GameObject aHeader = InterfaceTool.ImgSetup("Header",
            aPanel.transform, out Image headerImg,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(headerImg.rectTransform,
            headerSize, new Vector2(0.5f, 1),
            new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject aHeaderTxtObj = InterfaceTool.TextSetup("Title",
            aHeader.transform, out Text aHeaderTxt, false);
        InterfaceTool.FormatRect(aHeaderTxt.rectTransform);
        InterfaceTool.FormatText(aHeaderTxt, SysManager.DEFAULT_FONT,
            48, Color.white, TextAnchor.MiddleCenter,
            FontStyle.Normal);
        aHeaderTxt.text = "ACHIEVEMENTS";

        GameObject bButtonObj = InterfaceTool.ButtonSetup(
            "Back Button", aPanel.transform, out Image backImg,
            out Button backButton, SysManager.defaultButton,
            HideInterface);
        InterfaceTool.FormatRect(backImg.rectTransform,
            bButtonSize, Vector2.up, Vector2.up,
            new Vector2(0.5f, 0.5f), new Vector2(10, -10));
        Text bButtonTxt = InterfaceTool.CreateBody(
            "X", bButtonObj.transform, 36);

        GameObject scrollObj = InterfaceTool.ScrollbarSetup(
            aPanel.transform, aBody, aList, new Vector2(30, -90),
            Vector2.right, Vector2.one, new Vector2(1, 0.5f),
            new Vector2(-30, -15));
    }
}