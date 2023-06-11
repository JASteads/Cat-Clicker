[System.Serializable]
public class CLMiscData
{
    public long TimePlayed { get; set; }
    public long TimeStarted { get; set; }
    
    public int Difficulty { get; }
    public int FeverUpTime { get; set; }

    public CLMiscData()
    {
        TimePlayed = 0;
        TimeStarted = System.DateTime.Now.ToBinary();
        Difficulty = FeverUpTime = 0;
    }
}