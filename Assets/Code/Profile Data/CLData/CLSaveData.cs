using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CLSaveData
{
    public double CurrencyTotal { get; set; }
    public double CurrencyCurrent { get; set; }
    public double ClickPower { get; set; }
    public double BitsPerSecond { get; set; }
    
    public List<CLBuildingData> buildingData;
    public List<CLUpgradeData> upgradeData;
    public CLFeverData feverData;
    public CLMiscData misc;

    public int TotalClicks { get; set; }


    public CLSaveData()
    {
        CurrencyTotal = 0;
        CurrencyCurrent = 0;
        ClickPower = 1;
        BitsPerSecond = 0;
        TotalClicks = 0;
        
        buildingData = InitBData();
        upgradeData = new List<CLUpgradeData>();
        feverData = new CLFeverData();
        misc = new CLMiscData();
    }

    public int GetCurrencyCurrent()
    {
        return Mathf.RoundToInt((float)CurrencyCurrent);
    }
    public int GetCurrencyTotal()
    {
        return Mathf.RoundToInt((float)CurrencyTotal);
    }

    public void DeductBits(double amount)
    { CurrencyCurrent -= amount; }

    List<CLBuildingData> InitBData()
    {
        return new List<CLBuildingData>()
        {
            new CLBuildingData("Cello", 0.1f),
            new CLBuildingData("Dig-Dig", 1),
            new CLBuildingData("Smithy", 6),
            new CLBuildingData("Magic Orb", 20),

            new CLBuildingData("Work Cat", 100),
            new CLBuildingData("Engy Cat", 500),
            new CLBuildingData("Magic Cat", 1500),
            new CLBuildingData("Science Cat", 3140),

            new CLBuildingData("Mine", 10000),
            new CLBuildingData("Factory", 25000),
            new CLBuildingData("Tower", 500000),
            new CLBuildingData("Laboratory", 1000000)
        };
    }
}