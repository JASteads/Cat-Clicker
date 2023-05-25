using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements_Data
{
    public delegate bool A_Con();
    public static Achievement[] achievements = new Achievement[6];
    public static List<Achievement> pending_acheivements = new List<Achievement>();

    public struct Color_Bits
    {
        public int white, red, blue, green, purple;

        public Color_Bits(int w, int r, int b, int g, int p)
        {
            white = w;
            red = r;
            blue = b;
            green = g;
            purple = p;
        }
        public Color_Bits(int val)
        { white = red = blue = green = purple = val; }
    }

    public class Achievement
    {
        public static int next_id = 500;
        public enum Type
        { STORY, CP, BPS, RESEARCH, SECRET };

        public Type type;
        public int status;          // 0 = Unlocked ; 1 = Locked ; 2 = Hidden
        public int value, id;
        public double progress, max;   // Values used for the progress bar
        public Func<double> current;
        public DateTime date_earned;
        public string title, desc;
        public A_Con con;
        public Effect on_unlock;
        public Achievement[] next_achievements;

        public Achievement(string title, string desc, Type type, int value, A_Con con, Effect on_unlock)
        {
            id = next_id++;
            this.title = title;
            this.desc = desc;
            this.type = type;
            this.value = value;
            progress = 0;
            max = 1;
            current = null;
            status = 1;
            date_earned = DateTime.FromBinary(0);
            this.con = con;
            this.on_unlock = on_unlock;
            next_achievements = null;
        }

        public Achievement(string title, string desc, Type type, int value, Func<double> current, double max, A_Con con, Effect on_unlock)
        {
            id = next_id++;
            this.title = title;
            this.desc = desc;
            this.type = type;
            this.value = value;
            progress = 0;
            this.current = current;
            this.max = max;
            status = 1;
            date_earned = DateTime.FromBinary(0);
            this.con = con;
            this.on_unlock = on_unlock;
            next_achievements = null;
        }
    }
    public static void Activate_Achievement(Achievement achievement)
    {
        DateTime Get_Time() { return DateTime.Now; };
        achievement.date_earned = Get_Time();

        Debug.Log($"Acheievement Unlocked : {achievement.title}");
        int index = achievement.id - 500;
        achievements[index].on_unlock?.Invoke();
        achievements[index].status = 0;
        achievement.progress = achievement.max;
        if (achievements[index].next_achievements != null)
            foreach (Achievement a in achievements[index].next_achievements)
                pending_acheivements.Add(a);

        pending_acheivements.Remove(achievement);

        switch (achievements[index].type)
        {
            case Achievement.Type.STORY:
                Database.data.color_bits.white += achievements[index].value; break;
            case Achievement.Type.CP:
                Database.data.color_bits.red += achievements[index].value; break;
            case Achievement.Type.BPS:
                Database.data.color_bits.blue += achievements[index].value; break;
            case Achievement.Type.RESEARCH:
                Database.data.color_bits.green += achievements[index].value; break;
            case Achievement.Type.SECRET:
                Database.data.color_bits.purple += achievements[index].value; break;
            default:
                Debug.Log("Invalid index color bit assignment"); break;
        }
    }
    public static void Set_First_Acheievements()
    {
        for (int i = 0; i < 4; i++)
            pending_acheivements.Add(achievements[i]);
    }

    public static void Init_Achievements()
    {
        Achievement.next_id = 500; // Set the id counter back to its initial value.

        // Define achievements

        achievements[0] = new Achievement("First Click!", "Start your journey with your first click ever!",
            Achievement.Type.CP, 1,
            () =>  { return Database.data.total_clicks > 0; }, 
            null);
        achievements[1] = new Achievement("Start Your Engines", "Drop the first seeds, and they do the work for you. You now make 1 Bit per second!",
            Achievement.Type.BPS, 5, () => { return Database.data.bps; }, 1,
            () =>  { return Database.data.bps >= 1; }, 
            null);
        achievements[2] = new Achievement("Bit Hoarding", "You've mustered up more than 100 bits in total. Now you've got a personal stash!",
            Achievement.Type.CP, 10, () => { return Database.data.total_money; }, 100,
            () =>  { return Database.data.total_money >= 100; }, 
            null);
        achievements[3] = new Achievement("FEVER!", "You've reached a new height of production with your first Fever boost!",
            Achievement.Type.STORY, 1,
            () => { return Database.data.fever_data.time_active > 0; },
            null);
        achievements[4] = new Achievement("One-Hundred Clicks", "That's 100 total clicks. Click click away...",
            Achievement.Type.CP, 5, () => { return Database.data.total_clicks; }, 100,
            () => { return Database.data.total_clicks > 99; },
            null);
        achievements[5] = new Achievement("Beat the Game", "Purchase the final upgrade that wins the game!",
            Achievement.Type.STORY, 777,
            null,
            null);
        

        // Set next achievements

        achievements[0].next_achievements = new[] { achievements[5] };
    }

    public static void Check_Achievements()
    {
        for (int i = 0; i < pending_acheivements.Count; i++)
            if (pending_acheivements[i].con != null)
                if (pending_acheivements[i].con())
                    Activate_Achievement(pending_acheivements[i]);
                else if (pending_acheivements[i].current != null)
                    pending_acheivements[i].progress = pending_acheivements[i].current();
    }
}

