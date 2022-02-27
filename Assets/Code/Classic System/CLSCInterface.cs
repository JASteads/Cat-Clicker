using UnityEngine;
using UnityEngine.UI;

using static GameManagement;

public class CLSCInterface : MonoBehaviour
{
    CLSCSystem system; // This is where we grab values for the game instance

    GameObject bitsCanvas, infoCanvas;
    Text bitCounter, BPSCounter;
    Button clickButton, optButton;

    public CLSCStatusMessagesList generalMessages;
    public CLSCStatusMessagesList clickPopups;
    
    public CLSCTooltip tooltip;

    RectTransform optionsObj;
    Button[] options = new Button[4];

    // GAME UPDATES
    void Start()
    {
        system = gameObject.GetComponent<CLSCSystem>();

        gameObject.tag = "Main";

        bitsCanvas = InterfaceTool.Canvas_Setup("Bits Canvas", transform);
        infoCanvas = InterfaceTool.Canvas_Setup("Info Canvas", transform);

        tooltip = new CLSCTooltip(infoCanvas.transform);

        // Move tooltip forward on the Z axis so it displays in front of all objects in the scene
        tooltip.tooltipObj.transform.localPosition = new Vector3(
            tooltip.tooltipObj.transform.localPosition.x,
            tooltip.tooltipObj.transform.localPosition.y,
            -1);

        system.buildingSystem.CreateBuildingShop(transform, tooltip);
        system.upgradeSystem.CreateUpgradesPanel(transform, tooltip, system);
        system.feverSystem.CreateFeverSystem(transform, tooltip);
        
        Init_Bits();
        Init_Options();



        // POST SETUP

        generalMessages = gameObject.AddComponent<CLSCStatusMessagesList>();
        generalMessages.SetupList("General", 32, infoCanvas.transform,
            new Vector2(500, 50), new Vector2(0, -200));

        clickPopups = gameObject.AddComponent<CLSCStatusMessagesList>();
        clickPopups.SetupList("Click Popups", 32, 10, clickButton.transform,
            new Vector2(200, 50), new Vector2(0, 50));
    }
    
    void Update()
    {
        bitCounter.text = $"Bits : {BitNotation.ToBitNotation(profile.clscSaveData.GetCurrencyCurrent(), "#,0")}";
        BPSCounter.text = $"Bits per second : {BitNotation.ToBitNotation(profile.clscSaveData.BitsPerSecond * (system.feverSystem.isActive ? 1.5f : 1), "#,0.#")}";
        
        if (tooltip.tooltipObj.activeSelf)
        {
            tooltip.UpdateTooltip(Input.mousePosition);
        }
    }
    
    void Init_Bits()
    {
        GameObject bit_counter_obj, bps_obj, click_obj, click_text_obj, opt_obj;

        bit_counter_obj = InterfaceTool.Text_Setup("Bit Counter", bitsCanvas.transform, out bitCounter, false);
        InterfaceTool.Format_Rect(bitCounter.rectTransform, new Vector2(1300, 60),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -160));
        InterfaceTool.Format_Text(bitCounter, defaultFont, 42, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);

