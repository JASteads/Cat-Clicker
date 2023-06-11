using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : GameMode
{
    int activeFile;
    bool newGame, fileCheck;
    readonly bool[] existingFiles = new bool[3];

    readonly Color black = new Color(0.2f, 0.2f, 0.2f),
                   menuColor = new Color(0.7f, 0.7f, 0.75f);

    readonly List<Transform> menuStack = new List<Transform>();
    Transform startMenu, selectMenu, newGameMenu,
        profileMenu, optionsMenu, recordsMenu;

    Button[] fileSelectButtons = new Button[3];
    Button contButton, backButton;
    Text fileName, gameTitle;

    public MainMenu()
    {
        CreateMenuObjects();
        CreateStartMenu();
        CreateSelectMenu();
        CreateNewGameMenu();
        CreateProfileMenu();
        CreateOptionsMenu();
        CreateRecordsMenu();

        fileCheck = CheckExistingFiles();
        GoToStart();
    }

    void LoadScreen(Transform screen)
    {
        if (menuStack.Count > 0)
            menuStack[menuStack.Count - 1]
                .gameObject.SetActive(false);
        menuStack.Add(screen);
        screen.gameObject.SetActive(true);
        TryTitleFormat();
    }

    void ExitScreen()
    {
        int topIndex = menuStack.Count - 1;

        menuStack[topIndex].gameObject.SetActive(false);
        menuStack[topIndex - 1].gameObject.SetActive(true);
        menuStack.RemoveAt(topIndex);
        TryTitleFormat();
    }

    void GoToStart()
    {
        if (menuStack.Count > 0)
        {
            menuStack[menuStack.Count - 1]
                .gameObject.SetActive(false);
            menuStack.Clear();
        }
        LoadScreen(startMenu);
    }

    /* TryTitleFormat : Move title and toggle back button depending 
     * on whether or not the start menu is the current menu. */
    void TryTitleFormat()
    {
        Vector2 minSize = new Vector2(350, 120),
                maxSize = new Vector2(620, 200);

        GameObject backObj = backButton.gameObject;
        bool prevState = backObj.activeSelf;

        backObj.SetActive(menuStack.Count > 1);

        if (backObj.activeSelf == prevState) return;
        if (backObj.activeSelf)
        {
            InterfaceTool.FormatRect(gameTitle, minSize,
                Vector2.up, Vector2.up, Vector2.up,
                new Vector2(80, -80));
            InterfaceTool.FormatText(gameTitle,
                SysManager.DEFAULT_FONT, 48, Color.white,
                TextAnchor.UpperLeft, FontStyle.Bold);
        }
        else
        {
            InterfaceTool.FormatRect(gameTitle, maxSize,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0), new Vector2(0, 50));
            InterfaceTool.FormatText(gameTitle,
                SysManager.DEFAULT_FONT, 64, Color.white,
                TextAnchor.MiddleCenter, FontStyle.Bold);
        }
    }

    void CreateMenuObjects()
    {
        Transform backdropTF = InterfaceTool.ImgSetup("Backdrop",
            transform, out Image backdrop,
            SysManager.uiSprites[5], false);
        InterfaceTool.FormatRect(backdrop, Vector2.zero,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        backdrop.type = Image.Type.Tiled;
        backdrop.color = new Color(0.3f, 0.2f, 0.4f);

        Transform title = InterfaceTool.TextSetup("Game Title",
            transform, out gameTitle, false);
        InterfaceTool.FormatRect(gameTitle,
            new Vector2(620, 200), new Vector2(0, 200));
        InterfaceTool.FormatText(gameTitle, SysManager.DEFAULT_FONT,
            64, Color.white, TextAnchor.MiddleCenter,
            FontStyle.Bold);
        gameTitle.text = "CAT CLICKER (2020)";

        Transform versionNum = InterfaceTool.TextSetup(
            "Version Num", transform, out Text verText, false);
        InterfaceTool.FormatRect(verText, new Vector2(240, 50),
            Vector2.right, Vector2.right, Vector2.right,
            new Vector2(-30, 0));
        InterfaceTool.FormatText(verText, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.MiddleRight,
            FontStyle.Italic);
        verText.text = $"ver. {Application.version}";

        Transform copyrightTF = InterfaceTool.TextSetup(
            "Copyright", transform, out Text cpyText, false);
        InterfaceTool.FormatRect(cpyText, new Vector2(240, 50),
            Vector2.zero, Vector2.zero, Vector2.zero,
            new Vector2(30, 0));
        InterfaceTool.FormatText(cpyText, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.MiddleLeft,
            FontStyle.Italic);
        cpyText.text = "District 4311";

        Transform backTF = InterfaceTool.ButtonSetup("Back Button",
            transform, out Image back_img, out backButton, SysManager.defaultButton, ExitScreen);
        InterfaceTool.FormatRect(back_img,
            new Vector2(140, 80), new Vector2(0.5f, 0),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(-520, 40));
        InterfaceTool.CreateBody("<<", backTF, 48);
        backTF.gameObject.SetActive(false);
    }

    void CreateStartMenu()
    {
        Vector2 menuSize   = new Vector2(520, 480),
                buttonSize = new Vector2(400, 90);

        startMenu = InterfaceTool.ImgSetup("Start Menu",
            transform, out Image startImg,
            SysManager.defaultBox, false);

        InterfaceTool.FormatRectNPos(startImg, menuSize,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 1));
        startImg.color = menuColor;

        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Events.UnityAction action;

            Vector2 buttonPos = new Vector2(0, -(60 + (90 * i)));

            switch (i)
            {
                case 0:  action = () =>
                {
                    newGame = true;
                    for (int x = 0; x < 3; x++)
                        fileSelectButtons[x].interactable = true;
                    LoadScreen(selectMenu);
                }; break;
                case 1:  action = () =>
                {
                    newGame = false;
                    UpdateSelectButtons();
                    LoadScreen(selectMenu);
                }; break;
                case 2:  action = () =>
                { LoadScreen(optionsMenu); }; break;
                case 3:  action = SysManager.QuitGame; break;
                default: action = null; break;
            };
            Transform sButtonTF = InterfaceTool.ButtonSetup(
                $"Start Button #{i}", startMenu,
                out Image sImg, out Button sButton,
                SysManager.defaultButton, action);
            if (i == 1) sButton.interactable = fileCheck;

            InterfaceTool.FormatRect(sImg, buttonSize,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1), buttonPos);
            Text sText = InterfaceTool.CreateBody("", sButtonTF, 36);

            string txt;
            switch (i)
            {
                case 0:  txt = "New Game";  break;
                case 1:  txt = "Continue"; 
                    contButton = sButton;   break;
                case 2:  txt = "Options";   break;
                case 3:  txt = "Quit Game"; break;
                default: txt = "";          break;
            }
            sText.text = txt;
        }
    }

    void CreateSelectMenu()
    {
        selectMenu = InterfaceTool.ImgSetup("Select Menu",
            transform, out Image load_img,
            SysManager.defaultBox, false);

        InterfaceTool.FormatRectNPos(load_img,
            new Vector2(1200, 400), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        load_img.color = menuColor;
        fileSelectButtons = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            int fileChoice = i;

            Transform m_button_obj = InterfaceTool.ButtonSetup(
                $"File #{i}", selectMenu, out Image l_img,
                out fileSelectButtons[i], SysManager.defaultButton,
                () =>
                {
                    activeFile = fileChoice;
                    fileName.text = $"FILE" +
                    $" {(char)('A' + fileChoice)}";

                    if (!newGame)
                        SysManager.fileManager.FileLoad(activeFile);
                        
                    LoadScreen(newGame ? newGameMenu : profileMenu);
                });
            InterfaceTool.FormatRect(l_img,
                new Vector2(320, 260),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(50 - (50 * i), 0));

            Transform text_obj = InterfaceTool.TextSetup("Text",
                m_button_obj, out Text l_text, false);
            InterfaceTool.FormatRectNPos(l_text,
                new Vector2(320, 50), new Vector2(0.5f, 0),
                new Vector2(0.5f, 0), new Vector2(0.5f, 1));
            InterfaceTool.FormatText(l_text, SysManager.DEFAULT_FONT,
                32, Color.white, TextAnchor.MiddleCenter,
                FontStyle.Normal);
            l_text.text = $"FILE {(char)('A' + i)}";
        }

        selectMenu.gameObject.SetActive(false);
    }

    void CreateNewGameMenu()
    {
        newGameMenu = InterfaceTool.ImgSetup("New Game Menu",
            transform, out Image new_menu_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(new_menu_img,
            new Vector2(1250, 620), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -60));
        new_menu_img.color = menuColor;

        Transform header = InterfaceTool.ImgSetup("Header",
            newGameMenu, out Image headerImg,
            SysManager.defaultButton, false);
        InterfaceTool.FormatRectNPos(headerImg,
            new Vector2(600, 120), new Vector2(0.5f, 1),
            new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        InterfaceTool.CreateBody("Choose a Difficulty!", header, 36);

        Transform newDesc = InterfaceTool.TextSetup("Desc",
            newGameMenu, out Text descText, false);
        InterfaceTool.FormatRect(descText, new Vector2(-200, 80),
            Vector2.zero, Vector2.right, new Vector2(0.5f, 0),
            new Vector2(0, 30));
        InterfaceTool.FormatText(descText, SysManager.DEFAULT_FONT,
            28, black, TextAnchor.MiddleLeft, FontStyle.Normal);

        for (int i = 0; i < 3; i++)
        {
            Vector2 buttonPos = new Vector2(40 + (380 * i), 20);

            Transform buttonTF = InterfaceTool.ButtonSetup(
                $"Diff Option #{i}", newGameMenu,
                out Image img, out Button button,
                SysManager.defaultButton,
                () =>
                {
                    SysManager.fileManager.NewFile(activeFile);

                    newGame = false;
                    UpdateSelectButtons();

                    GoToStart();
                    LoadScreen(profileMenu);
                });
            InterfaceTool.FormatRect(img, new Vector2(360, 340),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), 
                new Vector2(0, 0.5f), buttonPos);

            Transform textTF = InterfaceTool.TextSetup("Text",
                buttonTF, out Text text, false);
            InterfaceTool.FormatRect(text, new Vector2(0, 80),
                Vector2.zero, Vector2.right, new Vector2(0.5f, 0.5f),
                new Vector2(0, 40));
            InterfaceTool.FormatText(text, SysManager.DEFAULT_FONT,
                36, black, TextAnchor.MiddleCenter,
                FontStyle.Normal);

            EventTrigger trigger = buttonTF.gameObject
                .AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry()
            { eventID = EventTriggerType.PointerEnter };

            switch (i)
            {
                case 0:
                    text.text = "Basic";
                    button.interactable = false;
                    entry.callback.AddListener((data) =>
                    { descText.text = "This is easy mode."; });
                    break;
                case 1:
                    text.text = "Standard";
                    entry.callback.AddListener((data) => 
                    {
                        descText.text = "The standard Cat Clicker" +
                        " (2020) experience! This is currently the" +
                        " only way to play, so knock yourself out!";
                    }); break;
                case 2:
                    text.text = "Advanced";
                    button.interactable = false;
                    entry.callback.AddListener((data) => 
                    { descText.text = "This is hard mode."; });
                    break;
                default: text.text = ""; break;
            }
            trigger.triggers.Add(entry);
        }

        Transform barVert = InterfaceTool.ImgSetup("Bar Horizontal",
            newGameMenu, out Image barVertImg, false);
        InterfaceTool.FormatRect(barVertImg, new Vector2(-340, 3),
            new Vector2(0, 0.5f), new Vector2(1, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0, -180));
        barVertImg.color = black;

        newGameMenu.gameObject.SetActive(false);
    }

    void CreateProfileMenu()
    {
        profileMenu = InterfaceTool.ImgSetup("Profile Menu",
            transform, out Image profile_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(profile_img,
            new Vector2(1200, 700), new Vector2(0, -50));
        profile_img.color = menuColor;

        Transform profileName = InterfaceTool.ImgSetup("File Name",
            profileMenu, out Image profileName_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(profileName_img,
            new Vector2(200, 100), Vector2.one, Vector2.one,
            Vector2.one, new Vector2(50, 80));
        fileName = InterfaceTool.CreateBody(
            "File Name Text", profileName, 36);

        for (int i = 0; i < 5; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = null; break;
                case 1: action = SysManager.LoadCLMode; break;
                case 2: action = null; break;
                case 3: action = () =>
                {
                    PrintProfileStats();
                    LoadScreen(recordsMenu);
                };
                    break;
                case 4: action = () =>
                {
                    SysManager.fileManager.DeleteFile();
                    CheckExistingFiles();
                    GoToStart();
                }; break;
                default: action = null; break;
            };
            Transform pButtonObj = InterfaceTool.ButtonSetup(
                "Button", profileMenu, out Image pButtonBG,
                out Button p_button, SysManager.defaultButton,
                action);
            Transform p_text_obj = InterfaceTool.TextSetup(
                "Button Text", pButtonObj, out Text p_text, false);
            InterfaceTool.FormatRectNPos(p_text, new Vector2(0, 110),
                Vector2.zero, Vector2.right, new Vector2(0.5f, 0));
            InterfaceTool.FormatText(p_text, SysManager.DEFAULT_FONT,
                48, black, TextAnchor.MiddleCenter,
                FontStyle.Normal);

            switch (i)
            {
                case 0:
                    pButtonObj.name = "Story Mode";
                    InterfaceTool.FormatRect(pButtonBG,
                        new Vector2(840, 280), Vector2.up,
                        Vector2.up, Vector2.up,
                        new Vector2(40, -40));
                    p_text.text = "Story Mode";
                    p_button.interactable = false;
                    break;
                case 1:
                    pButtonObj.name = "Classic Mode";
                    InterfaceTool.FormatRect(pButtonBG,
                        new Vector2(660, 280), Vector2.zero,
                        Vector2.zero, Vector2.zero,
                        new Vector2(40, 40));
                    p_text.text = "Classic Mode";
                    break;
                case 2:
                    pButtonObj.name = "Extras";
                    InterfaceTool.FormatRect(pButtonBG,
                        new Vector2(240, 280), Vector2.one,
                        Vector2.one, Vector2.one,
                        new Vector2(-40, -40));
                    p_text.text = "Extras";
                    p_button.interactable = false;
                    break;
                case 3:
                    pButtonObj.name = "Records";
                    InterfaceTool.FormatRect(pButtonBG,
                        new Vector2(420, 280), Vector2.right,
                        Vector2.right, Vector2.right,
                        new Vector2(-40, 40));
                    p_text.text = "Records";
                    break;
                case 4:
                    pButtonObj.name = "Delete";
                    InterfaceTool.FormatRect(pButtonBG,
                        new Vector2(110, 100), Vector2.right,
                        Vector2.right, Vector2.right,
                        new Vector2(50, -80));
                    InterfaceTool.FormatRect(p_text);
                    pButtonBG.color = new Color(1, 0, 0);
                    p_text.fontStyle = FontStyle.Bold;
                    p_text.fontSize = 24;
                    p_text.text = "DELETE";
                    p_text.color = Color.white;
                    break;
                default:
                    break;
            }
        }

        profileMenu.gameObject.SetActive(false);
    }

    void CreateOptionsMenu()
    {
        optionsMenu = InterfaceTool.ImgSetup("Options Menu",
            transform, out Image opts_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(opts_img, new Vector2(1200, 700),
            new Vector2(0, -50));
        opts_img.color = menuColor;

        Transform opts_header = InterfaceTool.TextSetup(
            "Options Header", optionsMenu,
            out Text opts_header_text, false);
        InterfaceTool.FormatRect(opts_header_text,
            new Vector2(0, 120), Vector2.up, Vector2.one,
            new Vector2(0.5f, 1), new Vector2(0, -20));
        InterfaceTool.FormatText(opts_header_text,
            SysManager.DEFAULT_FONT, 64, Color.white,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        opts_header_text.text = "OPTIONS";

        for (int i = 0; i < 5; i++)
        {
            float pos_y = 135 - (100 * i);

            InterfaceTool.TextSetup($"Option Text #{i}", optionsMenu,
                out Text opt_text, false);
            InterfaceTool.FormatRect(opt_text, new Vector2(500, 80),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(0, 0.5f), new Vector2(175, pos_y));
            InterfaceTool.FormatText(opt_text,
                SysManager.DEFAULT_FONT, 32, Color.white,
                TextAnchor.MiddleCenter, FontStyle.Normal);

            Transform opt_bar = InterfaceTool.ImgSetup(
                $"Option #{i}", optionsMenu,
                out Image opt_bar_img, SysManager.uiSprites[3],
                false);
            InterfaceTool.FormatRect(opt_bar_img,
                new Vector2(400, 80), new Vector2(1, 0.5f),
                new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-125, pos_y));
            opt_bar_img.color = new Color(0.39f, 0.36f, 0.4f);

            if (i == 0 || i == 4)
            {
                InterfaceTool.ImgSetup("Info",
                    opt_bar.transform, out Image info_img, true);
                InterfaceTool.FormatRect(info_img,
                    new Vector2(60, 60), new Vector2(1, 0.5f),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                    new Vector2(-10, 0));

                InterfaceTool.TextSetup("Indicator", opt_bar,
                    out Text indicator_text, false);
                InterfaceTool.FormatRectNPos(indicator_text,
                    new Vector2(330, 80), new Vector2(0, 0.5f),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                InterfaceTool.FormatText(indicator_text,
                    SysManager.DEFAULT_FONT, 32, Color.white,
                    TextAnchor.MiddleCenter, FontStyle.Normal);
                indicator_text.text = (i == 0) ? "High" : "Standard"; // Filler dialogue for now
            }

            switch (i)
            {
                case 0: opt_text.text = "Quality";       break;
                case 1: opt_text.text = "Master Volume"; break;
                case 2: opt_text.text = "Music Volume";  break;
                case 3: opt_text.text = "Sound Volume";  break;
                case 4: opt_text.text = "Difficulty";    break;
            };
        }

        InterfaceTool.ImgSetup("Bar Horizontal", opts_header,
            out Image opt_bar_vert_img, false);
        InterfaceTool.FormatRectNPos(opt_bar_vert_img,
            new Vector2(1000, 3), new Vector2(0.5f, 0),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        opt_bar_vert_img.color = Color.white;

        Transform comingSoon = InterfaceTool.ImgSetup(
            "COMING SOON", optionsMenu,
            out Image comingSoonImg, true);
        InterfaceTool.FormatRect(comingSoonImg);
        comingSoonImg.color = new Color(0, 0, 0, 0.75f);
        InterfaceTool.CreateBody("Coming Soon", comingSoon, 128)
            .rectTransform.Rotate(new Vector3(0, 0, 20));

        optionsMenu.gameObject.SetActive(false);
    }

    void CreateRecordsMenu()
    {
        recordsMenu = InterfaceTool.ImgSetup("Records Menu",
            transform, out Image records_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(records_img,
            new Vector2(600, 700), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(1, 0.5f));
        records_img.color = new Color(0.7f, 0.7f, 0.75f);

        Transform achieveTF = InterfaceTool
            .ButtonSetup("Achievements Button",
            recordsMenu.transform, out Image achievements_img,
            out Button achievements_button, SysManager.defaultButton,
            SysManager.achieveSys.DisplayInterface);
        InterfaceTool.FormatRect(achievements_img,
            new Vector2(-80, 200), Vector2.up, Vector2.one,
            new Vector2(0.5f, 1), new Vector2(0, -50));
        InterfaceTool.CreateBody("ACHIEVEMENTS", achieveTF, 48);

        Transform stats_header = InterfaceTool.TextSetup(
            "Stats Header", recordsMenu,
            out Text stats_header_txt, false);
        InterfaceTool.FormatRect(stats_header_txt,
            new Vector2(540, 60), Vector2.one, Vector2.one,
            Vector2.up, new Vector2(30, 0));
        InterfaceTool.FormatText(stats_header_txt,
            SysManager.DEFAULT_FONT, 36, Color.white,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        stats_header_txt.text = "STATS";

        Transform stats = InterfaceTool.TextSetup("Stats Text",
            stats_header, out Text stats_txt, false);
        InterfaceTool.FormatRectNPos(stats_txt,
            new Vector2(0, 600), Vector2.zero,
            Vector2.right, new Vector2(0.5f, 1));
        InterfaceTool.FormatText(stats_txt, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
        stats_txt.lineSpacing = 1.5f;

        recordsMenu.gameObject.SetActive(false);
    }

    void UpdateSelectButtons()
    {
        for (int i = 0; i < fileSelectButtons.Length; i++)
            fileSelectButtons[i].interactable = existingFiles[i];
    }

    bool CheckExistingFiles()
    {
        contButton.interactable = false;

        for (int i = 0; i < 3; i++)
        {
            existingFiles[i] = System.IO.File.Exists(
                $"{System.IO.Directory.GetCurrentDirectory()}" +
                $"/Saves/File {(char)('A' + i)}.nimmy");

            if (!contButton.interactable)
                contButton.interactable = existingFiles[i];
        }

        return contButton.interactable;
    }

    string PrintProfileStats()
    {
        return SysManager.activeProfile.cl != null ?
            $"Current bits : " +
            $"{SysManager.activeProfile.cl.CurrencyCurrent:#,0.#}\n" +
            $"Total bits   : " +
            $"{SysManager.activeProfile.cl.CurrencyTotal:#,0.#}\n" +
            $"Current BPS  : " +
            $"{SysManager.activeProfile.cl.BitsPerSecond:#,0.#}\n" +
            $"Time played  : " +
            $"{"TIME PLAYED PLACEHOLDER"} seconds\n" +
            $"Time started : " +
            $"{"TIME STARTED PLACEHOLDER"}" : "";
    }
}