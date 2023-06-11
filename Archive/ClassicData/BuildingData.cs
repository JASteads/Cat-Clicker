[System.Serializable]
public class BuildingData
{
    public string Name { get; }
    public string Desc { get; }
    public float BaseValue { get; set; }
    public float ClickPowerAmplifier { get; set; }
    public double Price { get; set; }
    public int Amount { get; set; }
    public float AccumulativeValue { get; set; }

    const float PRICE_MULTIPLIER = 1.15f;


    
    public BuildingData(string name, string desc, float baseValue, int price)
    {
        Name = name;
        Desc = desc;
        BaseValue = baseValue;
        ClickPowerAmplifier = 0;
        Price = price;
        Amount = 0;
        AccumulativeValue = 0;
    }

    public void Buy()
    {
        ++Amount;
        Price *= PRICE_MULTIPLIER;
    }
    public void Buy(int amount)
    {
        Amount += amount;

        for (int i = 0; i < amount; i++)
            Price *= PRICE_MULTIPLIER;
    }

    public double GetClickPowerModifiers()
    {
        double buildingCPModifier = BaseValue * Amount * ClickPowerAmplifier;

        return buildingCPModifier;
    }

    public void UpdateAccumulativeValue(float updateMultiplier)
    {
        AccumulativeValue += BaseValue * Amount * updateMultiplier;
    }
}
