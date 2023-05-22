using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu
{
    int activeFile;
    bool newGame, fileCheck;
    readonly bool[] existingFiles = new bool[3];

    readonly Color black = new Color(0.2f, 0.2f, 0.2f),
                   menuColor = new Color(0.7f, 0.7f, 0.75f);
    readonly Transform transform;

    readonly List<GameObject> menuStack = new List<GameObject>();
    GameObject startMenu, selectMenu, newGameMenu,
        profileMenu, optionsMenu, recordsMenu;

    Button[] fileSelectButtons = new Button[3];
    Button contButton, backButton;
    Text fileName, gameTitle;

    public MainMenu(Transform parent)
    {
        transform = InterfaceTool.CanvasSetup("Main Menu",
            parent.transform, out _).transform;
        
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

    void LoadScreen(GameObject screen)
    {
        Debug.Log($"Loading screen : {screen.name}");
        if (menuStack.Count > 0)
            menuStack[menuStack.Count - 1].SetActive(false);
        menuStack.Add(screen);
        screen.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    void ExitScreen()
    {
        int topIndex = menuStack.Count - 1;

        menuStack[topIndex].SetActive(false);
        menuStack[topIndex - 1].SetActive(true);
        menuStack.RemoveAt(topIndex);
        backButton.gameObject.SetActive(topIndex > 1);
    }

    /* GoToStart : Clears the menu stack and loads the start menu */
    void GoToStart()
    {
        if (menuStack.Count > 0)
        {
            menuStack[menuStack.Count - 1].SetActive(false);
            menuStack.Clear();
        }
        LoadScreen(startMenu);
        AttemptTitleFormat();
    }

    /* AttemptTitleFormat : Move title and toggle back button depending on 
     * whether or not the start menu is the current menu.
     */
    void AttemptTitleFormat()
    {
        GameObject topMenu = menuStack[menuStack.Count - 1],
            backObj = backButton.gameObject;
        if (topMenu != startMenu)
        {
            backObj.SetActive(!backObj.activeSelf);

            InterfaceTool.FormatRect(gameTitle.rectTransform,
                new Vector2(350, 120), Vector2.up, Vector2.up, Vector2.up,
                new Vector2(80, -80));
            InterfaceTool.FormatText(gameTitle,
                SysManager.DEFAULT_FONT, 48, Color.white,
                TextAnchor.UpperLeft, FontStyle.Bold);
        }
        else
        {
            backObj.SetActive(false);
            InterfaceTool.FormatRect(gameTitle.rectTransform,
                new Vector2(620, 200), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0),
                new Vector2(0, 50));
            InterfaceTool.FormatText(gameTitle,
                SysManager.DEFAULT_FONT, 64, Color.white,
                TextAnchor.MiddleCenter, FontStyle.Bold);
        }
    }

    /* CheckExistingFiles : Checks the Saves folder for existing files */
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

    void UpdateSelectButtons()
    {
        for (int i = 0; i < fileSelectButtons.Length; i++)
            fileSelectButtons[i].interactable = existingFiles[i];
    }

    string UpdateStats()
    {
        return SysManager.profile.clscSaveData != null ? 
            $"Current bits : " +
            $"{SysManager.profile.clscSaveData.CurrencyCurrent:#,0.#}\n" +
            $"Total bits   : " +
            $"{SysManager.profile.clscSaveData.CurrencyTotal:#,0.#}\n" +
            $"Current BPS  : " +
            $"{SysManager.profile.clscSaveData.BitsPerSecond:#,0.#}\n" +
            $"Time played  : " +
            $"{ "TIME PLAYED PLACEHOLDER" } seconds\n" +
            $"Time started : " +
            $"{ "TIME STARTED PLACEHOLDER" }" : "";
    }

    void CreateMenuObjects()
    {
        GameObject backdrop_obj = InterfaceTool.ImgSetup("Backdrop",
            transform, out Image backdrop,
            SysManager.uiSprites[5], false);
        InterfaceTool.FormatRect(backdrop.rectTransform,
            Vector2.zero, Vector2.zero,
            Vector2.one, new Vector2(0.5f, 0.5f));
        backdrop.type = Image.Type.Tiled;
        backdrop.color = new Color(0.3f, 0.2f, 0.4f);

        GameObject title_obj = InterfaceTool.TextSetup("Game Title",
            transform, out gameTitle, false);
        InterfaceTool.FormatRect(gameTitle.rectTransform,
            new Vector2(620, 200), new Vector2(0, 200));
        InterfaceTool.FormatText(gameTitle, SysManager.DEFAULT_FONT,
            64, Color.white, TextAnchor.MiddleCenter,
            FontStyle.Bold);
        gameTitle.text = "CAT CLICKER (2020)";

        GameObject version_num = InterfaceTool.TextSetup(
            "Version Num", transform, out Text ver_text, false);
        InterfaceTool.FormatRect(ver_text.rectTransform,
            new Vector2(240, 50), Vector2.right,
            Vector2.right, Vector2.right,
            new Vector2(-30, 0));
        InterfaceTool.FormatText(ver_text, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.MiddleRight,
            FontStyle.Italic);
        ver_text.text = $"ver. { Application.version }";

        GameObject copyright_obj = InterfaceTool.TextSetup(
            "Copyright", transform, out Text cpy_text, false);
        InterfaceTool.FormatRect(cpy_text.rectTransform,
            new Vector2(240, 50), Vector2.zero,
            Vector2.zero, Vector2.zero,
            new Vector2(30, 0));
        InterfaceTool.FormatText(cpy_text, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.MiddleLeft,
            FontStyle.Italic);
        cpy_text.text = "District 4311";

        GameObject backObj = InterfaceTool.ButtonSetup("Back Button",
            transform, out Image back_img,
            out backButton, SysManager.defaultButton, ExitScreen);
        InterfaceTool.FormatRect(back_img.rectTransform,
            new Vector2(140, 80), new Vector2(0.5f, 0),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(-520, 40));
        GameObject back_text_obj = InterfaceTool.TextSetup(
            "Back Text", backObj.transform,
            out Text back_text, false);
        InterfaceTool.FormatRect(back_text.rectTransform);
        InterfaceTool.FormatText(back_text, SysManager.DEFAULT_FONT,
            48, black, TextAnchor.MiddleCenter, FontStyle.Bold);
        back_text.alignByGeometry = true;
        back_text.text = "<<";
        backObj.SetActive(false);
    }

    void CreateStartMenu()
    {
        startMenu = InterfaceTool.ImgSetup("Start Menu",
            transform, out Image start_img,
            SysManager.defaultBox, false);

        InterfaceTool.FormatRectNPos(start_img.rectTransform,
            new Vector2(520, 480), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        start_img.color = menuColor;

        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Events.UnityAction action;

            switch(i)
            {
                case 0: action = () =>
                {
                    newGame = true;
                    for (int x = 0; x < 3; x++)
                        fileSelectButtons[x].interactable = true;
                    LoadScreen(selectMenu);
                }; break;
                case 1: action = () =>
                {
                    newGame = false;
                    UpdateSelectButtons();
                    LoadScreen(selectMenu);
                }; break;
                case 2: action = () =>
                { LoadScreen(optionsMenu); }; break;
                case 3: action = SysManager.QuitGame; break;
                default: action = null; break;
            };
            GameObject s_button_obj = InterfaceTool.ButtonSetup(
                $"Start Button #{i}", startMenu.transform,
                out Image s_img, out Button s_button,
                SysManager.defaultButton, action);

            InterfaceTool.FormatRect(s_img.rectTransform,
                new Vector2(400, 90), new Vector2(0.5f, 1),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1),
                new Vector2(0, -(60 + (90 * i))));

            GameObject text_obj = InterfaceTool.TextSetup("Text",
                s_button_obj.transform, out Text s_text, false);
            InterfaceTool.FormatRect(s_text.rectTransform);
            InterfaceTool.FormatText(s_text, SysManager.DEFAULT_FONT,
                36, black, TextAnchor.MiddleCenter, FontStyle.Normal);
            switch (i)
            {
                case 0: s_text.text = "New Game"; break;
                case 1: s_text.text = "Continue"; contButton = s_button; break;
                case 2: s_text.text = "Options"; break;
                case 3: s_text.text = "Quit Game"; break;
                default: s_text.text = ""; break;
            }

            if (i == 1 && !fileCheck)
                s_button.interactable = false;
        }
    }

    void CreateSelectMenu()
    {
        selectMenu = InterfaceTool.ImgSetup("Select Menu",
            transform, out Image load_img,
            SysManager.defaultBox, false);

        InterfaceTool.FormatRectNPos(load_img.rectTransform,
            new Vector2(1200, 400), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        load_img.color = menuColor;
        fileSelectButtons = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            int fileChoice = i;

            GameObject m_button_obj = InterfaceTool.ButtonSetup(
                $"File #{i}", selectMenu.transform, out Image l_img,
                out fileSelectButtons[i], SysManager.defaultButton,
                () =>
                {
                    activeFile = fileChoice;
                    fileName.text = $"FILE" +
                    $" {(char)('A' + fileChoice)}";

                    if (!newGame)
                    {
                        SysManager.fileManager.FileLoad(fileChoice);
                        SysManager.LoadAchievementsInterface();
                    }
                    LoadScreen(newGame ? newGameMenu : profileMenu);
                });
            InterfaceTool.FormatRect(l_img.rectTransform,
                new Vector2(320, 260),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(0 + (0.5f * i), 0.5f),
                new Vector2(50 - (50 * i), 0));

            GameObject text_obj = InterfaceTool.TextSetup("Text",
                m_button_obj.transform, out Text l_text, false);
            InterfaceTool.FormatRectNPos(l_text.rectTransform,
                new Vector2(320, 50), new Vector2(0.5f, 0),
                new Vector2(0.5f, 0), new Vector2(0.5f, 1));
            InterfaceTool.FormatText(l_text, SysManager.DEFAULT_FONT,
                32, Color.white, TextAnchor.MiddleCenter,
                FontStyle.Normal);
            l_text.text = $"FILE {(char)('A' + i)}";
        }

        selectMenu.SetActive(false);
    }

    void CreateNewGameMenu()
    {
        newGameMenu = InterfaceTool.ImgSetup("New Game Menu",
            transform, out Image new_menu_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(new_menu_img.rectTransform,
            new Vector2(1250, 620), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -60));
        new_menu_img.color = menuColor;

        GameObject header_obj = InterfaceTool.ImgSetup("Header",
            newGameMenu.transform, out Image header_img,
            SysManager.defaultButton, false);
        InterfaceTool.FormatRectNPos(header_img.rectTransform,
            new Vector2(600, 120), new Vector2(0.5f, 1),
            new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject header_text_obj = InterfaceTool.TextSetup(
            "Header Text", header_obj.transform,
            out Text header_text, false);
        InterfaceTool.FormatRect(header_text.rectTransform);
        InterfaceTool.FormatText(header_text,
            SysManager.DEFAULT_FONT, 36, black,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        header_text.text = "Choose a Difficulty!";

        GameObject new_desc = InterfaceTool.TextSetup("Desc",
            newGameMenu.transform, out Text desc_text, false);
        InterfaceTool.FormatRect(desc_text.rectTransform,
            new Vector2(-200, 80), Vector2.zero,
            Vector2.right, new Vector2(0.5f, 0),
            new Vector2(0, 30));
        InterfaceTool.FormatText(desc_text, SysManager.DEFAULT_FONT,
            28, black, TextAnchor.MiddleLeft, FontStyle.Normal);

        for (int i = 0; i < 3; i++)
        {
            GameObject n_button_obj = InterfaceTool.ButtonSetup(
                $"Diff Option #{i}", newGameMenu.transform,
                out Image n_img, out Button n_button,
                SysManager.defaultButton,
                () =>
                {
                    SysManager.fileManager.NewFile(
                        activeFile, (Difficulty)i);
                    SysManager.LoadAchievementsInterface();

                    fileCheck = CheckExistingFiles();
                    newGame = false;
                    UpdateSelectButtons();

                    GoToStart();
                    LoadScreen(profileMenu);
                });
            InterfaceTool.FormatRect(n_img.rectTransform,
                new Vector2(360, 340), new Vector2(0, 0.5f),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(40 + (380 * i), 20));

            GameObject text_obj = InterfaceTool.TextSetup("Text",
                n_button_obj.transform, out Text n_text, false);
            InterfaceTool.FormatRect(n_text.rectTransform,
                new Vector2(0, 80), Vector2.zero,
                Vector2.right, new Vector2(0.5f, 0.5f),
                new Vector2(0, 40));
            InterfaceTool.FormatText(n_text, SysManager.DEFAULT_FONT,
                36, black, TextAnchor.MiddleCenter,
                FontStyle.Normal);

            EventTrigger trigger = n_button_obj
                .AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry()
            { eventID = EventTriggerType.PointerEnter };

            switch (i)
            {
                case 0:
                    n_text.text = "Basic";
                    n_button.interactable = false;
                    entry.callback.AddListener((data) =>
                    { desc_text.text = "This is easy mode."; });
                    break;
                case 1:
                    n_text.text = "Standard";
                    entry.callback.AddListener((data) => 
                    {
                        desc_text.text = "The standard Cat Clicker" +
                        " (2020) experience! This is currently the" +
                        " only way to play, so knock yourself out!";
                    }); break;
                case 2:
                    n_text.text = "Advanced";
                    n_button.interactable = false;
                    entry.callback.AddListener((data) => 
                    { desc_text.text = "This is hard mode."; });
                    break;
                default: n_text.text = ""; break;
            }
            trigger.triggers.Add(entry);
        }

        GameObject barVert = InterfaceTool.ImgSetup("Bar Horizontal",
            newGameMenu.transform, out Image barVertImg, false);
        InterfaceTool.FormatRect(barVertImg.rectTransform,
            new Vector2(-340, 3), new Vector2(0, 0.5f),
            new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0, -180));
        barVertImg.color = black;

        newGameMenu.SetActive(false);
    }

    void CreateProfileMenu()
    {
        profileMenu = InterfaceTool.ImgSetup("Profile Menu",
            transform, out Image profile_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(profile_img.rectTransform,
            new Vector2(1200, 700), new Vector2(0, -50));
        profile_img.color = menuColor;

        Transform profileName = InterfaceTool.ImgSetup("File Name",
            profileMenu.transform, out Image profileName_img,
            SysManager.defaultBox, false).transform;
        InterfaceTool.FormatRect(profileName_img.rectTransform,
            new Vector2(200, 100), Vector2.one, Vector2.one,
            Vector2.one, new Vector2(50, 80));

        GameObject profileName_text_obj = InterfaceTool.TextSetup(
            "File Name Text", profileName, out fileName, false);
        InterfaceTool.FormatRect(fileName.rectTransform);
        InterfaceTool.FormatText(fileName, SysManager.DEFAULT_FONT,
            36, Color.white, TextAnchor.MiddleCenter,
            FontStyle.Bold);

        for (int i = 0; i < 5; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = null; break;
                case 1: action = () =>
                {
                    SysManager.LoadClassicMode();
                    EndMainMenu();
                }; break;
                case 2: action = null; break;
                case 3: action = () => LoadScreen(recordsMenu);
                    break;
                case 4: action = () =>
                {
                    SysManager.fileManager.DeleteFile();
                    SysManager.QuitAchievementsInterface();
                    CheckExistingFiles();
                    GoToStart();
                }; break;
                default: action = null; break;
            };
            GameObject pButtonObj = InterfaceTool.ButtonSetup(
                "Button", profileMenu.transform,
                out Image pButtonBG, out Button p_button,
                SysManager.defaultButton, action);
            GameObject p_text_obj = InterfaceTool.TextSetup(
                "Button Text", pButtonObj.transform,
                out Text p_text, false);
            InterfaceTool.FormatRectNPos(p_text.rectTransform,
                new Vector2(0, 110), Vector2.zero,
                Vector2.right, new Vector2(0.5f, 0));
            InterfaceTool.FormatText(p_text, SysManager.DEFAULT_FONT,
                48, black, TextAnchor.MiddleCenter,
                FontStyle.Normal);

            switch (i)
            {
                case 0:
                    pButtonObj.name = "Story Mode";
                    InterfaceTool.FormatRect(pButtonBG.rectTransform,
                        new Vector2(840, 280), Vector2.up,
                        Vector2.up, Vector2.up,
                        new Vector2(40, -40));
                    p_text.text = "Story Mode";
                    p_button.interactable = false;
                    break;
                case 1:
                    pButtonObj.name = "Classic Mode";
                    InterfaceTool.FormatRect(pButtonBG.rectTransform,
                        new Vector2(660, 280), Vector2.zero,
                        Vector2.zero, Vector2.zero,
                        new Vector2(40, 40));
                    p_text.text = "Classic Mode";
                    break;
                case 2:
                    pButtonObj.name = "Extras";
                    InterfaceTool.FormatRect(pButtonBG.rectTransform,
                        new Vector2(240, 280), Vector2.one,
                        Vector2.one, Vector2.one,
                        new Vector2(-40, -40));
                    p_text.text = "Extras";
                    p_button.interactable = false;
                    break;
                case 3:
                    pButtonObj.name = "Records";
                    InterfaceTool.FormatRect(pButtonBG.rectTransform,
                        new Vector2(420, 280), Vector2.right,
                        Vector2.right, Vector2.right,
                        new Vector2(-40, 40));
                    p_text.text = "Records";
                    break;
                case 4:
                    pButtonObj.name = "Delete";
                    InterfaceTool.FormatRect(pButtonBG.rectTransform,
                        new Vector2(110, 100), Vector2.right,
                        Vector2.right, Vector2.right,
                        new Vector2(50, -80));
                    InterfaceTool.FormatRect(p_text.rectTransform);
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

        profileMenu.SetActive(false);
    }

    void CreateOptionsMenu()
    {
        optionsMenu = InterfaceTool.ImgSetup("Options Menu",
            transform, out Image opts_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRect(opts_img.rectTransform,
            new Vector2(1200, 700), new Vector2(0, -50));
        opts_img.color = menuColor;

        GameObject opts_header = InterfaceTool.TextSetup(
            "Options Header", optionsMenu.transform,
            out Text opts_header_text, false);
        InterfaceTool.FormatRect(opts_header_text.rectTransform,
            new Vector2(0, 120), Vector2.up, Vector2.one,
            new Vector2(0.5f, 1), new Vector2(0, -20));
        InterfaceTool.FormatText(opts_header_text,
            SysManager.DEFAULT_FONT, 64, Color.white,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        opts_header_text.text = "OPTIONS";

        for (int i = 0; i < 5; i++)
        {
            float pos_y = 135 - (100 * i);

            InterfaceTool.TextSetup(
                $"Option Text #{i}", optionsMenu.transform,
                out Text opt_text, false);
            InterfaceTool.FormatRect(opt_text.rectTransform,
                new Vector2(500, 80), new Vector2(0, 0.5f),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(175, pos_y));
            InterfaceTool.FormatText(opt_text,
                SysManager.DEFAULT_FONT, 32, Color.white,
                TextAnchor.MiddleCenter, FontStyle.Normal);

            GameObject opt_bar = InterfaceTool.ImgSetup(
                $"Option #{i}", optionsMenu.transform,
                out Image opt_bar_img, SysManager.uiSprites[3],
                false);
            InterfaceTool.FormatRect(opt_bar_img.rectTransform,
                new Vector2(400, 80), new Vector2(1, 0.5f),
                new Vector2(1, 0.5f), new Vector2(1, 0.5f),
                new Vector2(-125, pos_y));
            opt_bar_img.color = new Color(0.39f, 0.36f, 0.4f);

            if (i == 0 || i == 4)
            {
                InterfaceTool.ImgSetup("Info",
                    opt_bar.transform, out Image info_img, true);
                InterfaceTool.FormatRect(info_img.rectTransform,
                    new Vector2(60, 60), new Vector2(1, 0.5f),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f),

                    new Vector2(-10, 0));

                InterfaceTool.TextSetup(
                    "Indicator", opt_bar.transform,
                    out Text indicator_text, false);
                InterfaceTool.FormatRectNPos(
                    indicator_text.rectTransform,
                    new Vector2(330, 80), new Vector2(0, 0.5f),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                InterfaceTool.FormatText(indicator_text,
                    SysManager.DEFAULT_FONT, 32, Color.white,
                    TextAnchor.MiddleCenter, FontStyle.Normal);
                indicator_text.text = (i == 0) ? "High" : "Standard"; // Filler dialogue for now
            }

            switch (i)
            {
                case 0: opt_text.text = "Quality"; break;
                case 1: opt_text.text = "Master Volume"; break;
                case 2: opt_text.text = "Music Volume"; break;
                case 3: opt_text.text = "Sound Volume"; break;
                case 4: opt_text.text = "Difficulty"; break;
            };
        }

        InterfaceTool.ImgSetup(
            "Bar Horizontal", opts_header.transform,
            out Image opt_bar_vert_img, false);
        InterfaceTool.FormatRectNPos(opt_bar_vert_img.rectTransform,
            new Vector2(1000, 3), new Vector2(0.5f, 0),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        opt_bar_vert_img.color = Color.white;

        GameObject comingSoonObj = InterfaceTool.ImgSetup(
            "COMING SOON", optionsMenu.transform,
            out Image comingSoonImg, true);
        InterfaceTool.FormatRect(comingSoonImg.rectTransform);
        comingSoonImg.color = new Color(0, 0, 0, 0.75f);

        InterfaceTool.TextSetup("COMING SOON Text",
            comingSoonObj.transform, out Text comingSoonTxt, false);
        InterfaceTool.FormatRect(comingSoonTxt.rectTransform);
        InterfaceTool.FormatText(comingSoonTxt,
            SysManager.DEFAULT_FONT, 128, Color.white,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        comingSoonTxt.rectTransform.Rotate(new Vector3(0, 0, 20));
        comingSoonTxt.text = "Coming Soon";

        optionsMenu.SetActive(false);
    }

    void CreateRecordsMenu()
    {
        recordsMenu = InterfaceTool.ImgSetup("Records Menu",
            transform, out Image records_img,
            SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(records_img.rectTransform,
            new Vector2(600, 700), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(1, 0.5f));
        records_img.color = new Color(0.7f, 0.7f, 0.75f);

        GameObject achievements_button_obj = InterfaceTool
            .ButtonSetup("Achievements Button",
            recordsMenu.transform, out Image achievements_img,
            out Button achievements_button, SysManager.defaultButton,
            () => SysManager.achievementsInterface
                .Display_Achievements(true));
        InterfaceTool.FormatRect(achievements_img.rectTransform,
            new Vector2(-80, 200), Vector2.right, Vector2.up,
            new Vector2(0.5f, 1), new Vector2(0, -50));

        GameObject achievement_button_txt = InterfaceTool.TextSetup(
            "A Button Text", achievements_button_obj.transform,
            out Text a_button_txt, false);
        InterfaceTool.FormatRect(a_button_txt.rectTransform);
        InterfaceTool.FormatText(a_button_txt,
            SysManager.DEFAULT_FONT, 48, Color.black,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        a_button_txt.text = "ACHIEVEMENTS";

        GameObject stats_header = InterfaceTool.TextSetup(
            "Stats Header", recordsMenu.transform,
            out Text stats_header_txt, false);
        InterfaceTool.FormatRect(stats_header_txt.rectTransform,
            new Vector2(540, 60), Vector2.one, Vector2.one,
            Vector2.up, new Vector2(30, 0));
        InterfaceTool.FormatText(stats_header_txt,
            SysManager.DEFAULT_FONT, 36, Color.white,
            TextAnchor.MiddleCenter, FontStyle.Normal);
        stats_header_txt.text = "STATS";

        GameObject stats = InterfaceTool.TextSetup("Stats Text",
            stats_header.transform, out Text stats_txt, false);
        InterfaceTool.FormatRectNPos(stats_txt.rectTransform,
            new Vector2(0, 600), Vector2.zero,
            Vector2.right, new Vector2(0.5f, 1));
        InterfaceTool.FormatText(stats_txt, SysManager.DEFAULT_FONT,
            28, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
        stats_txt.lineSpacing = 1.5f;
        stats_txt.text = UpdateStats();

        recordsMenu.SetActive(false);
    }

    void EndMainMenu()
    {
        Object.Destroy(transform.gameObject);
    }
}