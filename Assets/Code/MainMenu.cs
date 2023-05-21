using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public int activeFile;
    public bool newGame, fileCheck;
    bool[] existingFiles;

    Text fileName;
    Text gameTitle;

    Color black = new Color(0.2f, 0.2f, 0.2f);
    Color menuColor = new Color(0.7f, 0.7f, 0.75f);
    
    Button[] fileSelectButtons = new Button[3];
    Button contButton;
    Button backButton;

    List<GameObject> menus = new List<GameObject>();
    GameObject startScreen;
    GameObject currentMenu;
    GameObject backObj;

    // Allows for flexible menu navigation
    List<string> menuStack = new List<string>();


    /* Advance Menu : Move to the next menu */
    void AdvanceMenu(string nextMenuName)
    {
        currentMenu.SetActive(false);
        menuStack.Add(menus.Find(menu => menu.name == nextMenuName).name);

        // Set the current menu to the menu with the appropriate name
        currentMenu = menus.Find(menu => menu.name == menuStack[menuStack.Count - 1]);
        currentMenu.SetActive(true);

        // Change where the title is displayed and remove the back button if on title screen
        AttemptTitleFormat();
    }

    /* ExitMenu : Return to the previous menu */
    void ExitMenu()
    {
        currentMenu.SetActive(false);
        currentMenu = menus.Find(menu => menu.name == menuStack[menuStack.Count - 2]);
        menuStack.RemoveAt(menuStack.Count - 1);
        currentMenu.SetActive(true);

        AttemptTitleFormat();
    }

    /* RemoveFromStack : Removes a specific menu from the menu stack */
    void RemoveFromStack(string menuName)
    {
        menuStack.Remove(menuName);
    }

    /* GoToStart : Clears the menu stack and loads the start menu */
    void GoToStart()
    {
        if (menuStack.Count != 0)
        {
            currentMenu.SetActive(false);
            menuStack.Clear();
        }
        
        currentMenu = startScreen;
        currentMenu.SetActive(true);
        menuStack.Add(currentMenu.name);

        AttemptTitleFormat();
    }

    /* AttemptTitleFormat : Move title and toggle back button depending on 
     * whether or not the start menu is the current menu.
     */
    void AttemptTitleFormat()
    {
        
        if (currentMenu.name != "Start Menu")
        {
            if (!backObj.activeSelf) backObj.SetActive(true);
            InterfaceTool.Format_Rect(gameTitle.rectTransform, new Vector2(350, 120),
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -80));
            InterfaceTool.Format_Text(gameTitle, SysManager.defaultFont, 48, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
        }
        else
        {
            backObj.SetActive(false);
            InterfaceTool.Format_Rect(gameTitle.rectTransform, new Vector2(620, 200),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0), new Vector2(0, 50));
            InterfaceTool.Format_Text(gameTitle, SysManager.defaultFont, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        }
    }

    /* CheckExistingFiles : Checks the Saves folder for existing files */
    bool CheckExistingFiles(out bool[] existingFiles)
    {
        existingFiles = new bool[3];
        for (int i = 0; i < 3; i++)
        {
            existingFiles[i] = System.IO.File.Exists(
                $"{ System.IO.Directory.GetCurrentDirectory() + $"/Saves/" }File { (char)('A' + i) }.nimmy") ?
                true : false;
        }

        for (int i = 0; i < 3; i++)
            if (existingFiles[i])
            {
                if (contButton != null && !contButton.interactable) contButton.interactable = true;
                return true;
            }
        if (contButton != null && contButton.interactable) contButton.interactable = false;
        return false;
    }

    void UpdateSelectButtons(ref Button[] select_buttons)
    {
        for (int i = 0; i < select_buttons.Length; i++)
            select_buttons[i].interactable = existingFiles[i] ? true : false;
    }

    void Awake()
    {
        gameObject.tag = "Main";

        fileCheck = CheckExistingFiles(out existingFiles);
        GameObject menu_canvas = InterfaceTool.Canvas_Setup("Menu Canvas", gameObject.transform);


        // MENU-INDPENDENT OBJECTS

        GameObject backdrop_obj = InterfaceTool.Img_Setup("Backdrop", menu_canvas.transform, out Image backdrop, SysManager.uiSprites[5], false);
        backdrop.type = Image.Type.Tiled;
        InterfaceTool.Format_Rect(backdrop.rectTransform, new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));
        backdrop.color = new Color(0.3f, 0.2f, 0.4f);

        GameObject title_obj = InterfaceTool.Text_Setup("Game Title", menu_canvas.transform, out gameTitle, false);
        InterfaceTool.Format_Rect(gameTitle.rectTransform, new Vector2(620, 200), new Vector2(0, 200));
        InterfaceTool.Format_Text(gameTitle, SysManager.defaultFont, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        gameTitle.text = "CAT CLICKER (2020)";

        GameObject version_num = InterfaceTool.Text_Setup("Version Num", menu_canvas.transform, out Text ver_text, false);
        InterfaceTool.Format_Rect(ver_text.rectTransform, new Vector2(240, 50),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 0));
        InterfaceTool.Format_Text(ver_text, SysManager.defaultFont, 28, Color.white, TextAnchor.MiddleRight, FontStyle.Italic);
        ver_text.text = $"ver. { Application.version }";

        GameObject copyright_obj = InterfaceTool.Text_Setup("Copyright", menu_canvas.transform, out Text cpy_text, false);
        InterfaceTool.Format_Rect(cpy_text.rectTransform, new Vector2(240, 50),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 0));
        InterfaceTool.Format_Text(cpy_text, SysManager.defaultFont, 28, Color.white, TextAnchor.MiddleLeft, FontStyle.Italic);
        cpy_text.text = "District 4311";

        backObj = InterfaceTool.Button_Setup("Back Button", menu_canvas.transform, out Image back_img, out backButton, SysManager.defaultButton, ExitMenu);
        InterfaceTool.Format_Rect(back_img.rectTransform, new Vector2(140, 80),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-520, 40));
        GameObject back_text_obj = InterfaceTool.Text_Setup("Back Text", backObj.transform, out Text back_text, false);
        InterfaceTool.Format_Rect(back_text.rectTransform);
        InterfaceTool.Format_Text(back_text, SysManager.defaultFont, 48, black, TextAnchor.MiddleCenter, FontStyle.Bold);
        back_text.alignByGeometry = true;
        back_text.text = "<<";


        // START MENU

        startScreen = InterfaceTool.Img_Setup("Start Menu", menu_canvas.transform, out Image start_img, SysManager.defaultBox, false);
        menus.Add(startScreen);

        InterfaceTool.Format_Rect_NPos(start_img.rectTransform, new Vector2(520, 480),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        start_img.color = menuColor;

        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = () => 
                {
                    newGame = true;
                    for (int x = 0; x < 3; x++)
                        fileSelectButtons[x].interactable = true;
                    AdvanceMenu("Select Menu");
                }; break;
                case 1: action = () => 
                {
                    newGame = false;
                    UpdateSelectButtons(ref fileSelectButtons);
                    AdvanceMenu("Select Menu");
                }; break;
                case 2: action = () => { AdvanceMenu("Options Menu"); }; break;
                case 3: action = SysManager.QuitGame; break;
                default: action = null; break;
            }
            
            GameObject s_button_obj = InterfaceTool.Button_Setup($"Start Button #{i}", menus[0].transform, out Image s_img, out Button s_button, SysManager.defaultButton, action);
            InterfaceTool.Format_Rect(s_img.rectTransform, new Vector2(400, 90),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -(60 + (90 * i))));
            GameObject text_obj = InterfaceTool.Text_Setup("Text", s_button_obj.transform, out Text s_text, false);
            InterfaceTool.Format_Rect(s_text.rectTransform);
            InterfaceTool.Format_Text(s_text, SysManager.defaultFont, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);
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


        // SELECT MENU

        menus.Add(InterfaceTool.Img_Setup("Select Menu", menu_canvas.transform, out Image load_img, SysManager.defaultBox, false));

        InterfaceTool.Format_Rect_NPos(load_img.rectTransform, new Vector2(1200, 400),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        load_img.color = menuColor;
        fileSelectButtons = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            int fileChoice = i;

            GameObject m_button_obj = InterfaceTool.Button_Setup($"File #{i}", menus[1].transform, out Image l_img, out fileSelectButtons[i], SysManager.defaultButton,
                () =>
                {
                    activeFile = fileChoice;
                    fileName.text = $"FILE { (char)('A' + fileChoice) }";

                    if (!newGame)
                    {
                        SysManager.fileManager.FileLoad(fileChoice);
                        SysManager.LoadAchievementsInterface();
                    }
                    AdvanceMenu(newGame ? "New Game Menu" : "Profile Menu");
                });
            InterfaceTool.Format_Rect(l_img.rectTransform, new Vector2(320, 260),
                new Vector2(0 + (0.5f * i), 0.5f), new Vector2(0 + (0.5f * i), 0.5f), new Vector2(0 + (0.5f * i), 0.5f), new Vector2(50 - (50 * i), 0));

            GameObject text_obj = InterfaceTool.Text_Setup("Text", m_button_obj.transform, out Text l_text, false);
            InterfaceTool.Format_Rect_NPos(l_text.rectTransform, new Vector2(320, 50),
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1));
            InterfaceTool.Format_Text(l_text, SysManager.defaultFont, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
            l_text.text = $"FILE { (char)('A' + i) }";
        }


        // NEW GAME MENU

        menus.Add(InterfaceTool.Img_Setup("New Game Menu", menu_canvas.transform, out Image new_menu_img, SysManager.defaultBox, false));

        InterfaceTool.Format_Rect(new_menu_img.rectTransform, new Vector2(1250, 620),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60));
        new_menu_img.color = menuColor;

        GameObject header_obj = InterfaceTool.Img_Setup("Header", menus[2].transform, out Image header_img, SysManager.defaultButton, false);
        InterfaceTool.Format_Rect_NPos(header_img.rectTransform, new Vector2(600, 120),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject header_text_obj = InterfaceTool.Text_Setup("Header Text", header_obj.transform, out Text header_text, false);
        InterfaceTool.Format_Rect(header_text.rectTransform);
        InterfaceTool.Format_Text(header_text, SysManager.defaultFont, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);
        header_text.text = "Choose a Difficulty!";

        GameObject new_desc = InterfaceTool.Text_Setup("Desc", menus[2].transform, out Text desc_text, false);
        InterfaceTool.Format_Rect(desc_text.rectTransform, new Vector2(-200, 80),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 30));
        InterfaceTool.Format_Text(desc_text, SysManager.defaultFont, 28, black, TextAnchor.MiddleLeft, FontStyle.Normal);

        for (int i = 0; i < 3; i++)
        {
            GameObject n_button_obj = InterfaceTool.Button_Setup($"Diff Option #{i}", menus[2].transform, out Image n_img, out Button n_button, SysManager.defaultButton,
                () =>
                {
                    SysManager.fileManager.NewFile(activeFile, (Difficulty)i);
                    SysManager.LoadAchievementsInterface();

                    fileCheck = CheckExistingFiles(out existingFiles);
                    newGame = false;
                    UpdateSelectButtons(ref fileSelectButtons);

                    AdvanceMenu("Profile Menu");
                    RemoveFromStack("New Game Menu");
                });
            InterfaceTool.Format_Rect(n_img.rectTransform, new Vector2(360, 340),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(40 + (380 * i), 20));

            GameObject text_obj = InterfaceTool.Text_Setup("Text", n_button_obj.transform, out Text n_text, false);
            InterfaceTool.Format_Rect(n_text.rectTransform, new Vector2(0, 80),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
            InterfaceTool.Format_Text(n_text, SysManager.defaultFont, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);

            EventTrigger trigger = n_button_obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry{ eventID = EventTriggerType.PointerEnter };

            switch (i)
            {
                case 0:
                    n_text.text = "Basic";
                    n_button.interactable = false;
                    entry.callback.AddListener((data) => { desc_text.text = "This is easy mode."; });
                    break;
                case 1:
                    n_text.text = "Standard";
                    entry.callback.AddListener((data) => { desc_text.text = "The standard Cat Clickers 2020 experience! " +
                        "This is currently the only way to play, so knock yourself out!"; });
                    break;
                case 2:
                    n_text.text = "Advanced";
                    n_button.interactable = false;
                    entry.callback.AddListener((data) => { desc_text.text = "This is hard mode."; });
                    break;
                default: n_text.text = ""; break;
            }
            trigger.triggers.Add(entry);
        }

        GameObject bar_vert = InterfaceTool.Img_Setup("Bar Horizontal", menus[2].transform, out Image bar_vert_img, false);
        InterfaceTool.Format_Rect(bar_vert_img.rectTransform, new Vector2(-340, 3),
            new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -180));
        bar_vert_img.color = black;
        
        
        // PROFILE MENU

        menus.Add(InterfaceTool.Img_Setup("Profile Menu", menu_canvas.transform, out Image profile_img, SysManager.defaultBox, false));

        InterfaceTool.Format_Rect(profile_img.rectTransform, new Vector2(1200, 700), new Vector2(0, -50));
        profile_img.color = menuColor;

        GameObject profileName_obj = InterfaceTool.Img_Setup("File Name", menus[3].transform, out Image profileName_img, SysManager.defaultBox, false);
        InterfaceTool.Format_Rect(profileName_img.rectTransform, new Vector2(200, 100),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(50, 80));
        GameObject profileName_text_obj = InterfaceTool.Text_Setup("File Name Text", profileName_obj.transform, out fileName, false);
        InterfaceTool.Format_Rect(fileName.rectTransform);
        InterfaceTool.Format_Text(fileName, SysManager.defaultFont, 36, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);

        for (int i = 0; i < 5; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = () => { }; break;
                case 1: action = () => { SysManager.LoadClassicMode(); SysManager.QuitMainMenu(); }; break;
                case 2: action = () => { }; break;
                case 3: action = () => AdvanceMenu("Records Menu"); break;
                case 4: action = () => 
                {
                    SysManager.fileManager.DeleteFile();
                    SysManager.QuitAchievementsInterface();
                    CheckExistingFiles(out existingFiles);
                    GoToStart();
                    
                }; break;
                default: action = null; break;
            }

            GameObject p_button_obj = InterfaceTool.Button_Setup("Button", menus[3].transform, out Image p_button_bg, out Button p_button, SysManager.defaultButton, action);
            GameObject p_text_obj = InterfaceTool.Text_Setup("Button Text", p_button_obj.transform, out Text p_text, false);
            InterfaceTool.Format_Rect_NPos(p_text.rectTransform, new Vector2(0, 110),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
            InterfaceTool.Format_Text(p_text, SysManager.defaultFont, 48, black, TextAnchor.MiddleCenter, FontStyle.Normal);

            switch (i)
            {
                case 0:
                    p_button_obj.name = "Story Mode";
                    InterfaceTool.Format_Rect(p_button_bg.rectTransform, new Vector2(840, 280), 
                        new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -40));
                    p_text.text = "Story Mode";
                    p_button.interactable = false;
                    break;
                case 1:
                    p_button_obj.name = "Classic Mode";
                    InterfaceTool.Format_Rect(p_button_bg.rectTransform, new Vector2(660, 280),
                        new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(40, 40));
                    p_text.text = "Classic Mode";
                    break;
                case 2:
                    p_button_obj.name = "Extras";
                    InterfaceTool.Format_Rect(p_button_bg.rectTransform, new Vector2(240, 280), 
                        new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -40));
                    p_text.text = "Extras";
                    p_button.interactable = false;
                    break;
                case 3:
                    p_button_obj.name = "Records";
                    InterfaceTool.Format_Rect(p_button_bg.rectTransform, new Vector2(420, 280),
                        new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 40));
                    p_text.text = "Records";
                    break;
                case 4:
                    p_button_obj.name = "Delete";
                    InterfaceTool.Format_Rect(p_button_bg.rectTransform, new Vector2(110, 100),
                        new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(50, -80));
                    InterfaceTool.Format_Rect(p_text.rectTransform);
                    p_button_bg.color = new Color(1, 0, 0);
                    p_text.fontStyle = FontStyle.Bold;
                    p_text.fontSize = 24;
                    p_text.text = "DELETE";
                    p_text.color = Color.white;
                    break;
                default:
                    break;
            }
        }

        // OPTIONS

        menus.Add(InterfaceTool.Img_Setup("Options Menu", menu_canvas.transform, out Image opts_img, SysManager.defaultBox, false));

        InterfaceTool.Format_Rect(opts_img.rectTransform, new Vector2(1200, 700), new Vector2(0, -50));
        opts_img.color = menuColor;

        GameObject opts_header = InterfaceTool.Text_Setup("Options Header", menus[4].transform, out Text opts_header_text, false);
        InterfaceTool.Format_Rect(opts_header_text.rectTransform, new Vector2(0, 120),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
        InterfaceTool.Format_Text(opts_header_text, SysManager.defaultFont, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        opts_header_text.text = "OPTIONS";

        for (int i = 0; i < 5; i++)
        {
            float pos_y = 135 - (100 * i);
            GameObject opt_name = InterfaceTool.Text_Setup($"Option Text #{i}", menus[4].transform, out Text opt_text, false);
            InterfaceTool.Format_Rect(opt_text.rectTransform, new Vector2(500, 80),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(175, pos_y));
            InterfaceTool.Format_Text(opt_text, SysManager.defaultFont, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);

            GameObject opt_bar = InterfaceTool.Img_Setup($"Option #{i}", menus[4].transform, out Image opt_bar_img, SysManager.uiSprites[3], false);
            InterfaceTool.Format_Rect(opt_bar_img.rectTransform, new Vector2(400, 80),
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-125, pos_y));
            opt_bar_img.color = new Color(0.39f, 0.36f, 0.4f);

            if (i == 0 || i == 4)
            {
                GameObject info_obj = InterfaceTool.Img_Setup("Info", opt_bar.transform, out Image info_img, true);
                InterfaceTool.Format_Rect(info_img.rectTransform, new Vector2(60, 60),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-10, 0));

                GameObject indicator_obj = InterfaceTool.Text_Setup("Indicator", opt_bar.transform, out Text indicator_text, false);
                InterfaceTool.Format_Rect_NPos(indicator_text.rectTransform, new Vector2(330, 80),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                InterfaceTool.Format_Text(indicator_text, SysManager.defaultFont, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
                indicator_text.text = (i == 0) ? "High" : "Standard"; // Filler dialogue for now
            }

            switch (i)
            {
                case 0:
                    opt_text.text = "Quality"; break;
                case 1:
                    opt_text.text = "Master Volume"; break;
                case 2:
                    opt_text.text = "Music Volume"; break;
                case 3:
                    opt_text.text = "Sound Volume"; break;
                case 4:
                    opt_text.text = "Difficulty"; break;
                default:
                    break;
            }
        }

        GameObject opt_bar_vert = InterfaceTool.Img_Setup("Bar Horizontal", opts_header.transform, out Image opt_bar_vert_img, false);
        InterfaceTool.Format_Rect_NPos(opt_bar_vert_img.rectTransform, new Vector2(1000, 3),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        opt_bar_vert_img.color = Color.white;

        GameObject comingSoonObj = InterfaceTool.Img_Setup("COMING SOON", menus.Find(menu => menu.name == "Options Menu").transform, out Image comingSoonImg, true);
        InterfaceTool.Format_Rect(comingSoonImg.rectTransform);
        comingSoonImg.color = new Color(0, 0, 0, 0.75f);

        GameObject comingSoonTxtObj = InterfaceTool.Text_Setup("COMING SOON Text", comingSoonObj.transform, out Text comingSoonTxt, false);
        InterfaceTool.Format_Rect(comingSoonTxt.rectTransform);
        InterfaceTool.Format_Text(comingSoonTxt, SysManager.defaultFont, 128, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
        comingSoonTxt.rectTransform.Rotate(new Vector3(0, 0, 20));
        comingSoonTxt.text = "Coming Soon";
        

        // RECORDS

        menus.Add(InterfaceTool.Img_Setup("Records Menu", menu_canvas.transform, out Image records_img, SysManager.defaultBox, false));

        InterfaceTool.Format_Rect_NPos(records_img.rectTransform, new Vector2(600, 700),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1, 0.5f));
        records_img.color = new Color(0.7f, 0.7f, 0.75f);

        GameObject achievements_button_obj = InterfaceTool.Button_Setup("Achievements Button", menus[5].transform,
            out Image achievements_img, out Button achievements_button, SysManager.defaultButton, 
            () => { SysManager.achievementsInterface.Display_Achievements(true); });

        InterfaceTool.Format_Rect(achievements_img.rectTransform, new Vector2(-80, 200),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -50));

        GameObject achievement_button_txt = InterfaceTool.Text_Setup("A Button Text", achievements_button_obj.transform, out Text a_button_txt, false);
        InterfaceTool.Format_Rect(a_button_txt.rectTransform);
        InterfaceTool.Format_Text(a_button_txt, SysManager.defaultFont, 48, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        a_button_txt.text = "ACHIEVEMENTS";

        GameObject stats_header = InterfaceTool.Text_Setup("Stats Header", menus[5].transform, out Text stats_header_txt, false);
        InterfaceTool.Format_Rect(stats_header_txt.rectTransform, new Vector2(540, 60),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(30, 0));
        InterfaceTool.Format_Text(stats_header_txt, SysManager.defaultFont, 36, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
        stats_header_txt.text = "STATS";

        GameObject stats = InterfaceTool.Text_Setup("Stats Text", stats_header.transform, out Text stats_txt, false);
        InterfaceTool.Format_Rect_NPos(stats_txt.rectTransform, new Vector2(0, 600), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1));
        InterfaceTool.Format_Text(stats_txt, SysManager.defaultFont, 28, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
        stats_txt.lineSpacing = 1.5f;
        stats_txt.text = UpdateStats();

        // Once everything is loaded in, disable all menus except for the start menu.
        for (int i = 1; i < menus.Count; i++)
        {
            menus[i].SetActive(false);
        }

        GoToStart();
    }

    string UpdateStats()
    {
        return SysManager.profile.clscSaveData != null ? 
            $"Current bits : { SysManager.profile.clscSaveData.CurrencyCurrent.ToString("#,0.#") }\n" +
            $"Total bits   : { SysManager.profile.clscSaveData.CurrencyTotal.ToString("#,0.#") }\n" +
            $"Current BPS  : { SysManager.profile.clscSaveData.BitsPerSecond.ToString("#,0.#") }\n" +
            $"Time played  : { "TIME PLAYED PLACEHOLDER" } seconds\n" +
            $"Time started : { "TIME STARTED PLACEHOLDER" }" : "";
    }
}
 
 
 