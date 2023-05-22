using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsInterface : MonoBehaviour
{
    public class A_Block
    {
        public GameObject block_obj;
        public Image block, icon, progress_bar, cover, bar;
        public Text value, title, desc, date, progress_txt;
    }

    List<AchievementData> achievementsData = SysManager.profile.achievements.data;

    public GameObject achievement_interface;
    RectTransform a_list;
    A_Block[] a_blocks;
    
    void Awake()
    {
        achievement_interface = InterfaceTool.CanvasSetup("Achievements Canvas", gameObject.transform, out Canvas canvas);

        gameObject.SetActive(false);

        GameObject backdrop = InterfaceTool.ImgSetup("Backdrop", achievement_interface.transform, out Image backdrop_img, true);
        InterfaceTool.FormatRect(backdrop_img.rectTransform);
        backdrop_img.color = new Color(0, 0, 0, 0.3f);

        GameObject a_panel = InterfaceTool.ImgSetup("Panel", achievement_interface.transform, out Image panel_img, SysManager.defaultBox, false);
        panel_img.color = Color.white;
        InterfaceTool.FormatRectNPos(panel_img.rectTransform, new Vector2(1300, 900));

        GameObject a_content = InterfaceTool.ImgSetup("Contents", a_panel.transform, out Image content_img, false);
        content_img.color = new Color(0.29f, 0.27f, 0.3f);
        InterfaceTool.FormatRect(content_img.rectTransform, new Vector2(-90, -90), new Vector2(0, 0), new Vector2(1, 1), new Vector2(-15, -15));
        a_content.AddComponent<RectMask2D>();

        a_list = new GameObject("List").AddComponent<RectTransform>();
        a_list.transform.SetParent(a_content.transform, false);
        InterfaceTool.FormatRectNPos(a_list, new Vector2(0, 185 * Achievements_Data.achievements.Length), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));

        GameObject a_header = InterfaceTool.ImgSetup("Header", a_panel.transform, out Image header_img, SysManager.defaultBox, false);
        InterfaceTool.FormatRectNPos(header_img.rectTransform, new Vector2(760, 120), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f));
        GameObject a_header_txt_obj = InterfaceTool.TextSetup("Title", a_header.transform, out Text a_header_txt, false);
        InterfaceTool.FormatRect(a_header_txt.rectTransform);
        InterfaceTool.FormatText(a_header_txt, SysManager.DEFAULT_FONT, 48, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
        a_header_txt.text = "ACHIEVEMENTS";

        GameObject scrollbar_obj = InterfaceTool.ScrollbarSetup(a_panel.transform, a_content, a_list, 
            new Vector2(30, -90), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 0.5f), new Vector2(-30, -15));

        GameObject back_button_obj = InterfaceTool.ButtonSetup("Back Button", a_panel.transform, out Image back_img, out Button back_button, SysManager.defaultButton,
            () => Display_Achievements(false));
        InterfaceTool.FormatRect(back_img.rectTransform, new Vector2(70, 70),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(10, -10));
        GameObject back_button_txt_obj = InterfaceTool.TextSetup("B Button Text", back_button_obj.transform, out Text back_button_txt, false);
        InterfaceTool.FormatRect(back_button_txt.rectTransform);
        InterfaceTool.FormatText(back_button_txt, SysManager.DEFAULT_FONT, 36, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
        back_button_txt.text = "X";

        CreateAchievementBlocks();
    }

    public void Display_Achievements(bool set_active)
    {
        if (set_active)
            UpdateListing();
        gameObject.SetActive(set_active);

        InterfaceTool.ToggleCanvasPriority(GameObject.FindWithTag("Main"), achievement_interface.GetComponent<Canvas>());
    }

    void CreateAchievementBlocks()
    {
        a_blocks = new A_Block[achievementsData.Count];

        for (int i = 0; i < a_blocks.Length; i++)
        {
            a_blocks[i] = new A_Block();

            GameObject icon_obj, value_obj, progress_bar_obj, cover_obj;
            GameObject title_obj, desc_obj, date_obj, bar_obj, progress_txt_obj;

            a_blocks[i].block_obj = InterfaceTool.ImgSetup($"Achievement #{i}", a_list, out a_blocks[i].block, SysManager.defaultBox, false);
            a_blocks[i].block.color = new Color(0.75f, 0.72f, 0.8f);
            InterfaceTool.FormatRect(a_blocks[i].block.rectTransform, new Vector2(0, 185), new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0.5f, 1), new Vector2(0, -185 * i));

            icon_obj = InterfaceTool.ImgSetup("Icon", a_blocks[i].block_obj.transform, out Image icon, false);
            InterfaceTool.FormatRect(icon.rectTransform, new Vector2(100, 100), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(40, -40));

            switch (achievementsData[i].Type)
            {
                case AchievementType.STORY:
                    icon.color = new Color(1, 1, 1); break;
                case AchievementType.CP:
                    icon.color = new Color(1, 0, 0.3334f); break;
                case AchievementType.BPS:
                    icon.color = new Color(0, 0.5f, 1); break;
                case AchievementType.RESEARCH:
                    icon.color = new Color(0, 0.75f, 0.3f); break;
                case AchievementType.SECRET:
                    icon.color = new Color(0.45f, 0.35f, 1); break;
                default:
                    Debug.Log("Invalid index color bit assignment"); break;
            }

            title_obj = InterfaceTool.TextSetup("Title", icon_obj.transform, out a_blocks[i].title, false);
            InterfaceTool.FormatRect(a_blocks[i].title.rectTransform, new Vector2(800, 40), new Vector2(1, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(30, 0));
            InterfaceTool.FormatText(a_blocks[i].title, SysManager.DEFAULT_FONT, 32, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
            a_blocks[i].title.text = achievementsData[i].Title;

            desc_obj = InterfaceTool.TextSetup("Desc", title_obj.transform, out Text desc, false);
            InterfaceTool.FormatRectNPos(desc.rectTransform, new Vector2(800, 80), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 1));
            InterfaceTool.FormatText(desc, SysManager.DEFAULT_FONT, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
            desc.text = achievementsData[i].Desc;

            value_obj = InterfaceTool.TextSetup("Value", a_blocks[i].block_obj.transform, out a_blocks[i].value, false);
            InterfaceTool.FormatRect(a_blocks[i].value.rectTransform, new Vector2(190, 40), new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-15, -15));
            InterfaceTool.FormatText(a_blocks[i].value, SysManager.DEFAULT_FONT, 32, Color.white, TextAnchor.UpperRight, FontStyle.Italic);
            a_blocks[i].value.text = $"{achievementsData[i].BitAmount}";

            date_obj = InterfaceTool.TextSetup("Date", value_obj.transform, out a_blocks[i].date, false);
            InterfaceTool.FormatRectNPos(a_blocks[i].date.rectTransform, new Vector2(190, 80), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 1));
            InterfaceTool.FormatText(a_blocks[i].date, SysManager.DEFAULT_FONT, 28, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
            a_blocks[i].date.text = $"" +
                $"{ (achievementsData[i].DateEarned == DateTime.FromBinary(0) ? "" : achievementsData[i].DateEarned.ToString()) }";

            progress_bar_obj = InterfaceTool.ImgSetup("Progress Bar", a_blocks[i].block_obj.transform, out a_blocks[i].progress_bar, false);
            InterfaceTool.FormatRect(a_blocks[i].progress_bar.rectTransform, new Vector2(1020, 40), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-15, 10));
            a_blocks[i].progress_bar.color = new Color(0.4f, 0.4f, 0.4f);

            bar_obj = InterfaceTool.ImgSetup("Bar", progress_bar_obj.transform, out a_blocks[i].bar, false);
            InterfaceTool.FormatRectNPos(a_blocks[i].bar.rectTransform, new Vector2(a_blocks[i].progress_bar.rectTransform.sizeDelta.x, 0),
                new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f));
            a_blocks[i].bar.color = new Color(0.9f, 0.75f, 0);
            a_blocks[i].bar.rectTransform.localScale = new Vector2(achievementsData[i].Status == 0 ?
                1 : (float)(achievementsData[i].Progress / achievementsData[i].Max), 1);

            progress_txt_obj = InterfaceTool.TextSetup("Value", progress_bar_obj.transform, out a_blocks[i].progress_txt, false);
            InterfaceTool.FormatRect(a_blocks[i].progress_txt.rectTransform);
            InterfaceTool.FormatText(a_blocks[i].progress_txt, SysManager.DEFAULT_FONT, 32, Color.black, TextAnchor.MiddleCenter, FontStyle.Normal);
            a_blocks[i].progress_txt.text = $"{ BitNotation.ToBitNotation(achievementsData[i].Progress, "#,0.#") } / { achievementsData[i].Max }";

            cover_obj = InterfaceTool.ImgSetup("Cover", a_blocks[i].block_obj.transform, out a_blocks[i].cover, true);
            InterfaceTool.FormatRect(a_blocks[i].cover.rectTransform);
            a_blocks[i].cover.color = new Color(0, 0, 0, 0.3f);
            a_blocks[i].cover.enabled = achievementsData[i].Status == 0 ? false : true;
        }
    }

    public void UpdateListing()
    {
        SysManager.profile.achievements.CheckAchievements();

        for (int i = 0; i < a_blocks.Length; i++)
        {
            a_blocks[i].cover.enabled = achievementsData[i].Status == AchievementStatus.UNLOCKED ? false : true;
            a_blocks[i].progress_txt.text = $"{ BitNotation.ToBitNotation(achievementsData[i].Progress, "#,0.#") } / { achievementsData[i].Max }";
            a_blocks[i].bar.rectTransform.localScale = new Vector2(achievementsData[i].Status == 0 ?
                1 : (float)(achievementsData[i].Progress / achievementsData[i].Max), 1);
        }
    }
}