public class Achievements : MonoBehaviour
{
    public class A_Block
    {
        public GameObject block_obj;
        public Image block, icon, progress_bar, cover, bar;
        public Text value, title, desc, date, progress_txt;
    }

    public GameObject achievement_interface;
    public RectTransform a_list;
    public A_Block[] a_blocks;
    
    void Awake()
    {
        achievement_interface = UI_Tool.CanvasSetup("Achievements Canvas", gameObject.transform);

        GameObject backdrop = UI_Tool.ImgSetup("Backdrop", achievement_interface.transform, out Image backdrop_img, true);
        UI_Tool.FormatRect(backdrop_img.rectTransform);
        backdrop_img.color = new Color(0, 0, 0, 0.3f);

        GameObject a_panel = UI_Tool.ImgSetup("Panel", achievement_interface.transform, out Image panel_img, Database.default_box, false);
        panel_img.color = Color.white;
        UI_Tool.FormatRectNPos(panel_img.rectTransform, new Vector2(1300, 900));

        GameObject a_content = UI_Tool.ImgSetup("Contents", a_panel.transform, out Image content_img, false);
        content_img.color = new Color(0.29f, 0.27f, 0.3f);
        UI_Tool.FormatRect(content_img.rectTransform, new Vector2(-90, -90), new Vector2(0, 0), new Vector2(1, 1), new Vector2(-15, -15));
        a_content.AddComponent<RectMask2D>();

        a_list = new GameObject("List").AddComponent<RectTransform>();
        a_list.transform.SetParent(a_content.transform, false);
        UI_Tool.FormatRectNPos(a_list, new Vector2(0, 185 * Achievements_Data.achievements.Length), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));

