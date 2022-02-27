//#define DEBUG

using UnityEngine;

public delegate void Effect();  // Delegate for misc. effects for upgrades and achievements.

public class Data
{
    public int slot, difficulty, total_clicks;
    public double total_money, currency, click_power, bps;
    public long time_played, time_started;

    public Buildings_System.Building_Data[] b_data;
    public int[] u_statuses;
    public Fever_System.Fever_Data fever_data;
    public Achievements_Data.Color_Bits color_bits;

    
    /* Debug_Cheats() : Quick cheats for debugging. */
    private void Debug_Cheats()
    {
#if DEBUG
        bool infinite_money = false;
        if (infinite_money)
        {
            currency = total_money = 1.000e65;
        }
#endif
    }

    public Data()
    {
        total_money = total_clicks = 0;
        time_played = 0;
        currency = bps = 0;
        click_power = 1;
        
        b_data = new Buildings_System.Building_Data[12];
        u_statuses = new int[14];
        fever_data = new Fever_System.Fever_Data(0, 3.33f, 1, 1, 0);
        color_bits = new Achievements_Data.Color_Bits(0);
        time_started = System.DateTime.Now.ToBinary();

        Debug_Cheats();
    }
    public Data(int slot, int difficulty, double total_money, double currency, double click_power, double bps, long time_played, long time_started)
    {
        this.slot = slot;
        this.difficulty = difficulty;
        this.total_money = total_money;
        this.currency = currency;
        this.click_power = click_power;
        this.bps = bps;
        this.time_played = time_played;
        this.time_started = time_started;

        b_data = new Buildings_System.Building_Data[12];
        u_statuses = new int[14];
        fever_data = new Fever_System.Fever_Data(0, 3.33f, 1, 1, 0);

        Debug_Cheats();
    }
}

public class Database : MonoBehaviour
{
    public static Font arial;
    public static Sprite[] ui_sprites;
    public static Sprite default_button, default_box;
    public static Camera main_cam;

    public static Data data;
    public static Classic_System classic_system;
    public static Main_Menu main_menu;
    public static Achievements achievements;

    public long fever_active_time = 0;

    public static void Quit_Game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /* Start and stop for Main_Menu */
    public static void Begin_Main_Menu()
    { main_menu = new GameObject("Main Menu").AddComponent<Main_Menu>(); }
    public static void Quit_Main_Menu()
    { Destroy(main_menu.gameObject); }

    /* Start and stop for Classic */
    public static void Begin_Classic()
    { classic_system = new GameObject("Classic System").AddComponent<Classic_System>(); }
    public static void Quit_Classic()
    { Begin_Main_Menu(); Destroy(classic_system.gameObject); }

    /* Starts Achievements system */
    public static void Begin_Achievements()
    { achievements = new GameObject("Achievements").AddComponent<Achievements>(); }
    
    /* To_Bit_Notation() : Condenses value into custom variant of scientific notation */
    public static string To_Bit_Notation(double val, string format)
    {
        if (val >= 1.000e6)
        {
            string num_str, suffix_str = "";
            float remainder;

            // Convert val into string with four decimal points
            num_str = val.ToString("e5");

            // PARSING IS SLOW. Consider making custom parse if this causes problems.
            float.TryParse(num_str.Substring(0, 7), out float prefix); // Number
            int.TryParse(num_str.Substring(num_str.Length - 3), out int suffix_num); // Tail


            remainder = suffix_num * 0.333f;
            while (remainder > 1) --remainder;
            if ((int)(remainder + 0.2f) != 1)
                for (int i = 0; i < (int)((remainder * 3) + 0.5f); i++) prefix *= 10;

            // Returns the appropriate suffix based on suffix_num's value
            suffix_str = Get_Suffix(suffix_num);

            // Concatenate both num_str and suffix_str
            num_str = $"{prefix.ToString("##0.000")} {suffix_str}";
            return num_str;
        }
        return val.ToString(format);
    }

    /* Get_Suffix() : If exoponent would display a value over 1M, return custom suffix. */
    static string Get_Suffix(int suffix_num)
    {
        if (suffix_num > 5)
        {
            if (suffix_num < 9) return "million";
            if (suffix_num < 12) return "billion";
            if (suffix_num < 15) return "trillion";
            if (suffix_num < 18) return "quadillion";
            if (suffix_num < 21) return "quintillion";
            if (suffix_num < 24) return "sextillion";
            if (suffix_num < 27) return "septillion";
            if (suffix_num < 30) return "octillion";
            if (suffix_num < 33) return "nonillion";
            if (suffix_num < 36) return "decillion";
            if (suffix_num < 39) return "undecillion";
            if (suffix_num < 42) return "duodecillion";
            if (suffix_num < 45) return "tredecillion";
            if (suffix_num < 48) return "quattuordecillion";
            if (suffix_num < 51) return "quindecillion";
            if (suffix_num < 54) return "sexdecillion";
            if (suffix_num < 57) return "septendecillion";
            if (suffix_num < 60) return "octodecillion";
            if (suffix_num < 63) return "novemdecillion";
            if (suffix_num < 66) return "vigintillion";
            return "gigantillion";
        }

        return "";
    }


    /* Creates the Database when game starts. This allows game to start without the Database already being active */
    /* DISABLED. We don't want this running on top of the new code
    [RuntimeInitializeOnLoadMethod]
    static void Start_Database()
    { Database database = new GameObject("Database").AddComponent<Database>(); }
    */

    void Start()
    {
        arial = Resources.Load("Fonts/alphbeta") as Font;
        ui_sprites = Resources.LoadAll<Sprite>("Sprites/Cat_Clicker_Sprite_Sheet_V1");
        default_box = ui_sprites[0];
        default_button = ui_sprites[1];
        main_cam = Camera.main;

        Begin_Main_Menu();
        Begin_Achievements();
    }

    void FixedUpdate()
    {
        if (data != null)
            fever_active_time = data.fever_data.time_active;
    }
}