using UnityEngine;

// public delegate void Effect();  // Delegate for misc. effects for upgrades and achievements.

public class SysManager : MonoBehaviour
{
    public static Font defaultFont;
    public static Sprite defaultButton, defaultBox;
    public static Sprite[] uiSprites;
    public static Camera mainCam;

    public static MainMenu mainMenu;
    public static CLSCSystem classicMode;
    public static AchievementsInterface achievementsInterface;

    public static FileManagement fileManager;
    public static Profile profile;
    
    void Start()
    {
        defaultFont = Resources.Load("Fonts/alphbeta") as Font;
        uiSprites = Resources.LoadAll<Sprite>("Sprites/Cat_Clicker_Sprite_Sheet_V1");
        defaultBox = uiSprites[0];
        defaultButton = uiSprites[1];
        mainCam = Camera.main;
        
        fileManager = new FileManagement();
        profile = new Profile();
        
        string savesDirectory = $"{System.IO.Directory.GetCurrentDirectory()}/Saves";

        // If there are no saves, create a new save directory. This can be used to signify first time loading the game
        if (!System.IO.Directory.Exists(savesDirectory))
        {
            Debug.Log("Saves directory doesn't exist! Creating new one ..");
            System.IO.Directory.CreateDirectory(savesDirectory);
        }

        LoadMainMenu();
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    /* Start and stop for Main_Menu */
    public static void LoadMainMenu()
    {
        mainMenu = new GameObject("Main Menu").AddComponent<MainMenu>();
    }
    public static void QuitMainMenu()
    { Destroy(mainMenu.gameObject); }

    /* Start and stop for Classic */
    public static void LoadClassicMode()
    {
        classicMode = new GameObject("Classic System").AddComponent<CLSCSystem>();
    }
    public static void QuitClassic()
    {
        LoadMainMenu();
        Destroy(classicMode.gameObject);
    }

    /* Starts Achievements system */
    public static void LoadAchievementsInterface()
    {
        // Make sure to clear the old achievement interface when loading a new profile
        if (achievementsInterface != null)
        {
            QuitAchievementsInterface();
        }
        achievementsInterface = new GameObject("Achievements Interface").AddComponent<AchievementsInterface>();
    }
    public static void QuitAchievementsInterface()
    {
        Destroy(achievementsInterface.gameObject);
    }

    /* Creates the SysManager when game starts. This allows game to start without the Database already being active */
    [RuntimeInitializeOnLoadMethod]
    static void StartGame()
    { SysManager SysManager = new GameObject("SysManager").AddComponent<SysManager>(); }
}