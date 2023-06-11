[System.Serializable]
public class CLUpgradeData
{
    public string Name { get; }
    public Status Status { get; set; }

    public CLUpgradeData(string name)
    {
        Name = name;
        Status = Status.LOCKED;
    }
}