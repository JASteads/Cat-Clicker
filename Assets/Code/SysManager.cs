using System.Collections.Generic;
using UnityEngine;

public class SysManager
{
    public readonly static Font DEFAULT_FONT;
    public readonly static Sprite defaultButton, defaultBox;
    public static Sprite[] uiSprites;
    public static Camera mainCam;
    
    public static FileManagement fileManager;
    public static Profile activeProfile;

    static GameMode gameMode;
    static AchievementSystem achieveSys;


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
        achieveSys = new AchievementSystem();
    }

    [RuntimeInitializeOnLoadMethod]
    static void StartApplication()
    { LoadMainMenu(); }

    public static Transform GetMainTF()
    { return gameMode.transform; }
    public static void LoadMainMenu()
    { ChangeMode(new MainMenu()); }
    public static void LoadCLMode()
    { ChangeMode(new CLMode()); }
    public static void DisplayAchievements()
    { achieveSys.DisplayInterface(); }
    public static List<AchievementInfo> GetADB()
    { return achieveSys.database; }
    public static List<AchievementInfo> UpdateAchievements()
    { return achieveSys.UpdateDB(); }
    public static void ForceUnlockAchievement(string name)
    { GetADB().Find(a => a.Title == name)?.Unlock(); }

    static void ChangeMode(GameMode newMode)
    {
        if (gameMode != null)
            Object.Destroy(gameMode.transform.gameObject);
        gameMode = newMode;
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