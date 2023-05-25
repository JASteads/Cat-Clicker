[System.Serializable]
public class Profile
{
    public int Slot { get; }

    public CLSaveData cl;
    public AchieveData achievements;

    public Profile(int slot)
    {
        cl = new CLSaveData();
        achievements = new AchieveData(SysManager.GetADB());
        
        Slot = slot;
    }
}