using System;

public class AchievementInfo
{
    public delegate bool Condition();

    public string Title { get; }
    public string Desc { get; }
    public int BitAmount { get; }

    public AchievementType Type { get; }
    public AchievementStatus Status { get; set; }

    public double Progress { get; set; }
    public double Max { get; }
    
    readonly Func<double> GetProperties;
    readonly Condition UnlockCondition;
    readonly Effect OnUnlock;


    public AchievementInfo(string title, string desc,
        AchievementType achivementType, int bitAmount,
        double max, Func<double> propertiesFunc,
        Condition condition, Effect onUnlockFunc)
    {
        Title = title;
        Desc = desc;

        Type = achivementType;
        BitAmount = bitAmount;
        UnlockCondition = condition;
        OnUnlock = onUnlockFunc;

        Status = AchievementStatus.LOCKED;
        Progress = 0;
        Max = max;
        GetProperties = propertiesFunc;
    }

    public bool Update()
    {
        bool conditionMet = CheckCondition();

        if (conditionMet)
        {
            if (!SysManager.activeProfile.achievements
                .Find(Title).IsUnlocked()) Unlock();
            else
            {
                Status = AchievementStatus.UNLOCKED;
                Progress = Max;
            }
            return true;
        }
        if (GetProperties != null)
        {
            Status = AchievementStatus.LOCKED;
            Progress = GetProperties();
        }

        return false;
    }

    public void Unlock()
    {
        UnityEngine.Debug.Log($"{Title} is unlocked!");

        SysManager.activeProfile.achievements.data
            .Find(a => a.name == Title).dateEarned = DateTime.Now;

        Progress = Max;
        Status = AchievementStatus.UNLOCKED;

        OnUnlock?.Invoke();
    }

    bool CheckCondition()
    {
        if (UnlockCondition != null)
            return UnlockCondition();
        return false;
    }
}

public enum AchievementType
{
    STORY,
    CP,
    BPS,
    RESEARCH,
    SECRET
};

public enum AchievementStatus
{
    LOCKED,
    UNLOCKED,
    HIDDEN
};