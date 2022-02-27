[System.Serializable]
public class Profile
{
    /* 
     * WHEN ADDING ANYMORE MODES, MONITOR THE COMPLIEXITY OF THE PROFILE MANAGEMENT.
     * IF IT BECOMES DIFFICULT TO CONTROL, CONSIDER BREAKING INTO MULTIPLE LOAD FILES.
    */
    public int Slot { get; }
    public Difficulty Difficulty { get; }

    public CLSCSaveData clscSaveData;
    public AchievementSystem achievements;

    public Profile()
    {

    }
    public Profile(int slot, Difficulty difficulty)
    {
        clscSaveData = new CLSCSaveData();
        achievements = new AchievementSystem();
        
        Slot = slot;
        Difficulty = difficulty;
    }
}

public enum Difficulty
{
    BASIC,
    STANDARD,
    ADVANCED
}