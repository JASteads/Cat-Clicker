using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsInterface
{
    const int BLOCK_HEIGHT = 200;

    readonly Transform transform;
    RectTransform list;
    ABlock[] aBlocks;

    public AchievementsInterface(List<AchievementInfo> db)
    {
        transform = InterfaceTool.CanvasSetup("Achievements Canvas",
            null, out Canvas canvas).transform;
        canvas.sortingOrder = 1;
        transform.gameObject.SetActive(false);

        CreateInterface(db.Count);
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
            aBlocks[i] = new ABlock(list, db[i], BLOCK_HEIGHT);
            aBlocks[i].transform.anchoredPosition =
                new Vector2(0, -BLOCK_HEIGHT * i);
        }
    }

    void CreateInterface(int aCount)
    {
        Vector2 panelSize   = new Vector2(1300, 900),
                bodySize    = new Vector2(-90, -90),
                headerSize  = new Vector2(760, 120),
                bButtonSize = new Vector2(70, 70);

        Color   cBG    = new Color(0, 0, 0, 0.3f),
                cPanel = Color.white,
                cBody  = new Color(0.29f, 0.27f, 0.3f);

        Transform panel;

        InterfaceTool.ImgSetup("Backdrop", transform,
            out Image backdropImg, true);
        InterfaceTool.FormatRect(backdropImg);
        backdropImg.color = cBG;

        panel = InterfaceTool.ImgSetup("Panel", transform,
            out Image panelImg, SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(panelImg, panelSize);
        panelImg.color = cPanel;

        Transform aBody = InterfaceTool.ImgSetup("Body",
            panel, out Image bodyImg, false);
        InterfaceTool.FormatRect(bodyImg, bodySize,
            Vector2.zero, Vector2.one, new Vector2(-15, -15));
        aBody.gameObject.AddComponent<RectMask2D>();
        bodyImg.color = cBody;

        list = new GameObject("List").AddComponent<RectTransform>();
        list.SetParent(aBody, false);
        InterfaceTool.FormatRectNPos(list, new Vector2(
            0, BLOCK_HEIGHT * aCount), Vector2.up, Vector2.one,
            new Vector2(0.5f, 1));

        Transform aHeader = InterfaceTool.ImgSetup("Header", panel,
            out Image headerImg, SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(headerImg, headerSize,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1),
            new Vector2(0.5f, 0.5f));
        InterfaceTool.CreateBody("ACHIEVEMENTS", aHeader, 48);

        Transform bButton = InterfaceTool.ButtonSetup("Back Button",
            panel, out Image backImg, out Button _,
            SysManager.defaultButton, HideInterface);
        InterfaceTool.FormatRect(backImg, bButtonSize,
            Vector2.up, Vector2.up, new Vector2(0.5f, 0.5f),
            new Vector2(10, -10));
        InterfaceTool.CreateBody("X", bButton, 36);

        InterfaceTool.ScrollbarSetup(panel, aBody.gameObject, list,
            new Vector2(30, -90), Vector2.right, Vector2.one,
            new Vector2(1, 0.5f), new Vector2(-30, -15));
    }
}