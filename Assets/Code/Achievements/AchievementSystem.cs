using System.Collections.Generic;

public class AchievementSystem
{
    public List<AchievementInfo> database;
    AchievementsInterface aInterface;

    public AchievementSystem()
    {
        InitDatabase();
        aInterface = new AchievementsInterface(database);
    }

    public List<AchievementInfo> UpdateDB()
    {
        List<AchievementInfo> unlockedAchievements = 
            new List<AchievementInfo>();

        database.ForEach(a => 
        { if (a.Update()) unlockedAchievements.Add(a); });

        return unlockedAchievements;
    }

    public void DisplayInterface()
    { aInterface.Display(); }

    void InitDatabase() 
    {
        database = new List<AchievementInfo>
        {
            new AchievementInfo("First Click!",
            "Start your journey with your first click ever!",
            AchievementType.CP, 1,
            1,
            () => { return SysManager.activeProfile.cl.TotalClicks; },
            () => { return SysManager.activeProfile.cl.TotalClicks > 0; },
            null),

            new AchievementInfo("Start Your Engines",
            "Drop the first seeds, and they do the work for you. You now make 1 Bit per second!",
            AchievementType.BPS, 5,
            1,
            () => { return SysManager.activeProfile.cl.BitsPerSecond; },
            () => { return SysManager.activeProfile.cl.BitsPerSecond >= 1; },
            null),

            new AchievementInfo("Bit Hoarding",
            "You've mustered up more than 100 bits in total. Now you've got a personal stash!",
            AchievementType.CP, 10,
            100,
            () => { return SysManager.activeProfile.cl.CurrencyTotal; },
            () => { return SysManager.activeProfile.cl.CurrencyTotal >= 100; },
            null),

            new AchievementInfo("FEVER!",
            "You've reached a new height of production with your first Fever boost!",
            AchievementType.STORY, 1,
            1,
            () => { return SysManager.activeProfile.cl.misc.FeverUpTime; },
            () => { return SysManager.activeProfile.cl.misc.FeverUpTime > 0; },
            null),

            new AchievementInfo("One-Hundred Clicks",
            "That's 100 total clicks. Click click away...",
            AchievementType.CP, 5,
            100,
            () => { return SysManager.activeProfile.cl.TotalClicks; },
            () => { return SysManager.activeProfile.cl.TotalClicks > 99; },
            null),

            new AchievementInfo("Beat the Game",
            "Purchase the final upgrade that wins the game!",
            AchievementType.STORY, 777,
            1,
            null,
            null,
            null)
        };
    }
}