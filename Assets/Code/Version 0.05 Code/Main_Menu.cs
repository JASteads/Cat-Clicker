using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
    public int active_menu = 0; // Used to coordinate where to go when back button is clicked
    public int active_file = 0;
    public bool new_game, file_check;
    bool[] existing_files;
    public Text file_name;
    
    Text main_title;
    Color black, menu_color;

    GameObject back_obj;
    Button[] file_select_buttons;
    Button cont_button;
    GameObject[] menus;
    /* MENU IDs
        0 - Start menu
        1 - Profile Select
        2 - New Game
        3 - Profile Menu
        4 - Options
    */

    void Change_Menu(int next_menu, bool go_back)
    {
        menus[active_menu].SetActive(false);
        if (go_back)
            switch (next_menu)
            {
                case 1: active_menu = 0; break;
                case 2: active_menu = 1; break;
                case 3: active_menu = 1; break;
                case 4: active_menu = 0; break;
                case 5: active_menu = 3; break;
                default: break;
            }
        else { active_menu = next_menu; }
        menus[active_menu].SetActive(true);

        // Move title and toggle back button depending on which menu is active
        if (active_menu != 0)
        {
            if (!back_obj.activeSelf) back_obj.SetActive(true);
            UI_Tool.FormatRect(main_title.rectTransform, new Vector2(350, 120),
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -80));
            UI_Tool.FormatText(main_title, Database.arial, 48, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
        }
        else
        {
            back_obj.SetActive(false);
            UI_Tool.FormatRect(main_title.rectTransform, new Vector2(620, 200),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0), new Vector2(0, 50));
            UI_Tool.FormatText(main_title, Database.arial, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        }
    }
    bool Check_Existing_Files(out bool[] existing_files)
    {
        existing_files = new bool[3];
        for (int i = 0; i < 3; i++)
            existing_files[i] = System.IO.File.Exists($"{File_Manager.save_directory}File { (char)('A' + i) }.txt") ?
                true : false;
        for (int i = 0; i < 3; i++)
            if (existing_files[i])
            {
                if (cont_button != null && !cont_button.interactable) cont_button.interactable = true;
                return true;
            }
        if (cont_button != null && cont_button.interactable) cont_button.interactable = false;
        return false;
    }
    void Update_Select_Buttons(ref Button[] select_buttons)
    {
        for (int i = 0; i < select_buttons.Length; i++)
            select_buttons[i].interactable = existing_files[i] ? true : false;
    }

    void Awake()
    {
        gameObject.tag = "Main";
        menus = new GameObject[6];
        
        black = new Color(0.2f, 0.2f, 0.2f);
        menu_color = new Color(0.7f, 0.7f, 0.75f);

        GameObject menu_canvas = UI_Tool.CanvasSetup("Menu Canvas", gameObject.transform);

        file_check = Check_Existing_Files(out existing_files);


        // MENU-INDPENDENT OBJECTS

        GameObject backdrop_obj = UI_Tool.ImgSetup("Backdrop", menu_canvas.transform, out Image backdrop, Database.ui_sprites[5], false);
        backdrop.type = Image.Type.Tiled;
        UI_Tool.FormatRect(backdrop.rectTransform, new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));
        backdrop.color = new Color(0.3f, 0.2f, 0.4f);

        GameObject title_obj = UI_Tool.TextSetup("Game Title", menu_canvas.transform, out main_title, false);
        UI_Tool.FormatRect(main_title.rectTransform, new Vector2(620, 200), new Vector2(0, 200));
        UI_Tool.FormatText(main_title, Database.arial, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        main_title.text = "CAT CLICKER (2020)";

        GameObject version_num = UI_Tool.TextSetup("Version Num", menu_canvas.transform, out Text ver_text, false);
        UI_Tool.FormatRect(ver_text.rectTransform, new Vector2(240, 50),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 0));
        UI_Tool.FormatText(ver_text, Database.arial, 28, Color.white, TextAnchor.MiddleRight, FontStyle.Italic);
        ver_text.text = $"ver. { Application.version }";

        GameObject copyright_obj = UI_Tool.TextSetup("Copyright", menu_canvas.transform, out Text cpy_text, false);
        UI_Tool.FormatRect(cpy_text.rectTransform, new Vector2(240, 50),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 0));
        UI_Tool.FormatText(cpy_text, Database.arial, 28, Color.white, TextAnchor.MiddleLeft, FontStyle.Italic);
        cpy_text.text = "District 4311";

        back_obj = UI_Tool.ButtonSetup("Back Button", menu_canvas.transform, out Image back_img, out Button back_button, Database.default_button, () => Change_Menu(active_menu, true));
        UI_Tool.FormatRect(back_img.rectTransform, new Vector2(140, 80),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-520, 40));
        GameObject back_text_obj = UI_Tool.TextSetup("Back Text", back_obj.transform, out Text back_text, false);
        UI_Tool.FormatRect(back_text.rectTransform);
        UI_Tool.FormatText(back_text, Database.arial, 48, black, TextAnchor.MiddleCenter, FontStyle.Bold);
        back_text.alignByGeometry = true;
        back_text.text = "<<";

        
        // START MENU

        menus[0] = UI_Tool.ImgSetup("Start Menu", menu_canvas.transform, out Image start_img, Database.default_box, false);
        UI_Tool.FormatRectNPos(start_img.rectTransform, new Vector2(520, 480),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        start_img.color = menu_color;

        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = () => 
                {
                    new_game = true;
                    Change_Menu(1, false);
                    for (int x = 0; x < 3; x++)
                        file_select_buttons[x].interactable = true;
                }; break;
                case 1: action = () => 
                {
                    new_game = false;
                    Change_Menu(1, false);
                    Update_Select_Buttons(ref file_select_buttons);
                }; break;
                case 2: action = () => { Change_Menu(4, false); }; break;
                case 3: action = Database.Quit_Game; break;
                default: action = null; break;
            }
            
            GameObject s_button_obj = UI_Tool.ButtonSetup($"Start Button #{i}", menus[0].transform, out Image s_img, out Button s_button, Database.default_button, action);
            UI_Tool.FormatRect(s_img.rectTransform, new Vector2(400, 90),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -(60 + (90 * i))));
            GameObject text_obj = UI_Tool.TextSetup("Text", s_button_obj.transform, out Text s_text, false);
            UI_Tool.FormatRect(s_text.rectTransform);
            UI_Tool.FormatText(s_text, Database.arial, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);
            switch (i)
            {
                case 0: s_text.text = "New Game"; break;
                case 1: s_text.text = "Continue"; cont_button = s_button; break;
                case 2: s_text.text = "Options"; break;
                case 3: s_text.text = "Quit Game"; break;
                default: s_text.text = ""; break;
            }
            
            if (i == 1 && !file_check)
                s_button.interactable = false;
        }


        // SELECT MENU

        menus[1] = UI_Tool.ImgSetup("Select Menu", menu_canvas.transform, out Image load_img, Database.default_box, false);
        UI_Tool.FormatRectNPos(load_img.rectTransform, new Vector2(1200, 400),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1));
        load_img.color = menu_color;
        file_select_buttons = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            int file_choice = i;

            GameObject m_button_obj = UI_Tool.ButtonSetup($"File #{i}", menus[1].transform, out Image l_img, out file_select_buttons[i], Database.default_button,
                () =>
                {
                    active_file = file_choice;
                    file_name.text = $"FILE { (char)('A' + file_choice) }";

                    if (!new_game) File_Manager.File_Load(file_choice);
                    Change_Menu(new_game ? 2 : 3, false);
                });
            UI_Tool.FormatRect(l_img.rectTransform, new Vector2(320, 260),
                new Vector2(0 + (0.5f * i), 0.5f), new Vector2(0 + (0.5f * i), 0.5f), new Vector2(0 + (0.5f * i), 0.5f), new Vector2(50 - (50 * i), 0));

            GameObject text_obj = UI_Tool.TextSetup("Text", m_button_obj.transform, out Text l_text, false);
            UI_Tool.FormatRectNPos(l_text.rectTransform, new Vector2(320, 50),
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 1));
            UI_Tool.FormatText(l_text, Database.arial, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
            l_text.text = $"FILE { (char)('A' + i) }";
        }


        // NEW GAME MENU

        menus[2] = UI_Tool.ImgSetup("New Game Menu", menu_canvas.transform, out Image new_menu_img, Database.default_box, false);
        UI_Tool.FormatRect(new_menu_img.rectTransform, new Vector2(1250, 620),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60));
        new_menu_img.color = menu_color;

        GameObject header_obj = UI_Tool.ImgSetup("Header", menus[2].transform, out Image header_img, Database.default_button, false);
        UI_Tool.FormatRectNPos(header_img.rectTransform, new Vector2(600, 120),
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject header_text_obj = UI_Tool.TextSetup("Header Text", header_obj.transform, out Text header_text, false);
        UI_Tool.FormatRect(header_text.rectTransform);
        UI_Tool.FormatText(header_text, Database.arial, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);
        header_text.text = "Choose a Difficulty!";

        GameObject new_desc = UI_Tool.TextSetup("Desc", menus[2].transform, out Text desc_text, false);
        UI_Tool.FormatRect(desc_text.rectTransform, new Vector2(-200, 80),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 30));
        UI_Tool.FormatText(desc_text, Database.arial, 28, black, TextAnchor.MiddleLeft, FontStyle.Normal);

        for (int i = 0; i < 3; i++)
        {
            GameObject n_button_obj = UI_Tool.ButtonSetup($"Diff Option #{i}", menus[2].transform, out Image n_img, out Button n_button, Database.default_button,
                () =>
                {
                    File_Manager.New_File(i, active_file);
                    file_check = Check_Existing_Files(out existing_files);
                    new_game = false;
                    Change_Menu(3, false);
                    Update_Select_Buttons(ref file_select_buttons);
                });
            UI_Tool.FormatRect(n_img.rectTransform, new Vector2(360, 340),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(40 + (380 * i), 20));

            GameObject text_obj = UI_Tool.TextSetup("Text", n_button_obj.transform, out Text n_text, false);
            UI_Tool.FormatRect(n_text.rectTransform, new Vector2(0, 80),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
            UI_Tool.FormatText(n_text, Database.arial, 36, black, TextAnchor.MiddleCenter, FontStyle.Normal);

            EventTrigger trigger = n_button_obj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry{ eventID = EventTriggerType.PointerEnter };

            switch (i)
            {
                case 0:
                    n_text.text = "Simple";
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

        GameObject bar_vert = UI_Tool.ImgSetup("Bar Horizontal", menus[2].transform, out Image bar_vert_img, false);
        UI_Tool.FormatRect(bar_vert_img.rectTransform, new Vector2(-340, 3),
            new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -180));
        bar_vert_img.color = black;
        
        
        // PROFILE MENU

        menus[3] = UI_Tool.ImgSetup("Profile Menu", menu_canvas.transform, out Image profile_img, Database.default_box, false);
        UI_Tool.FormatRect(profile_img.rectTransform, new Vector2(1200, 700), new Vector2(0, -50));
        profile_img.color = menu_color;

        GameObject profile_name_obj = UI_Tool.ImgSetup("File Name", menus[3].transform, out Image profile_name_img, Database.default_box, false);
        UI_Tool.FormatRect(profile_name_img.rectTransform, new Vector2(200, 100),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(50, 80));
        GameObject profile_name_text_obj = UI_Tool.TextSetup("File Name Text", profile_name_obj.transform, out file_name, false);
        UI_Tool.FormatRect(file_name.rectTransform);
        UI_Tool.FormatText(file_name, Database.arial, 36, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);

        for (int i = 0; i < 5; i++)
        {
            UnityEngine.Events.UnityAction action;
            switch (i)
            {
                case 0: action = () => { }; break;
                case 1: action = () => { Database.Begin_Classic(); Database.Quit_Main_Menu(); }; break;
                case 2: action = () => { }; break;
                case 3: action = () => Change_Menu(5, false); break;
                case 4: action = () => 
                {
                    Change_Menu(0, false);
                    File_Manager.File_Delete(Database.data.slot);
                    Check_Existing_Files(out existing_files);
                }; break;
                default: action = null; break;
            }

            GameObject p_button_obj = UI_Tool.ButtonSetup("Button", menus[3].transform, out Image p_button_bg, out Button p_button, Database.default_button, action);
            GameObject p_text_obj = UI_Tool.TextSetup("Button Text", p_button_obj.transform, out Text p_text, false);
            UI_Tool.FormatRectNPos(p_text.rectTransform, new Vector2(0, 110),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
            UI_Tool.FormatText(p_text, Database.arial, 48, black, TextAnchor.MiddleCenter, FontStyle.Normal);

            switch (i)
            {
                case 0:
                    p_button_obj.name = "Story Mode";
                    UI_Tool.FormatRect(p_button_bg.rectTransform, new Vector2(840, 280), 
                        new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -40));
                    p_text.text = "Story Mode";
                    p_button.interactable = false;
                    break;
                case 1:
                    p_button_obj.name = "Classic Mode";
                    UI_Tool.FormatRect(p_button_bg.rectTransform, new Vector2(660, 280),
                        new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(40, 40));
                    p_text.text = "Classic Mode";
                    break;
                case 2:
                    p_button_obj.name = "Extras";
                    UI_Tool.FormatRect(p_button_bg.rectTransform, new Vector2(240, 280), 
                        new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -40));
                    p_text.text = "Extras";
                    p_button.interactable = false;
                    break;
                case 3:
                    p_button_obj.name = "Records";
                    UI_Tool.FormatRect(p_button_bg.rectTransform, new Vector2(420, 280),
                        new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-40, 40));
                    p_text.text = "Records";
                    break;
                case 4:
                    p_button_obj.name = "Delete";
                    UI_Tool.FormatRect(p_button_bg.rectTransform, new Vector2(110, 100),
                        new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(50, -80));
                    UI_Tool.FormatRect(p_text.rectTransform);
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

        menus[4] = UI_Tool.ImgSetup("Options Menu", menu_canvas.transform, out Image opts_img, Database.default_box, false);
        UI_Tool.FormatRect(opts_img.rectTransform, new Vector2(1200, 700), new Vector2(0, -50));
        opts_img.color = menu_color;

        GameObject opts_header = UI_Tool.TextSetup("Options Header", menus[4].transform, out Text opts_header_text, false);
        UI_Tool.FormatRect(opts_header_text.rectTransform, new Vector2(0, 120),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -20));
        UI_Tool.FormatText(opts_header_text, Database.arial, 64, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        opts_header_text.text = "OPTIONS";

        for (int i = 0; i < 5; i++)
        {
            float pos_y = 135 - (100 * i);
            GameObject opt_name = UI_Tool.TextSetup($"Option Text #{i}", menus[4].transform, out Text opt_text, false);
            UI_Tool.FormatRect(opt_text.rectTransform, new Vector2(500, 80),
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(175, pos_y));
            UI_Tool.FormatText(opt_text, Database.arial, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);

            GameObject opt_bar = UI_Tool.ImgSetup($"Option #{i}", menus[4].transform, out Image opt_bar_img, Database.ui_sprites[3], false);
            UI_Tool.FormatRect(opt_bar_img.rectTransform, new Vector2(400, 80),
                new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-125, pos_y));
            opt_bar_img.color = new Color(0.39f, 0.36f, 0.4f);

            if (i == 0 || i == 4)
            {
                GameObject info_obj = UI_Tool.ImgSetup("Info", opt_bar.transform, out Image info_img, true);
                UI_Tool.FormatRect(info_img.rectTransform, new Vector2(60, 60),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-10, 0));

                GameObject indicator_obj = UI_Tool.TextSetup("Indicator", opt_bar.transform, out Text indicator_text, false);
                UI_Tool.FormatRectNPos(indicator_text.rectTransform, new Vector2(330, 80),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                UI_Tool.FormatText(indicator_text, Database.arial, 32, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
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

        GameObject opt_bar_vert = UI_Tool.ImgSetup("Bar Horizontal", opts_header.transform, out Image opt_bar_vert_img, false);
        UI_Tool.FormatRectNPos(opt_bar_vert_img.rectTransform, new Vector2(1000, 3),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        opt_bar_vert_img.color = Color.white;

        // RECORDS

        menus[5] = UI_Tool.ImgSetup("Records Menu", menu_canvas.transform, out Image records_img, Database.default_box, false);
        UI_Tool.FormatRectNPos(records_img.rectTransform, new Vector2(600, 700),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1, 0.5f));
        records_img.color = new Color(0.7f, 0.7f, 0.75f);

        GameObject achievements_button_obj = UI_Tool.ButtonSetup("Achievements Button", menus[5].transform,
            out Image achievements_img, out Button achievements_button, Database.default_button, 
            () => { Database.achievements.Display_Achievements(true); });

        UI_Tool.FormatRect(achievements_img.rectTransform, new Vector2(-80, 200),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -50));

        GameObject achievement_button_txt = UI_Tool.TextSetup("A Button Text", achievements_button_obj.transform, out Text a_button_txt, false);
        UI_Tool.FormatRect(a_button_txt.rectTransform);
        UI_Tool.FormatText(a_button_txt, Database.arial, 48, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        a_button_txt.text = "ACHIEVEMENTS";

        GameObject stats_header = UI_Tool.TextSetup("Stats Header", menus[5].transform, out Text stats_header_txt, false);
        UI_Tool.FormatRect(stats_header_txt.rectTransform, new Vector2(540, 60),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(30, 0));
        UI_Tool.FormatText(stats_header_txt, Database.arial, 36, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
        stats_header_txt.text = "STATS";

        GameObject stats = UI_Tool.TextSetup("Stats Text", stats_header.transform, out Text stats_txt, false);
        UI_Tool.FormatRectNPos(stats_txt.rectTransform, new Vector2(0, 600), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1));
        UI_Tool.FormatText(stats_txt, Database.arial, 28, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
        stats_txt.lineSpacing = 1.5f;
        stats_txt.text = Update_Stats();

        // Once everything is loaded in, disable all menus except for the start menu.
        for (int i = 1; i < menus.Length; i++) menus[i].SetActive(false);
        back_obj.SetActive(false);
    }

    string Update_Stats()
    {
        return Database.data != null ? $"Current bits : { Database.data.currency.ToString("#,0.#") }\nTotal bits : { Database.data.total_money.ToString("#,0.#") }\n"
            + $"Current BPS : { Database.data.bps.ToString("#,0.#") }\nTime played : { Database.data.time_played } seconds\n"
            + $"Time started : { System.DateTime.FromBinary(Database.data.time_started) }" : "";
    }
}