        bps_obj = InterfaceTool.Text_Setup("BPS Counter", bit_counter_obj.transform, out BPSCounter, false);
        InterfaceTool.Format_Rect(BPSCounter.rectTransform, new Vector2(800, 40),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -bitCounter.rectTransform.rect.height));
        InterfaceTool.Format_Text(BPSCounter, defaultFont, 32, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);

        click_obj = InterfaceTool.Button_Setup("Click Button", bitsCanvas.transform, out Image click_img, out clickButton, uiSprites[3], system.Click);
        InterfaceTool.Format_Rect(click_img.rectTransform, new Vector2(270, 90),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(180, -300));

        click_text_obj = InterfaceTool.Text_Setup("Click Text", click_obj.transform, out Text click_text, false);
        InterfaceTool.Format_Rect_NPos(click_text.rectTransform, new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));
        InterfaceTool.Format_Text(click_text, defaultFont, 42, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        click_text.text = "CLICK";

        opt_obj = InterfaceTool.Button_Setup("Options Button", bitsCanvas.transform, out Image opt_img, out optButton, uiSprites[5], Toggle_Options);
        InterfaceTool.Format_Rect(opt_img.rectTransform, new Vector2(80, 80),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(15, -15));
        opt_img.type = Image.Type.Simple;
    }

    // OPTIONS

    void Init_Options()
    {
        GameObject options_panel, title, title_text_container, close, close_text_container, backdrop;

        optionsObj = new GameObject("Options").AddComponent<RectTransform>();
        optionsObj.SetParent(infoCanvas.transform, false);
        InterfaceTool.Format_Rect(optionsObj);

        backdrop = InterfaceTool.Img_Setup("Backdrop", optionsObj.transform, out Image backdrop_img, false);
        InterfaceTool.Format_Rect(backdrop_img.rectTransform);
        backdrop_img.color = new Color(0, 0, 0, 0.3f);

        options_panel = InterfaceTool.Img_Setup("Options Panel", optionsObj.transform, out Image panel_img, defaultBox, false);
        panel_img.rectTransform.localPosition = new Vector2(0, 100);
        panel_img.rectTransform.sizeDelta = new Vector2(400, 500);
        panel_img.color = new Color(0.6f, 0.6f, 0.6f);

        title = InterfaceTool.Img_Setup("Options Title", options_panel.transform, out Image title_img, defaultBox, false);
        InterfaceTool.Format_Rect_NPos(title_img.rectTransform, new Vector2(0, 100),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
        title_img.color = Color.white;

        title_text_container = InterfaceTool.Text_Setup("Title Text", title.transform, out Text title_text, false);
        InterfaceTool.Format_Rect(title_text.rectTransform);
        InterfaceTool.Format_Text(title_text, defaultFont, 48, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        title_text.alignByGeometry = true;
        title_text.text = "OPTIONS";

        close = InterfaceTool.Button_Setup("Close", options_panel.transform, out Image close_img, out Button close_button, uiSprites[3], Toggle_Options);
        InterfaceTool.Format_Rect(close_img.rectTransform, new Vector2(50, 50),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-15, 0));

        close_text_container = InterfaceTool.Text_Setup("Close Text", close.transform, out Text close_text, false);
        InterfaceTool.Format_Rect(close_text.rectTransform);
        InterfaceTool.Format_Text(close_text, defaultFont, 28, Color.black, TextAnchor.MiddleCenter, FontStyle.Bold);
        close_text.alignByGeometry = true;
        close_text.text = "X";

        Text[] opt_txt = new Text[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            GameObject opt_obj, opt_txt_obj;

            opt_obj = InterfaceTool.Button_Setup($"Option {i}", options_panel.transform, out Image opt_img, out options[i], uiSprites[3], null);
            InterfaceTool.Format_Rect(opt_img.rectTransform, new Vector2(300, 70),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -160 - (90 * i)));

            opt_txt_obj = InterfaceTool.Text_Setup("Option Text", opt_obj.transform, out opt_txt[i], false);
            InterfaceTool.Format_Rect(opt_txt[i].rectTransform);
            InterfaceTool.Format_Text(opt_txt[i], defaultFont, 32, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
            opt_txt[i].alignByGeometry = true;
        }
        opt_txt[0].text = "Save";
        opt_txt[1].text = "Achievements";
        opt_txt[2].text = "Fullscreen";
        opt_txt[3].text = "Quit";

        options[0].onClick.AddListener(() => 
        {
            fileManager.FileSave();
            generalMessages.Broadcast("File saved!", StatusType.BONUS);
        });
        options[1].onClick.AddListener(() => achievementsInterface.Display_Achievements(true));
        options[2].onClick.AddListener(Toggle_Fullscreen);
        options[3].onClick.AddListener(QuitClassic);

        optionsObj.gameObject.SetActive(false);
    }
    public void Toggle_Options()
    {
        optionsObj.gameObject.SetActive(!optionsObj.gameObject.activeSelf);
        InterfaceTool.Toggle_Canvas_Priority(gameObject, infoCanvas.GetComponent<Canvas>());
    }
    void Toggle_Fullscreen()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(1280, 720, false);
        else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }
}
