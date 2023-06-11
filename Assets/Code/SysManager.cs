using System.Collections.Generic;
using UnityEngine;

public class SysManager
{
    public readonly static FileManagement fileManager;

    public readonly static Font DEFAULT_FONT;
    public readonly static Sprite defaultButton, defaultBox;
    public readonly static Camera mainCam;
    public static Sprite[] uiSprites;

    public static readonly AchievementSystem achieveSys;
    public static Profile activeProfile;
    static GameMode gameMode;


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

    /* Game Modes */
    public static void LoadMainMenu()
    { ChangeMode(new MainMenu()); }
    public static void LoadCLMode()
    { ChangeMode(new CLMode()); }

    static void ChangeMode(GameMode newMode)
    {
        gameMode?.Reset();
        gameMode = newMode;
    }

    public static void ToggleFullscreen()
    {
        if (Screen.fullScreen)
            Screen.SetResolution(1280, 720, false);
        else
            Screen.SetResolution(
                Screen.currentResolution.width,
                Screen.currentResolution.height, true);
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