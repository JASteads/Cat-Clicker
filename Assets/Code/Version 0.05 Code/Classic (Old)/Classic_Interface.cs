using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static Database;

public class Building_Button
{
    public int id;
    public Image image;
    public Button button;
    public Text name, price, count;
    public bool active;
}

public class Upgrade_Button
{
    public int id;
    public RectTransform rect_tf;
    public Button button;
    public Text title, price;
    public Image button_image, icon;
}

public class Upgrade_Tab
{
    public int id;
    public Button button;
    public Text text;
    public Image image;
}

public class Fever_Meter
{
    public RectTransform tf;
    public Image fever_bar;
}

public class Status_Msg
{
    public int timer;
    public Text message;
    public Type msg_type;
    public enum Type
    {
        News = 0,
        Warning = 1,
        Bonus = 2
    }

    public Status_Msg(string name, string msg, Type type, Font font, int font_size)
    {
        timer = 1;
        msg_type = type;
        GameObject msg_obj = new GameObject(name);
        message = msg_obj.AddComponent<Text>();

        message.font = font;
        message.fontSize = font_size;
        message.color = new Color(1, 1, 1);
        message.fontStyle = FontStyle.Bold;
        message.alignment = TextAnchor.MiddleCenter;
        message.text = msg;
    }
}

public class Tooltip
{
    public int id;
    public RectTransform tf, icon_tf, title_tf, price_tf, desc_tf, extra_1_tf, extra_2_tf;
    public Image icon;
    public Text title, desc, price, count, value, extra_1, extra_2;

    public enum Display_Format
    {
        Building = 1,
        Upgrade = 2,
        Fever = 3
    };
    public Display_Format format;
}

public class Classic_Interface : MonoBehaviour
{
    Classic_System system; // This is where we grab values for the game instance

    GameObject buildings_canvas, upgrades_canvas, bits_canvas, messages_canvas, fever_canvas, info_canvas;
    Text bit_counter, bps_counter, fever_text;
    Button click_button, opt_button;
    GameObject building_shop;
    
    Fever_Meter fever_meter, fever_meter_secondary;

    RectTransform building_list;
    Building_Button[] building_buttons = new Building_Button[12];
    Image b_shop_bg;
    Button up_toggle_button, seek_button_l, seek_button_r;
    int building_shop_len;

    GameObject upgrades_shop;
    Upgrade_Button[] upgrade_buttons;
    Upgrade_Tab[] upgrade_tabs = new Upgrade_Tab[4];
    public int active_tab = -1;
    int[] active_upgrades;
    int active_index; // Index for active_upgrades array

    public Status_Msg[] status_msgs = new Status_Msg[5];
    public Status_Msg[] click_popups = new Status_Msg[10];
    GameObject status_messages_list, click_popups_list;

    Vector3 mouse_pos;
    Tooltip tooltip;

    RectTransform options_obj;
    Button[] options = new Button[4];

    int update_tick = 0;

    Color pure_black = new Color(0, 0, 0);

    void Init_Bits()
    {
        GameObject bit_counter_obj, bps_obj, click_obj, click_text_obj, opt_obj;

        bit_counter_obj = UI_Tool.TextSetup("Bit Counter", bits_canvas.transform, out bit_counter, false);
        UI_Tool.FormatRect(bit_counter.rectTransform, new Vector2(1300, 60),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -160));
        UI_Tool.FormatText(bit_counter, arial, 42, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);

