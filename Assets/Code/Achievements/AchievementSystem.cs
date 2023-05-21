using System.Collections.Generic;

[System.Serializable]
public class AchievementSystem
{
    public List<AchievementData> data = new List<AchievementData>
    {
        new AchievementData("First Click!",
        "Start your journey with your first click ever!",
        AchievementType.CP, 1,
        1,
        () => { return SysManager.profile.clscSaveData.TotalClicks; },
        () => { return SysManager.profile.clscSaveData.TotalClicks > 0; },
        null),

        new AchievementData("Start Your Engines",
        "Drop the first seeds, and they do the work for you. You now make 1 Bit per second!",
        AchievementType.BPS, 5,
        1,
        () => { return SysManager.profile.clscSaveData.BitsPerSecond; },
        () => { return SysManager.profile.clscSaveData.BitsPerSecond >= 1; },
        null),

        new AchievementData("Bit Hoarding",
        "You've mustered up more than 100 bits in total. Now you've got a personal stash!",
        AchievementType.CP, 10,
        100,
        () => { return SysManager.profile.clscSaveData.CurrencyTotal; },
        () => { return SysManager.profile.clscSaveData.CurrencyTotal >= 100; },
        null),

        new AchievementData("FEVER!",
        "You've reached a new height of production with your first Fever boost!",
        AchievementType.STORY, 1,
        1,
        () => { return SysManager.profile.clscSaveData.misc.FeverUpTime; },
        () => { return SysManager.profile.clscSaveData.misc.FeverUpTime > 0; },
        null),

        new AchievementData("One-Hundred Clicks",
        "That's 100 total clicks. Click click away...",
        AchievementType.CP, 5,
        100,
        () => { return SysManager.profile.clscSaveData.TotalClicks; },
        () => { return SysManager.profile.clscSaveData.TotalClicks > 99; },
        null),

        new AchievementData("Beat the Game",
        "Purchase the final upgrade that wins the game!",
        AchievementType.STORY, 777,
        1,
        null,
        null,
        null)
    };

    // CHECK PARENTS AND CHILDREN OF AN ACHIEVEMENT LATER
    public void CheckAchievements()
    {
        data.ForEach(achievement =>
        {
            // Update progress bars for locked achievements
            if (achievement.Status != AchievementStatus.UNLOCKED && achievement.trackProgress != null)
            {
                achievement.Progress = achievement.trackProgress();
            }

            // If an achievement is locked and its conditions are met, unlock it.
            if (achievement.Status != AchievementStatus.UNLOCKED && achievement.CheckCondition())
            {
                if (achievement.CheckCondition())
                {
                    UnityEngine.Debug.Log($"{achievement.Title} is unlocked!");
                    achievement.Unlock();
                }
            }
        });
    }
}