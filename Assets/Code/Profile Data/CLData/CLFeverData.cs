[System.Serializable]
public class CLFeverData
{
    public float Gain { get; set; }
    public float Drain { get; set; }
    public float Persistence { get; set; }
    public float ClickMultiplier { get; set; }
    public float BPSMultiplier { get; set; }
    public float Max { get; set; }

    public long TimeActive { get; set; }

    public CLFeverData()
    {
        ClickMultiplier = 2;
        BPSMultiplier = 1.5f;

        Gain = 3.5f;
        Drain = 1;
        Persistence = 1;

        Max = 100;

        TimeActive = 0;
    }
}