        GameObject a_header = UI_Tool.ImgSetup("Header", a_panel.transform, out Image header_img, Database.default_box, false);
        UI_Tool.FormatRectNPos(header_img.rectTransform, new Vector2(760, 120), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject a_header_txt_obj = UI_Tool.TextSetup("Title", a_header.transform, out Text a_header_txt, false);
        UI_Tool.FormatRect(a_header_txt.rectTransform);
        UI_Tool.FormatText(a_header_txt, Database.arial, 48, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
        a_header_txt.text = "ACHIEVEMENTS";

        GameObject scrollbar_obj = UI_Tool.ScrollbarSetup(a_panel.transform, a_content, a_list, 
            new Vector2(30, -90), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f), new Vector2(-30, -15));

        GameObject back_button_obj = UI_Tool.ButtonSetup("Back Button", a_panel.transform, out Image back_img, out Button back_button, Database.default_button,
            () => DisplayAchievements(false));
        UI_Tool.FormatRect(back_img.rectTransform, new Vector2(70, 70),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(10, -10));
        GameObject back_button_txt_obj = UI_Tool.TextSetup("B Button Text", back_button_obj.transform, out Text back_button_txt, false);
        UI_Tool.FormatRect(back_button_txt.rectTransform);
        UI_Tool.FormatText(back_button_txt, Database.arial, 36, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        back_button_txt.text = "X";

        gameObject.SetActive(false);
    }

    public void DisplayAchievements(bool set_active)
    {
        if (set_active) Update_Listing();
        gameObject.SetActive(set_active);

        UI_Tool.ToggleCanvasPriority(GameObject.FindWithTag("Main"), achievement_interface.GetComponent<Canvas>());
    }


    public void Update_Listing()
    {
        if (a_blocks != null)
            for (int i = 0; i < a_blocks.Length; i++)
            {
                a_blocks[i].cover.enabled = Achievements_Data.achievements[i].status == 0 ? false : true;
                a_blocks[i].progress_txt.text = $"{ Database.To_Bit_Notation(Achievements_Data.achievements[i].progress, "#,0.#") } / { Achievements_Data.achievements[i].max }";
                a_blocks[i].bar.rectTransform.localScale = new Vector2(Achievements_Data.achievements[i].status == 0 ?
                    1 : (float)(Achievements_Data.achievements[i].progress / Achievements_Data.achievements[i].max), 1);
            }
        else
        {
            a_blocks = new A_Block[Achievements_Data.achievements.Length];

            for (int i = 0; i < a_blocks.Length; i++)
            {
                a_blocks[i] = new A_Block();

                GameObject icon_obj, value_obj, progress_bar_obj, cover_obj;
                GameObject title_obj, desc_obj, date_obj, bar_obj, progress_txt_obj;

                a_blocks[i].block_obj = UI_Tool.ImgSetup($"Achievement #{i}", a_list, out a_blocks[i].block, Database.default_box, false);
                a_blocks[i].block.color = new Color(0.75f, 0.72f, 0.8f);
                UI_Tool.FormatRect(a_blocks[i].block.rectTransform, new Vector2(0, 185), new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(0.5f, 1), new Vector2(0, -185 * i));

                icon_obj = UI_Tool.ImgSetup("Icon", a_blocks[i].block_obj.transform, out Image icon, false);
                UI_Tool.FormatRect(icon.rectTransform, new Vector2(100, 100), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -40));

                switch (Achievements_Data.achievements[i].type)
                {
                    case Achievements_Data.Achievement.Type.STORY:
                        icon.color = new Color(1, 1, 1); break;
                    case Achievements_Data.Achievement.Type.CP:
                        icon.color = new Color(1, 0, 0.3334f); break;
                    case Achievements_Data.Achievement.Type.BPS:
                        icon.color = new Color(0, 0.5f, 1); break;
                    case Achievements_Data.Achievement.Type.RESEARCH:
                        icon.color = new Color(0, 0.75f, 0.3f); break;
                    case Achievements_Data.Achievement.Type.SECRET:
                        icon.color = new Color(0.45f, 0.35f, 1); break;
                    default:
                        Debug.Log("Invalid index color bit assignment"); break;
                }

                title_obj = UI_Tool.TextSetup("Title", icon_obj.transform, out a_blocks[i].title, false);
                UI_Tool.FormatRect(a_blocks[i].title.rectTransform, new Vector2(800, 40), new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(30, 0));
                UI_Tool.FormatText(a_blocks[i].title, Database.arial, 32, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
                a_blocks[i].title.text = Achievements_Data.achievements[i].title;

                desc_obj = UI_Tool.TextSetup("Desc", title_obj.transform, out Text desc, false);
                UI_Tool.FormatRectNPos(desc.rectTransform, new Vector2(800, 80), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 1));
                UI_Tool.FormatText(desc, Database.arial, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
                desc.text = Achievements_Data.achievements[i].desc;

                value_obj = UI_Tool.TextSetup("Value", a_blocks[i].block_obj.transform, out a_blocks[i].value, false);
                UI_Tool.FormatRect(a_blocks[i].value.rectTransform, new Vector2(190, 40), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-15, -15));
                UI_Tool.FormatText(a_blocks[i].value, Database.arial, 32, Color.white, TextAnchor.UpperRight, FontStyle.Italic);
                a_blocks[i].value.text = $"{Achievements_Data.achievements[i].value}";

                date_obj = UI_Tool.TextSetup("Date", value_obj.transform, out a_blocks[i].date, false);
                UI_Tool.FormatRectNPos(a_blocks[i].date.rectTransform, new Vector2(190, 80), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 1));
                UI_Tool.FormatText(a_blocks[i].date, Database.arial, 28, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
                a_blocks[i].date.text = $"" +
                    $"{ (Achievements_Data.achievements[i].date_earned == DateTime.FromBinary(0) ? "" : Achievements_Data.achievements[i].date_earned.ToString()) }";

                progress_bar_obj = UI_Tool.ImgSetup("Progress Bar", a_blocks[i].block_obj.transform, out a_blocks[i].progress_bar, false);
                UI_Tool.FormatRect(a_blocks[i].progress_bar.rectTransform, new Vector2(1020, 40), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-15, 10));
                a_blocks[i].progress_bar.color = new Color(0.4f, 0.4f, 0.4f);

                bar_obj = UI_Tool.ImgSetup("Bar", progress_bar_obj.transform, out a_blocks[i].bar, false);
                UI_Tool.FormatRectNPos(a_blocks[i].bar.rectTransform, new Vector2(a_blocks[i].progress_bar.rectTransform.sizeDelta.x, 0),
                    new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f));
                a_blocks[i].bar.color = new Color(0.9f, 0.75f, 0);
                a_blocks[i].bar.rectTransform.localScale = new Vector2(Achievements_Data.achievements[i].status == 0 ?
                    1 : (float)(Achievements_Data.achievements[i].progress / Achievements_Data.achievements[i].max), 1);

                progress_txt_obj = UI_Tool.TextSetup("Value", progress_bar_obj.transform, out a_blocks[i].progress_txt, false);
                UI_Tool.FormatRect(a_blocks[i].progress_txt.rectTransform);
                UI_Tool.FormatText(a_blocks[i].progress_txt, Database.arial, 32, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
                a_blocks[i].progress_txt.text = $"{ Database.To_Bit_Notation(Achievements_Data.achievements[i].progress, "#,0.#") } / { Achievements_Data.achievements[i].max }";

                cover_obj = UI_Tool.ImgSetup("Cover", a_blocks[i].block_obj.transform, out a_blocks[i].cover, true);
                UI_Tool.FormatRect(a_blocks[i].cover.rectTransform);
                a_blocks[i].cover.color = new Color(0, 0, 0, 0.3f);
                a_blocks[i].cover.enabled = Achievements_Data.achievements[i].status == 0 ? false : true;
            }
        }
    }
}