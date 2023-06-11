[System.Serializable]
public class CLBuildingData
{
    public string Name { get; }
    public float BaseValue { get; set; }
    public float ClickPowerAmplifier { get; set; }
    public int Amount { get; set; }
    public float AccumulativeValue { get; set; }


    public CLBuildingData(string name, float baseValue)
    {
        Name = name;
        BaseValue = baseValue;
        ClickPowerAmplifier = 0;
        Amount = 0;
        AccumulativeValue = 0;
    }
}