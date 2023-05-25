using System;

[Serializable]
public class AchieveDataBlock
{
    public string name;
    public DateTime dateEarned;

    public AchieveDataBlock(string name)
    {
        this.name = name;
        dateEarned = DateTime.MinValue;
    }

    public bool IsUnlocked()
    {
        return dateEarned > DateTime.MinValue;
    }
}