        bps_obj = UI_Tool.TextSetup("BPS Counter", bit_counter_obj.transform, out bps_counter, false);
        UI_Tool.FormatRect(bps_counter.rectTransform, new Vector2(800, 40),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -bit_counter.rectTransform.rect.height));
        UI_Tool.FormatText(bps_counter, arial, 32, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);

        click_obj = UI_Tool.ButtonSetup("Click Button", bits_canvas.transform, out Image click_img, out click_button, ui_sprites[3], () => system.Click());
        UI_Tool.FormatRect(click_img.rectTransform, new Vector2(270, 90),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(180, -300));

        click_text_obj = UI_Tool.TextSetup("Click Text", click_obj.transform, out Text click_text, false);
        UI_Tool.FormatRectNPos(click_text.rectTransform, new Vector2(0, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));
        UI_Tool.FormatText(click_text, arial, 42, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        click_text.text = "CLICK";

        opt_obj = UI_Tool.ButtonSetup("Options Button", bits_canvas.transform, out Image opt_img, out opt_button, ui_sprites[5], () => Toggle_Options());
        UI_Tool.FormatRect(opt_img.rectTransform, new Vector2(80, 80),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(15, -15));
        opt_img.type = Image.Type.Simple;
    }
    
    // BUILDING SHOP
    
    void Init_Building_Shop()
    {
        GameObject panel_l, panel_r, upgrade_toggle, seek_l, seek_r;
        GameObject up_toggle_txt_container, seek_txt_container_l, seek_txt_container_r;

        const int button_height = 150;

        // Length of shop panels (340 * 2), plus the width of each building and respective spacing,
        // minus the spacing for the first and last button.
        building_shop_len = 680 + (data.b_data.Length * 210) - 20;

        building_shop = UI_Tool.ImgSetup("Building Shop", buildings_canvas.transform, out b_shop_bg, ui_sprites[2], true);
        UI_Tool.FormatRect(b_shop_bg.rectTransform, new Vector2(0, 200),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 0));
        b_shop_bg.color = new Color(0.6f, 0.6f, 0.6f);

        building_list = new GameObject("List").AddComponent<RectTransform>();
        building_list.SetParent(building_shop.transform, false);
        UI_Tool.FormatRect(building_list, new Vector2(620, 0),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 0.5f), new Vector2(310, 0));

        panel_l = UI_Tool.ImgSetup("Building Panel Left", buildings_canvas.transform, out Image pan_img_l, default_box, true);
        UI_Tool.FormatRect(pan_img_l.rectTransform, new Vector2(310, 200),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
        pan_img_l.color = new Color(0.8f, 0.8f, 0.8f);

        panel_r = UI_Tool.ImgSetup("Building Panel Right", buildings_canvas.transform, out Image pan_img_r, default_box, true);
        UI_Tool.FormatRect(pan_img_r.rectTransform, new Vector2(310, 200),
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0, 0));
        pan_img_r.color = new Color(0.8f, 0.8f, 0.8f);

        upgrade_toggle = UI_Tool.ButtonSetup("Upgrades Button", panel_r.transform, out Image up_toggle_img, out up_toggle_button, ui_sprites[3], () => Toggle_Upgrades_Shop(up_toggle_button.gameObject));
        UI_Tool.FormatRect(up_toggle_img.rectTransform, new Vector2(140, button_height),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-35, 0));

        seek_l = UI_Tool.ButtonSetup("Seek Left", panel_l.transform, out Image seek_img_l, out seek_button_l, ui_sprites[3], () => Seek_Shop(building_list, false));
        UI_Tool.FormatRect(seek_img_l.rectTransform, new Vector2(70, button_height),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-35, 0));
        ColorBlock seek_colors_l = seek_button_l.colors;
        seek_img_l.color = Color.white;
        seek_colors_l.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1);
        seek_button_l.colors = seek_colors_l;

        seek_txt_container_l = UI_Tool.TextSetup("Seek Button Text Left", seek_l.transform, out Text seek_txt_l, false);
        UI_Tool.FormatRect(seek_txt_l.rectTransform);
        UI_Tool.FormatText(seek_txt_l, arial, 60, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        seek_txt_l.text = "<";
        seek_txt_l.alignByGeometry = true;

        seek_r = UI_Tool.ButtonSetup("Seek Right", panel_r.transform, out Image seek_img_r, out seek_button_r, ui_sprites[3], () => Seek_Shop(building_list, true));
        UI_Tool.FormatRect(seek_img_r.rectTransform, new Vector2(70, button_height),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(35, 0));
        ColorBlock seek_colors_r = seek_button_r.colors;
        seek_img_r.color = Color.white;
        seek_colors_r.disabledColor = new Color(0.5f, 0.5f, 0.5f, 1);
        seek_button_r.colors = seek_colors_r;

        seek_txt_container_r = UI_Tool.TextSetup("Seek Button Text Right", seek_r.transform, out Text seek_txt_r, false);
        UI_Tool.FormatRect(seek_txt_r.rectTransform);
        UI_Tool.FormatText(seek_txt_r, arial, 60, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        seek_txt_r.text = ">";
        seek_txt_r.alignByGeometry = true;

        up_toggle_txt_container = UI_Tool.TextSetup("Upgrades Button Text", upgrade_toggle.transform, out Text up_toggle_txt, false);
        UI_Tool.FormatRect(up_toggle_txt.rectTransform);
        UI_Tool.FormatText(up_toggle_txt, arial, 24, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        up_toggle_txt.text = "UPGRADES";
        
        for (int i = 0; i < system.buildings_system.buildings.Length; i++)
        {
            building_buttons[i] = Generate_Shop_Button(i, building_list, new Vector2((i * 210) + 15, 0));

            building_buttons[i].name.text = $"{system.buildings_system.buildings[i].name}";
            building_buttons[i].price.text = $"$ {To_Bit_Notation(data.b_data[i].price, "#,0")}";
            building_buttons[i].count.text = $"{To_Bit_Notation(data.b_data[i].count, "#,0")}";

            building_buttons[i].button.gameObject.SetActive(false);
        }

        building_buttons[0].active = true;
        building_buttons[0].button.gameObject.SetActive(true); // First button should be active

        Update_Buildings_List();
        Update_Seek_Buttons(building_list);
    }
    void Update_Buildings_List()
    {
        int new_len = -Screen.width;
        foreach (Building_Button b in building_buttons)
            if (b.active) new_len += 210;
        building_list.sizeDelta = new Vector2(new_len + 20, 0);
    }
    Building_Button Generate_Shop_Button(int id_num, RectTransform tf, Vector2 pos)
    {
        GameObject building, name, price, count;

        Building_Button building_button = new Building_Button { id = id_num };

        building = UI_Tool.ButtonSetup($"Building #{id_num}", tf,
            out building_button.image, out building_button.button, ui_sprites[3], () => system.Purchase_Building(building_button));
        UI_Tool.FormatRect(building_button.image.rectTransform, new Vector2(200, 150),
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), pos);
        Assign_Tooltip(building, id_num);

        name = UI_Tool.TextSetup("Name", building_button.button.transform, out building_button.name, false);
        UI_Tool.FormatRectNPos(building_button.name.rectTransform, new Vector2(0, 50),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
        UI_Tool.FormatText(building_button.name, arial, 20, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        building_button.name.text = system.buildings_system.buildings[id_num].name;

        count = UI_Tool.TextSetup("Count", building_button.button.transform, out building_button.count, false);
        UI_Tool.FormatRect(building_button.count.rectTransform, new Vector2(-30, 50),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -20));
        UI_Tool.FormatText(building_button.count, arial, 16, pure_black, TextAnchor.MiddleLeft, FontStyle.Normal);
        building_button.count.text = "0";

        price = UI_Tool.TextSetup("Price", name.transform, out building_button.price, false);
        UI_Tool.FormatRectNPos(building_button.price.rectTransform, new Vector2(-30, 20),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 0));
        UI_Tool.FormatText(building_button.price, arial, 16, pure_black, TextAnchor.MiddleRight, FontStyle.Normal);
        building_button.price.text = "$ 0";

        return building_button;
    }
    public void Update_Seek_Buttons(RectTransform tf)
    {
        int new_pos = (int)tf.anchoredPosition.x;
        
        seek_button_l.interactable = new_pos != 310;
        seek_button_r.interactable = new_pos >= -(310 + tf.sizeDelta.x);
    }
    public void Seek_Shop(RectTransform tf, bool forward)
    {
        tf.localPosition = 
            new Vector2(tf.localPosition.x + (forward ? -210 : 210), tf.localPosition.y);
        Update_Seek_Buttons(tf);
    }
    
    // UPGRADES SHOP
    
    void Init_Upgrades_Shop()
    {
        GameObject scrollbar, title_box, title_text_container, upgrades_container, upgrades_list, tabs_container;
        
        RectTransform list_tf;
        active_upgrades = new int[system.upgrades_system.upgrades.Length];

        upgrades_shop = new GameObject("Upgrades");
        upgrades_shop.AddComponent<RectTransform>();
        upgrades_shop.transform.SetParent(upgrades_canvas.transform, false);
        UI_Tool.FormatRect(upgrades_shop.GetComponent<RectTransform>(), new Vector2(460, 560),
            new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 30));

        title_box = UI_Tool.ImgSetup("Upgrades Title", upgrades_shop.transform, out Image title_img, default_box, false);
        UI_Tool.FormatRect(title_img.rectTransform, new Vector2(0, 120),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, 30));
        title_img.color = new Color(.8f, .8f, 1);

        title_text_container = UI_Tool.TextSetup("Upgrade Title Text", title_box.transform, out Text title_text, false);
        UI_Tool.FormatRect(title_text.rectTransform, new Vector2(-30, 90),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(-15, 0));
        UI_Tool.FormatText(title_text, arial, 42, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        title_text.text = "UPGRADES";

        upgrades_container = UI_Tool.ImgSetup("Upgrades Container", upgrades_shop.transform, out Image upgrades_container_img, default_box, true);
        upgrades_container.AddComponent<RectMask2D>();
        UI_Tool.FormatRect(upgrades_container_img.rectTransform);
        upgrades_container_img.color = new Color(0.6f, 0.6f, 1);

        upgrades_list = new GameObject("Upgrades List");
        upgrades_list.transform.SetParent(upgrades_container.transform, false);
        list_tf = upgrades_list.AddComponent<RectTransform>();
        UI_Tool.FormatRect(list_tf, new Vector2(-30, 0), 
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -15));
        
        tabs_container = new GameObject("Tabs Containter");
        tabs_container.transform.SetParent(upgrades_shop.transform, false);
        tabs_container.AddComponent<RectTransform>();
        UI_Tool.FormatRectNPos(tabs_container.GetComponent<RectTransform>(), new Vector2(50, 0),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0.5f));

        scrollbar = UI_Tool.ScrollbarSetup(upgrades_shop.transform, upgrades_container, list_tf, 30);

        upgrade_buttons = new Upgrade_Button[system.upgrades_system.upgrades.Length];

        for (int i = 0; i < 4; i++)
            upgrade_tabs[i] = Generate_Upgrade_Tab(i, new Vector2(0, 180 - (i * 120)));

        for (int i = 0; i < system.upgrades_system.upgrades.Length; i++)
            upgrade_buttons[i] = Generate_Upgrade_Button(i, new Vector2(0, -80 * i));

        Toggle_Upgrades_Shop(up_toggle_button.gameObject);
    }
    
    Upgrade_Button Generate_Upgrade_Button(int id_num, Vector2 pos)
    {
        GameObject upgrade, icon, title, price;
        Upgrade_Button upgrade_button = new Upgrade_Button { id = id_num };

        upgrade = UI_Tool.ButtonSetup($"Upgrade #{upgrade_button.id}", upgrades_shop.transform.GetChild(1).GetChild(0),
            out upgrade_button.button_image, out upgrade_button.button, ui_sprites[3], () => system.Purchase_Upgrade(upgrade_button));
        UI_Tool.FormatRect(upgrade_button.button_image.rectTransform, new Vector2(-30, 80),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -15));
        upgrade_button.button_image.color = new Color(1, 1, .8f);

        upgrade_button.rect_tf = upgrade_button.button_image.rectTransform;

        ColorBlock button_colors = upgrade_button.button.colors;
        button_colors.disabledColor = new Color(0.5f, 0.5f, 0.4f);
        button_colors.highlightedColor = new Color(.7f, .7f, .7f);
        button_colors.pressedColor = new Color(.4f, .4f, .4f);
        upgrade_button.button.colors = button_colors;

        icon = UI_Tool.ImgSetup("Upgrade Icon", upgrade_button.rect_tf, out upgrade_button.icon, false);
        UI_Tool.FormatRect(upgrade_button.icon.rectTransform, new Vector2(60, 60),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(17, -10));
        upgrade_button.icon.color = new Color(0, .1f, 1);

        title = UI_Tool.TextSetup("Upgrade Title", upgrade_button.rect_tf, out upgrade_button.title, false);
        UI_Tool.FormatRect(upgrade_button.title.rectTransform, new Vector2(340, 30),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(90, -25));
        UI_Tool.FormatText(upgrade_button.title, arial, 16, pure_black, TextAnchor.MiddleLeft, FontStyle.Bold);
        upgrade_button.title.text = system.upgrades_system.upgrades[id_num].title;
        upgrade_button.title.alignByGeometry = true;
        upgrade_button.title.resizeTextForBestFit = true;
        upgrade_button.title.resizeTextMaxSize = 30;

        price = UI_Tool.TextSetup("Upgrade Price", upgrade_button.rect_tf, out upgrade_button.price, false);
        UI_Tool.FormatRect(upgrade_button.price.rectTransform, new Vector2(250, 20),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -7.5f));
        UI_Tool.FormatText(upgrade_button.price, arial, 16, pure_black, TextAnchor.MiddleRight, FontStyle.Italic);
        upgrade_button.price.text = $"$ {system.upgrades_system.upgrades[id_num].price.ToString("0.##")}";
        upgrade_button.price.alignByGeometry = true;
        
        Assign_Tooltip(upgrade, upgrade_button.id + 100);

        return upgrade_button;
    }

    Upgrade_Tab Generate_Upgrade_Tab(int tab_num, Vector2 pos)
    {
        GameObject tab, text;
        Upgrade_Tab upgrade_tab = new Upgrade_Tab { id = tab_num };

        tab = UI_Tool.ButtonSetup($"Tab #{tab_num}", upgrades_shop.transform.GetChild(2), out upgrade_tab.image, out upgrade_tab.button, ui_sprites[3], () => Refresh_Upgrades(tab_num, true));
        upgrade_tab.image.rectTransform.sizeDelta = new Vector2(50, 100);
        upgrade_tab.image.rectTransform.anchoredPosition = pos;
        upgrade_tab.image.color = Color.white;

        text = UI_Tool.TextSetup("Tab Text", upgrade_tab.button.transform, out upgrade_tab.text, false);
        upgrade_tab.text.rectTransform.sizeDelta = new Vector2(50, 150);
        UI_Tool.FormatText(upgrade_tab.text, arial, 16, pure_black, TextAnchor.MiddleCenter, FontStyle.Normal);
        upgrade_tab.text.text = $"{tab_num + 1}";

        return upgrade_tab;
    }

    // CAUSE OF UPGRADE DISPLAY BUG. active_upgrades does not accurately represent upgrade index; use index array instead
    public void Refresh_Upgrades(int tab_id, bool reset_tab)
    {
        // Sets which tab to display
        if (reset_tab)
        {
            if (active_tab != -1)
            {
                upgrade_tabs[active_tab].button.transition = Selectable.Transition.ColorTint;
                upgrade_tabs[active_tab].image.color = Color.white;
            }
            if (tab_id == active_tab) tab_id = -1;
            else
            {
                upgrade_tabs[tab_id].button.transition = Selectable.Transition.None;
                upgrade_tabs[tab_id].image.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }

        float button_pos = 0;
        
        // Turn off all inactive upgrade buttons
        for (int i = 0; i < upgrade_buttons.Length; i++)
            upgrade_buttons[i].rect_tf.gameObject.SetActive(false);


        active_index = 0;   // Reset to overwrite previous active_upgrades values.

        // Display available upgrades_canvas
        for (int i = 0; i < active_upgrades.Length; i++)
            if ((tab_id == -1 || (int)system.upgrades_system.upgrades[i].type == tab_id) && system.upgrades_system.upgrades[i].status == 1)
            {
                // Debug.Log($"{ data.upgrades[i].title } is enabled.");
                // Debug.Log($"Status : { data.upgrades[i].status }");

                upgrade_buttons[i].rect_tf.gameObject.SetActive(true);
                upgrade_buttons[i].rect_tf.anchoredPosition = new Vector2(0, button_pos - 15);
                button_pos -= upgrade_buttons[i].rect_tf.rect.height;
                active_upgrades[active_index++] = upgrade_buttons[i].id;
            }

        active_upgrades[active_index++] = -1;   // Set final index to -1. This will be our stop value. 
                                                // Increment again for accurate list length

        active_tab = tab_id;

        // Resize list so it can be scrolled through
        upgrades_shop.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 80 * active_index);
    }

    public void Toggle_Upgrades_Shop(GameObject upgrade_button)
    {
        // Use the tab section as an indicator to whether the upgrade shop is open or not.
        // Could be any of the tabs, but we can use them to access the parent object and turn off.
        GameObject tabs_section = upgrade_tabs[0].button.transform.parent.gameObject;
        RectTransform shop_tf = upgrades_shop.GetComponent<RectTransform>();

        if (!tabs_section.activeSelf)
        {
            shop_tf.pivot = new Vector2(1, 0.5f);
            shop_tf.anchoredPosition = new Vector2(-30, 30);
            tabs_section.SetActive(true);
            Refresh_Upgrades(active_tab, false);
            upgrade_button.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
            upgrade_button.GetComponent<Button>().transition = Selectable.Transition.None;
        }
        else
        {
            shop_tf.pivot = new Vector2(0, 0.5f);
            shop_tf.anchoredPosition = new Vector2(0, 30);
            tabs_section.SetActive(false); 
            upgrade_button.GetComponent<Image>().color = Color.white;
            upgrade_button.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        }
    }
    
    // FEVER

    void Init_Fever_System()
    {
        GameObject fever_text_obj, fever_bar, inside, indicator, indicator2;
        fever_meter = new Fever_Meter();
        fever_meter_secondary = new Fever_Meter();

        fever_bar = new GameObject("Fever Meter");
        fever_bar.transform.SetParent(fever_canvas.transform, false);
        fever_bar.transform.rotation = Quaternion.Euler(0, 0, 270);
        Image bar_img = fever_bar.AddComponent<Image>();
        UI_Tool.FormatRect(bar_img.rectTransform, new Vector2(100, 250),
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(125, 250));
        bar_img.color = new Color(0.28f, 0.24f, 0.3f);
        
        inside = new GameObject("Fever Inside");
        inside.transform.SetParent(fever_bar.transform, false);
        inside.AddComponent<Image>().color = new Color(0.4f, 0.36f, 0.4f);
        UI_Tool.FormatRectNPos(inside.GetComponent<RectTransform>(), new Vector2(-30, -30),
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f));

        Assign_Tooltip(inside, -1);

        indicator = new GameObject("Fever Bar");
        fever_meter.fever_bar = indicator.AddComponent<Image>();
        fever_meter.fever_bar.color = new Color(0.9f, 0.7f, 0);
        fever_meter.tf = fever_meter.fever_bar.rectTransform;

        indicator2 = new GameObject("Secondary Fever Bar");
        fever_meter_secondary.fever_bar = indicator2.AddComponent<Image>();
        fever_meter_secondary.fever_bar.color = new Color(0.9f, 0.6f, 0);
        fever_meter_secondary.tf = fever_meter_secondary.fever_bar.rectTransform;

        fever_meter.tf.SetParent(inside.transform, false);
        UI_Tool.FormatRectNPos(fever_meter.tf, new Vector2(70, 220),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        fever_meter.fever_bar.raycastTarget = false;

        fever_meter_secondary.tf.SetParent(inside.transform, false);
        UI_Tool.FormatRectNPos(fever_meter_secondary.tf, new Vector2(70, 220),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        fever_meter_secondary.fever_bar.raycastTarget = false;

        fever_text_obj = new GameObject("FEVER Text");
        fever_text = fever_text_obj.AddComponent<Text>();
        fever_text.rectTransform.SetParent(fever_canvas.transform, false);
        fever_text.rectTransform.localPosition = new Vector2(0, 250);
        fever_text.rectTransform.sizeDelta = new Vector2(500, 100);
        fever_text.raycastTarget = false;

        UI_Tool.FormatText(fever_text, arial, 64, new Color(0.9f, 0.7f, 0), TextAnchor.MiddleCenter, FontStyle.Bold);
        fever_text.text = "FEVER";
    }
    
    
    // STATUS MESSAGES

    void Init_Status_Messages(GameObject list_obj, Status_Msg[] msg_list, string title, int msg_type, int font_size)
    {
        list_obj.AddComponent<RectTransform>();
        
        list_obj.transform.SetParent(messages_canvas.transform, false);
        list_obj.transform.localPosition = new Vector2();
        list_obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        
        for (int i = 0; i < msg_list.Length; i++)
        {
            msg_list[i] = new Status_Msg($"{title} #{i}", "", Status_Msg.Type.News, arial, font_size);
            msg_list[i].message.transform.SetParent(list_obj.transform, false);
            
            // Game Updates
            if (msg_type == 0)
            {
                msg_list[i].message.transform.localPosition = new Vector2(-100, -200 + (50 * i));
                msg_list[i].message.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 42);
            }
            // Click Updates
            if (msg_type == 1)
            {
                msg_list[i].message.transform.localPosition = new Vector2(-650, 250 + (10 * i));
                msg_list[i].message.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 42);
            }
            msg_list[i].message.raycastTarget = false;
        }
    }

    public void Status_Update(Status_Msg[] msg_list, string msg, Status_Msg.Type type)
    {
        for (int i = msg_list.Length - 1; i > 0; i--)
        {
            msg_list[i].timer = msg_list[i - 1].timer;
            msg_list[i].msg_type = msg_list[i - 1].msg_type;
            msg_list[i].message.text = msg_list[i - 1].message.text;
        }
        msg_list[0].msg_type = type;
        msg_list[0].message.text = msg;
        msg_list[0].timer = 1;

        foreach (Status_Msg status in msg_list)
            switch (status.msg_type)
            {
                case Status_Msg.Type.News:
                    status.message.color = Color.white;
                    break;
                case Status_Msg.Type.Warning:
                    status.message.color = Color.red;
                    break;
                case Status_Msg.Type.Bonus:
                    status.message.color = Color.yellow;
                    break;
            }
    }
    
    void Status_Update_Clear(Status_Msg status)
    {
        status.message.text = "";
        status.timer = 0;
    }

    // TOOLTIPS

    void Assign_Tooltip(GameObject obj, int id)
    {
        EventTrigger trigger = obj.AddComponent<EventTrigger>();
        EventTrigger.Entry enter_event = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        EventTrigger.Entry exit_event = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        enter_event.callback.AddListener((data) => { Show_Tooltip_New(id); });
        exit_event.callback.AddListener((data) => { Hide_Tooltip(); });
        trigger.triggers.Add(enter_event);
        trigger.triggers.Add(exit_event);
    }
    
    public void Show_Tooltip_New(int id)
    {
        tooltip.id = id;
        tooltip.tf.gameObject.SetActive(true);

        if (id == -1)
            if (tooltip.format != Tooltip.Display_Format.Fever) Format_Tooltip(Tooltip.Display_Format.Fever);
        if (id >= 0 && id < 100)
        {
            if (tooltip.format != Tooltip.Display_Format.Building) Format_Tooltip(Tooltip.Display_Format.Building);

            UI_Tool.FormatRect(tooltip.tf, tooltip.tf.sizeDelta,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-340, 220));

            tooltip.title.text = building_buttons[id].name.text;
            tooltip.price.text = building_buttons[id].price.text;
            tooltip.desc.text = system.buildings_system.buildings[id].desc;
            tooltip.extra_1.text = $"BPS   |   {To_Bit_Notation(data.b_data[id].bps, "#,0.#")}";
            tooltip.extra_2.text = $"Value   |   {To_Bit_Notation(data.b_data[id].value, "#,0.#")}";
        }
        if (id >= 100)
        {
            int upgrade_pos = upgrade_buttons[id - 100].id;

            if (tooltip.format != Tooltip.Display_Format.Upgrade) Format_Tooltip(Tooltip.Display_Format.Upgrade);

            tooltip.title.text = system.upgrades_system.upgrades[upgrade_pos].title;
            tooltip.desc.text = system.upgrades_system.upgrades[upgrade_pos].desc;
            tooltip.price.text = $"$ {system.upgrades_system.upgrades[upgrade_pos].price.ToString()}";
            switch (system.upgrades_system.upgrades[upgrade_pos].type)
            {
                case Upgrades_System.Upgrade.Type.CP:
                    tooltip.extra_1.text = "[CP]";
                    tooltip.extra_2.text = $"{data.click_power}";
                    break;
                case Upgrades_System.Upgrade.Type.BUILDING:
                    tooltip.extra_1.text = "[Building]";
                    tooltip.extra_2.text = ""; // $"{Database.buildings_canvas[data.upgrades[upgrade_pos].target_building].name}";
                    break;
                case Upgrades_System.Upgrade.Type.CPS:
                    tooltip.extra_1.text = "[BPS]";
                    tooltip.extra_2.text = $"{data.bps.ToString()}";
                    break;
                case Upgrades_System.Upgrade.Type.SPECIAL:
                    tooltip.extra_1.text = "[Misc]";
                    tooltip.extra_2.text = "Something special may happen ...";
                    break;
            }
        }
    }
    public void Hide_Tooltip()
    {
        tooltip.tf.gameObject.SetActive(false);
    }
    void Update_Tooltip()
    {
        if (tooltip.id == -1) { }
        if (tooltip.id < 100 && tooltip.id >= 0)
        {
            tooltip.price.text = building_buttons[tooltip.id].price.text;
            tooltip.extra_1.text = $"BPS   |   {To_Bit_Notation(data.b_data[tooltip.id].bps * data.b_data[tooltip.id].count, "#,0.#")}";
            tooltip.extra_2.text = $"Value   |   {To_Bit_Notation(data.b_data[tooltip.id].value, "#,0.#")}";
        }
        if (tooltip.id >= 100)
        {
            tooltip.tf.position = main_cam.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, main_cam.nearClipPlane));

            tooltip.tf.localPosition = new Vector2(tooltip.tf.localPosition.x - 235, tooltip.tf.localPosition.y + 10);
            if (tooltip.tf.position.x >= Screen.width - (tooltip.tf.rect.width * 0.5f) - 20) tooltip.tf.position = new Vector2(Screen.width, tooltip.tf.position.y);
        }
    }
    void Format_Tooltip(Tooltip.Display_Format format)
    {
        tooltip.format = format;

        switch (format)
        {
            case Tooltip.Display_Format.Building:
            {
                UI_Tool.FormatRectNPos(tooltip.tf, new Vector2(550, 250),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
                UI_Tool.FormatRect(tooltip.icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -30));
                UI_Tool.FormatRect(tooltip.title_tf, new Vector2(165, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -30));
                UI_Tool.FormatRect(tooltip.desc_tf, new Vector2(490, 80),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(30, -145));
                UI_Tool.FormatRect(tooltip.price_tf, new Vector2(230, 60),
                    new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-30, -30));
                UI_Tool.FormatRect(tooltip.extra_1_tf, new Vector2(170, 40),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(30, 25));
                UI_Tool.FormatRect(tooltip.extra_2_tf, new Vector2(170, 40),
                    new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-30, 25));

                // Text Formatting

                UI_Tool.FormatText(tooltip.title, arial, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Bold);
                UI_Tool.FormatText(tooltip.price, arial, 24, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.desc, arial, 24, Color.white, TextAnchor.UpperLeft, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.extra_1, arial, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.extra_2, arial, 14, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                break;
            }
            case Tooltip.Display_Format.Upgrade:
            {
                UI_Tool.FormatRect(tooltip.tf, new Vector2(550, 230),
                    new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(-670, 310));
                UI_Tool.FormatRect(tooltip.icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -30));
                UI_Tool.FormatRect(tooltip.title_tf, new Vector2(190, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(100, -30));
                UI_Tool.FormatRect(tooltip.desc_tf, new Vector2(490, 70),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20));
                UI_Tool.FormatRect(tooltip.price_tf, new Vector2(230, 60),
                    new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-30, -30));
                UI_Tool.FormatRect(tooltip.extra_1_tf, new Vector2(100, 40),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(115, 20));
                UI_Tool.FormatRect(tooltip.extra_2_tf, new Vector2(220, 40),
                    new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-115, 20));
                    
                // Text Formatting

                UI_Tool.FormatText(tooltip.title, arial, 24, Color.white, TextAnchor.UpperCenter, FontStyle.Bold);
                UI_Tool.FormatText(tooltip.price, arial, 24, Color.white, TextAnchor.UpperRight, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.desc, arial, 20, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.extra_1, arial, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                UI_Tool.FormatText(tooltip.extra_2, arial, 18, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                break;
            }
            case Tooltip.Display_Format.Fever:
            {
                UI_Tool.FormatRect(tooltip.tf, new Vector2(550, 220),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(15, 310));
                UI_Tool.FormatRect(tooltip.icon_tf, new Vector2(60, 60),
                    new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(30, -25));
                UI_Tool.FormatRect(tooltip.title_tf, new Vector2(300, 60),
                    new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -25));
                UI_Tool.FormatRect(tooltip.desc_tf, new Vector2(490, 70),
                    new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 25));
                UI_Tool.FormatRect(tooltip.price_tf, new Vector2(170, 40),
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 5));
                UI_Tool.FormatRect(tooltip.extra_1_tf, new Vector2(160, 40),
                    new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(30, 5));
                UI_Tool.FormatRect(tooltip.extra_2_tf, new Vector2(160, 40),
                    new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-30, 5));

                // Text Formatting

                UI_Tool.FormatText(tooltip.title, arial, 36, new Color(1, 0.8f, 0), TextAnchor.UpperCenter, FontStyle.Bold);
                tooltip.title.text = "FEVER BAR";
                
                UI_Tool.FormatText(tooltip.price, arial, 14, Color.white, TextAnchor.MiddleCenter, FontStyle.Normal);
                tooltip.price.text = $"Drain   |   {data.fever_data.drain * 100}%";
                
                UI_Tool.FormatText(tooltip.extra_1, arial, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                tooltip.extra_1.text = $"MAX   |   {(system.fever_system.max / data.fever_data.gain).ToString("#,0")}";
                
                UI_Tool.FormatText(tooltip.extra_2, arial, 14, Color.white, TextAnchor.MiddleRight, FontStyle.Normal);
                tooltip.extra_2.text = $"Persistence   |   {data.fever_data.persistence * 100}%";

                UI_Tool.FormatText(tooltip.desc, arial, 14, Color.white, TextAnchor.MiddleLeft, FontStyle.Normal);
                tooltip.desc.text =
                    $"<i><size=14><color=#cccccc>When active...</color></size></i>\nClick Power Multi : <color=#ffcc00>{data.fever_data.click_multi * 100f}%</color>\n" +
                    $"BPS Multiplier      : <color=#ffcc00>{data.fever_data.bps_multi * 100f}%</color>";
                break;
            }
        }
    }
    void Init_Tooltip()
    {
        GameObject tt, tt_icon, tt_title, tt_price, tt_desc, tt_extra_1, tt_extra_2;
        tooltip = new Tooltip();

        tt = new GameObject("Tooltip");
        tt_icon = new GameObject("Icon");
        tt_title = new GameObject("Title");
        tt_price = new GameObject("Price");
        tt_desc = new GameObject("Desc");
        tt_extra_1 = new GameObject("Extra 1");
        tt_extra_2 = new GameObject("Extra 2");
        
        tt.transform.SetParent(info_canvas.transform, false);
        tt_icon.transform.SetParent(tt.transform, false);
        tt_title.transform.SetParent(tt.transform, false);
        tt_price.transform.SetParent(tt.transform, false);
        tt_desc.transform.SetParent(tt.transform, false);
        tt_extra_1.transform.SetParent(tt.transform, false);
        tt_extra_2.transform.SetParent(tt.transform, false);

        Image background = tt.AddComponent<Image>();

        tooltip.tf = background.rectTransform;
        tooltip.icon = tt_icon.AddComponent<Image>();
        tooltip.title = tt_title.AddComponent<Text>();
        tooltip.price = tt_price.AddComponent<Text>();
        tooltip.desc = tt_desc.AddComponent<Text>();
        tooltip.extra_1 = tt_extra_1.AddComponent<Text>();
        tooltip.extra_2 = tt_extra_2.AddComponent<Text>();

        tooltip.icon_tf = tooltip.icon.rectTransform;
        tooltip.title_tf = tooltip.title.rectTransform;
        tooltip.price_tf = tooltip.price.rectTransform;
        tooltip.desc_tf = tooltip.desc.rectTransform;
        tooltip.extra_1_tf = tooltip.extra_1.rectTransform;
        tooltip.extra_2_tf = tooltip.extra_2.rectTransform;
        
        background.sprite = default_box;
        background.type = Image.Type.Sliced;
        background.pixelsPerUnitMultiplier = 15;
        background.color = new Color(1, 1, 1, 0.7f);

        background.raycastTarget = false;
        tooltip.icon.raycastTarget = false;
        tooltip.title.raycastTarget = false;
        tooltip.price.raycastTarget = false;
        tooltip.desc.raycastTarget = false;
        tooltip.extra_1.raycastTarget = false;
        tooltip.extra_2.raycastTarget = false;

        tt.SetActive(false);
    }


    // OPTIONS

    void Init_Options()
    {
        GameObject options_panel, title, title_text_container, close, close_text_container, backdrop;

        options_obj = new GameObject("Options").AddComponent<RectTransform>();
        options_obj.SetParent(info_canvas.transform, false);
        UI_Tool.FormatRect(options_obj);

        backdrop = UI_Tool.ImgSetup("Backdrop", options_obj.transform, out Image backdrop_img, false);
        UI_Tool.FormatRect(backdrop_img.rectTransform);
        backdrop_img.color = new Color(0, 0, 0, 0.3f);

        options_panel = UI_Tool.ImgSetup("Options Panel", options_obj.transform, out Image panel_img, default_box, false);
        panel_img.rectTransform.localPosition = new Vector2(0, 100);
        panel_img.rectTransform.sizeDelta = new Vector2(400, 500);
        panel_img.color = new Color(0.6f, 0.6f, 0.6f);

        title = UI_Tool.ImgSetup("Options Title", options_panel.transform, out Image title_img, default_box, false);
        UI_Tool.FormatRectNPos(title_img.rectTransform, new Vector2(0, 100),
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
        title_img.color = Color.white;

        title_text_container = UI_Tool.TextSetup("Title Text", title.transform, out Text title_text, false);
        UI_Tool.FormatRect(title_text.rectTransform);
        UI_Tool.FormatText(title_text, arial, 48, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
        title_text.alignByGeometry = true;
        title_text.text = "OPTIONS";

        close = UI_Tool.ButtonSetup("Close", options_panel.transform, out Image close_img, out Button close_button, ui_sprites[3], Toggle_Options);
        UI_Tool.FormatRect(close_img.rectTransform, new Vector2(50, 50),
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-15, 0));

        close_text_container = UI_Tool.TextSetup("Close Text", close.transform, out Text close_text, false);
        UI_Tool.FormatRect(close_text.rectTransform);
        UI_Tool.FormatText(close_text, arial, 28, pure_black, TextAnchor.MiddleCenter, FontStyle.Bold);
        close_text.alignByGeometry = true;
        close_text.text = "X";

        Text[] opt_txt = new Text[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            GameObject opt_obj, opt_txt_obj;

            opt_obj = UI_Tool.ButtonSetup($"Option {i}", options_panel.transform, out Image opt_img, out options[i], ui_sprites[3], null);
            UI_Tool.FormatRect(opt_img.rectTransform, new Vector2(300, 70),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -160 - (90 * i)));

            opt_txt_obj = UI_Tool.TextSetup("Option Text", opt_obj.transform, out opt_txt[i], false);
            UI_Tool.FormatRect(opt_txt[i].rectTransform);
            UI_Tool.FormatText(opt_txt[i], arial, 32, pure_black, TextAnchor.MiddleCenter, FontStyle.Normal);
            opt_txt[i].alignByGeometry = true;
        }
        opt_txt[0].text = "Save";
        opt_txt[1].text = "Achievements";
        opt_txt[2].text = "Fullscreen";
        opt_txt[3].text = "Quit";

        options[0].onClick.AddListener(() => { if (File_Manager.File_Save()) Status_Update(status_msgs, "File saved!", Status_Msg.Type.Bonus); });
        options[1].onClick.AddListener(() => { achievements.DisplayAchievements(true); });
        options[2].onClick.AddListener(Toggle_Fullscreen);
        options[3].onClick.AddListener(Quit_Classic);

        options_obj.gameObject.SetActive(false);
    }
    public void Toggle_Options()
    {
        options_obj.gameObject.SetActive(!options_obj.gameObject.activeSelf);
        UI_Tool.ToggleCanvasPriority(gameObject, info_canvas.GetComponent<Canvas>());
    }
    void Toggle_Fullscreen()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(1280, 720, false);
        else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
    }

    // GAME UPDATES

    void Start()
    {
        system = gameObject.GetComponent<Classic_System>();

        gameObject.tag = "Main";
        
        buildings_canvas = UI_Tool.CanvasSetup("Buildings Canvas", gameObject.transform);
        upgrades_canvas = UI_Tool.CanvasSetup("Upgrades Canvas", gameObject.transform);
        bits_canvas = UI_Tool.CanvasSetup("Bits Canvas", gameObject.transform);
        messages_canvas = UI_Tool.CanvasSetup("Messages Canvas", gameObject.transform);
        fever_canvas = UI_Tool.CanvasSetup("Fever Canvas", gameObject.transform);
        info_canvas = UI_Tool.CanvasSetup("Info Canvas", gameObject.transform);

        Init_Bits();
        Init_Building_Shop();
        Init_Upgrades_Shop();
        Init_Fever_System();
        Init_Tooltip();
        Init_Options();

        status_messages_list = new GameObject($"Status Messages List");
        click_popups_list = new GameObject($"Click Popups List");
        Init_Status_Messages(status_messages_list, status_msgs, "Status Update", 0, 18);
        Init_Status_Messages(click_popups_list, click_popups, "Click Popup", 1, 32);

        messages_canvas.GetComponent<GraphicRaycaster>().enabled = false; // We don't need this
    }

    void Update()
    {
        mouse_pos = Input.mousePosition;

        fever_meter.tf.localScale = new Vector2(1, system.fever_system.active ? 1 : system.fever_system.val * .01f);
        fever_meter_secondary.tf.localScale = new Vector2(1, system.fever_system.bars_filled > 0 && system.fever_system.max_bars > 1 ? system.fever_system.val * .01f : 0);

        if (system.fever_system.active)
        {
            /*
            if (system.fever_system.bars_filled == 8 && !Furnace_Interface.ether_fever_on)
            {
                fever_text.text = "ETHER FEVER";
                fever_text.color = Color.red;
            }
            if (system.fever_system.bars_filled < 8 && Furnace_Interface.ether_fever_on)
            {
                fever_text.text = "FEVER";
                fever_text.color = new Color(0.9f, 0.7f, 0);
            }
            */

            fever_text.gameObject.SetActive(true);
            fever_meter.fever_bar.color = (system.fever_system.bars_filled > 1) ? 
                new Color(0.9f, 0.7f - ((system.fever_system.bars_filled - 1) * 0.0825f), 0) : new Color(1, 1, 0.1f);
            fever_meter_secondary.fever_bar.color = new Color(0.9f, 0.7f - (system.fever_system.bars_filled * 0.0825f), 0);
        }
        else
        {
            fever_text.gameObject.SetActive(false);
            fever_meter.fever_bar.color = 
                new Color(0.6f + (system.fever_system.val * 0.003f), system.fever_system.val * 0.004f, 0);
        }

        // THIS ALONE REDUCES FPS CONSIDERABLY. Consider optimizing To_Bit_Notation() (PARSING)
        bit_counter.text = $"Bits : {To_Bit_Notation(data.currency, "#,0")}";
        bps_counter.text = $"Bits per second : {To_Bit_Notation(data.bps * (system.fever_system.active ? 1.5f : 1), "#,0.#")}";


        for (int i = 0; i < building_buttons.Length; i++)
            if (building_buttons[i].button.gameObject.activeSelf)
                if (data.currency + 0.5f < data.b_data[i].price)
                {
                    if (building_buttons[i].button.IsInteractable())
                        building_buttons[i].button.interactable = false;
                }
                else if (!building_buttons[i].button.IsInteractable())
                    building_buttons[i].button.interactable = true;
                

        if (tooltip.tf.gameObject.activeSelf && tooltip.format != Tooltip.Display_Format.Fever) Update_Tooltip();

        foreach (Status_Msg msg in status_msgs)
            if (msg.timer != 0)
                if (msg.timer != 480) ++msg.timer;
                else { Status_Update_Clear(msg); }

        foreach (Status_Msg msg in click_popups)
            if (msg.timer != 0)
                if (msg.timer != 240) ++msg.timer;
                else { Status_Update_Clear(msg); }
    }

    void FixedUpdate()
    {
        if (++update_tick > 30)
        {
            // Skip first button; it should always be unlocked
            for (int i = 1; i < data.b_data.Length; i++)
                if (!building_buttons[i].button.gameObject.activeSelf)
                    if (data.total_money >= data.b_data[i].price)
                    {
                        building_buttons[i].active = true;
                        building_buttons[i].button.gameObject.SetActive(true);
                        Update_Buildings_List();
                    }
                    

            if (system.upgrades_system.Check_Upgrades())
            {
                Status_Update(status_msgs, "New upgrades are avaialable!", Status_Msg.Type.Bonus);
                Refresh_Upgrades(active_tab, false);
            }

            for (int i = 0; i < active_upgrades.Length; i++)
                if (active_upgrades[i] == -1) break;
                else if (data.currency + 0.5f >= system.upgrades_system.upgrades[active_upgrades[i]].price)
                {
                    if (!upgrade_buttons[active_upgrades[i]].button.IsInteractable())
                        upgrade_buttons[active_upgrades[i]].button.interactable = true;
                }
                else if (upgrade_buttons[active_upgrades[i]].button.IsInteractable())
                    upgrade_buttons[active_upgrades[i]].button.interactable = !upgrade_buttons[active_upgrades[i]].button.IsInteractable();

            update_tick = 0;
        }
    }
}