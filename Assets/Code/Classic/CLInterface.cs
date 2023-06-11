using UnityEngine;
using UnityEngine.UI;

public class CLInterface
{
    public CLTooltip tooltip;

    Transform parent;
    

    void Update()
    {
        tooltip.Update(Input.mousePosition);
    }

    public void Init(CLSystem sys, Transform parent)
    {
        this.parent = parent;

        tooltip = new CLTooltip();

        CreateFeverElements();
        CreateMiscElements();
    }

    void ToggleOptions(Canvas optCanvas, GameObject opt)
    {
        opt.SetActive(!opt.activeSelf);
        InterfaceTool.ToggleCanvasPriority(parent, optCanvas);
    }

    void CreateMiscElements()
    {
        Transform misc = InterfaceTool.CanvasSetup("Misc", parent,
            out Canvas miscCanvas);
        InterfaceTool.FormatRect(misc
            .GetComponent<RectTransform>());

        CreateOptions(miscCanvas);
    }

    void CreateFeverElements()
    {

    }

    void CreateUpgradeScreen()
    {

    }
    
    void CreateOptions(Canvas canvas)
    {
        Transform optionsPanel, title, close, backdrop;

        RectTransform options = new GameObject("Options")
            .AddComponent<RectTransform>();
        options.SetParent(canvas.transform, false);
        InterfaceTool.FormatRect(options);

        InterfaceTool.ButtonSetup("Options Button",
            canvas.transform, out Image optImg, out _,
            SysManager.uiSprites[5], 
            () => ToggleOptions(canvas, options.gameObject));
        InterfaceTool.FormatRect(optImg, new Vector2(80, 80),
            Vector2.up, Vector2.up, Vector2.up,
            new Vector2(15, -15));
        optImg.type = Image.Type.Simple;

        backdrop = InterfaceTool.ImgSetup("Backdrop", options,
            out Image backdropImg, false);
        InterfaceTool.FormatRect(backdropImg);
        backdropImg.color = new Color(0, 0, 0, 0.3f);

        optionsPanel = InterfaceTool.ImgSetup("Options Panel",
            options, out Image panelImg, SysManager.defaultBox,
            false);
        panelImg.rectTransform.localPosition = new Vector2(0, 100);
        panelImg.rectTransform.sizeDelta = new Vector2(400, 500);
        panelImg.color = new Color(0.6f, 0.6f, 0.6f);

        title = InterfaceTool.ImgSetup("Options Title", optionsPanel,
            out Image titleImg, SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(titleImg, new Vector2(0, 100),
            Vector2.up, Vector2.one, new Vector2(0.5f, 1));
        titleImg.color = Color.white;
        InterfaceTool.CreateBody("OPTIONS", title, 48);

        close = InterfaceTool.ButtonSetup("Close", optionsPanel,
            out Image closeImg, out _, SysManager.uiSprites[3],
            () => ToggleOptions(canvas, options.gameObject));
        InterfaceTool.FormatRect(closeImg, new Vector2(50, 50),
            Vector2.up, Vector2.up, Vector2.one,
            new Vector2(-15, 0));
        InterfaceTool.CreateBody("X", close, 28);

        Button[] opts = new Button[4];
        Text[] opt_txt = new Text[4];
        
        for (int i = 0; i < opt_txt.Length; i++)
        {
            Transform opt = InterfaceTool.ButtonSetup($"Option {i}",
                optionsPanel, out Image img, out opts[i],
                SysManager.uiSprites[3], null);
            InterfaceTool.FormatRect(img, new Vector2(300, 70),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 0.5f),
                new Vector2(0, -160 - (90 * i)));
            opt_txt[i] = InterfaceTool.CreateBody("", opt, 32);
        }
        opt_txt[0].text = "Save";
        opt_txt[1].text = "Achievements";
        opt_txt[2].text = "Fullscreen";
        opt_txt[3].text = "Quit";

        opts[0].onClick.AddListener(SysManager.fileManager.FileSave);
        opts[1].onClick.AddListener(SysManager.achieveSys
            .DisplayInterface);
        opts[2].onClick.AddListener(SysManager.ToggleFullscreen);
        opts[3].onClick.AddListener(SysManager.LoadMainMenu);

        options.gameObject.SetActive(false);
    }
}
