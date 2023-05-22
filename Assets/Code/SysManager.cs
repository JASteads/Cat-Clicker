using UnityEngine;

public class SysManager
{
    public readonly static Font DEFAULT_FONT;
    public static Sprite defaultButton, defaultBox;
    public static Sprite[] uiSprites;
    public static Camera mainCam;

    public static Transform baseTF;
    public static MainMenu mainMenu;
    public static CLSCSystem classicMode;
    public static AchievementsInterface achievementsInterface;

    public static FileManagement fileManager;
    public static Profile profile;

    static SysManager()
    {
        DEFAULT_FONT = Resources.Load("Fonts/alphbeta") as Font;
        uiSprites = Resources.LoadAll<Sprite>
            ("Sprites/Cat_Clicker_Sprite_Sheet_V1");
        defaultBox = uiSprites[0];
        defaultButton = uiSprites[1];
        mainCam = new GameObject("Main Cam").AddComponent<Camera>();
        mainCam.backgroundColor = new Color(0.2f, 0.15f, 0.3f);

        fileManager = new FileManagement();
        profile = new Profile();

        baseTF = new GameObject("Base").transform;
    }

    [RuntimeInitializeOnLoadMethod]
    static void StartApplication()
    {
        LoadMainMenu();
    }
    
    /* Start and stop for Main_Menu */
    public static void LoadMainMenu()
    {
        mainMenu = new MainMenu(baseTF);
    }

    /* Start and stop for Classic */
    public static void LoadClassicMode()
    {
        classicMode = new GameObject("Classic System")
            .AddComponent<CLSCSystem>();
    }
    public static void QuitClassic()
    {
        LoadMainMenu();
        Object.Destroy(classicMode.gameObject);
    }

    /* Starts Achievements system */
    public static void LoadAchievementsInterface()
    {
        // Make sure to clear the old achievement
        // interface when loading a new profile
        if (achievementsInterface != null)
            QuitAchievementsInterface();

        achievementsInterface = new GameObject
            ("Achievements Interface")
            .AddComponent<AchievementsInterface>();
    }
    public static void QuitAchievementsInterface()
    {
        Object.Destroy(achievementsInterface.gameObject);
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}