[System.Serializable]
public class AchievementData
{
    public delegate bool Condition();

    public string Title { get; }
    public string Desc { get; }
    public string Tag { get; }

    public AchievementType Type { get; }
    public int BitAmount { get; }

    // Progress tracking. trackProgress should return a value to Progress
    public AchievementStatus Status { get; set; }
    public double Progress { get; set; }
    public double Max { get; }
    public System.Func<double> trackProgress;
    
    public System.DateTime DateEarned { get; set; }
    Condition unlockCondition;
    Effect onUnlock;



    // CONSTRUCTOR
    public AchievementData(string title, string desc, AchievementType achivementType, 
        int bitAmount, double max, System.Func<double> toTrack, Condition condition, Effect onUnlockFunc)
    {
        Title = title;
        Desc = desc;
        Tag = "";

        Type = achivementType;
        BitAmount = bitAmount;
        unlockCondition = condition;
        onUnlock = onUnlockFunc;

        Status = AchievementStatus.LOCKED;
        Progress = 0;
        Max = max;
        trackProgress = toTrack;

        DateEarned = System.DateTime.FromBinary(0);
    }

    public void Unlock()
    {
        Status = AchievementStatus.UNLOCKED;
        DateEarned = System.DateTime.Now;
        Progress = Max;

        onUnlock?.Invoke();

        SysManager.achievementsInterface.UpdateListing();
    }

    public bool CheckCondition()
    {
        if (unlockCondition != null)
            return unlockCondition();
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