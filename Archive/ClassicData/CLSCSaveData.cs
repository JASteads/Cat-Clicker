using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CLSCSaveData
{
    public double CurrencyTotal { get; set; }
    public double CurrencyCurrent { get; set; }
    public double ClickPower { get; set; }
    public double BitsPerSecond { get; set; }
    
    public List<BuildingData> buildingsData;
    public List<CLSCUpgradeData> upgradesData;
    public FeverData feverData;
    public CLSCMiscStats misc;

    public int TotalClicks { get; set; }


    // CONSTRUCTOR
    public CLSCSaveData()
    {
        CurrencyTotal = 0;
        CurrencyCurrent = 0;
        ClickPower = 1;
        BitsPerSecond = 0;
        TotalClicks = 0;
        
        buildingsData = CreateStandardBuildings();
        upgradesData = CreateStandardUpgrades();
        feverData = new FeverData();
        misc = new CLSCMiscStats();
    }

    public int GetCurrencyCurrent()
    {
        return Mathf.RoundToInt((float)CurrencyCurrent);
    }
    public int GetCurrencyTotal()
    {
        return Mathf.RoundToInt((float)CurrencyTotal);
    }

    public void AddToCurrency(double amount)
    {
        CurrencyCurrent += amount;
        CurrencyTotal += amount;
    }

    List<BuildingData> CreateStandardBuildings()
    {
        return new List<BuildingData>
        {
            // Primary Characters
            new BuildingData(
                "Cello", "Your personal bit cat companion! Train" +
                " him to be active, and he'll produce more bits.",
                0.1f, 15),
            new BuildingData(
                "Dig-Dig", "The robot cat drill finds bits in" +
                " the ground. It ain't no trick to get rich quick!",
                1, 85),
            new BuildingData(
                "Smithy", "Smithy likes to take raw bits and turn" +
                "them into useful tools. For making more bits" +
                " of course!", 6, 700),
            new BuildingData(
                "Magic Orb", "The orb somehow creates bits out of" +
                " thin air! There seems to be a mysterious cat" +
                " inside the orb...", 20, 5000),

            // Bit Cats
            new BuildingData(
                "Work Cat", "", 100, 25000),
            new BuildingData(
                "Engy Cat", "", 500, 200000),
            new BuildingData(
                "Magic Cat", "", 1500, 420690),
            new BuildingData(
                "Science Cat", "", 3400, 1431100),

            // Bases
            new BuildingData(
                "Mine", "", 10000, 5000000),
            new BuildingData(
                "Factory", "", 25000, 20000000),
            new BuildingData(
                "Tower", "", 500000, 100000000),
            new BuildingData(
                "Laboratory", "", 1000000, 500000000)
        };
    }

    List<CLSCUpgradeData> CreateStandardUpgrades()
    {
        // Building preconditions are hardcoded since they aren't subject to change beyond number tweaks
        return new List<UpgradeData>
        {
            new UpgradeData("Double Sprouts", "Doubles BPS gained from Bit Plants", 100, UpgradeType.BUILDING,
            (up) => { return buildingsData[0].Amount > 0; },
            () => { buildingsData[0].BaseValue *= 2; }),

            new UpgradeData("Efficient Diggers", "Doubles BPS gained from Bit Diggers", 500, UpgradeType.BUILDING,
            (up) => { return buildingsData[1].Amount > 0; },
            () => { buildingsData[1].BaseValue *= 2; }),

            new UpgradeData("Double Click", "Doubles your Click Power", 1000, UpgradeType.CP,
            (up) => { return buildingsData[0].Amount >= 10; },
            () => { ClickPower *= 2; }),

            new UpgradeData("Shiny Plants", "Add some polish to your Bit Plants. Oh, and double their BPS too.", 5000, UpgradeType.BUILDING,
            (up) => { return buildingsData[0].Amount >= 100; },
            () => { buildingsData[0].BaseValue *= 2; }),

            new UpgradeData("Green Thumb", "Click Power gains +0.1 for each Bit Plant owned", 25000, UpgradeType.CP,
            (up) => { return buildingsData[0].AccumulativeValue >= 500; },
            () => { buildingsData[0].ClickPowerAmplifier += 0.1f; }),

            new UpgradeData("Precise Extraction", "Doubles BPS gained from Bit Extractors", 4000, UpgradeType.BUILDING,
            (up) => { return buildingsData[2].Amount > 0; },
            () => { buildingsData[2].BaseValue *= 2; }),

            new UpgradeData("New Fabrication Recipes", "Doubles BPS gained from Bit Fabricators", 20000, UpgradeType.BUILDING,
            (up) => { return buildingsData[3].Amount > 0; },
            () => { buildingsData[3].BaseValue *= 2; }),

            new UpgradeData("Greased Wheels", "Doubles BPS gained from Cat Wheels", 100000, UpgradeType.BUILDING,
            (up) => { return buildingsData[4].Amount > 0; },
            () => { buildingsData[4].BaseValue *= 2; }),

            new UpgradeData("Polished Gears", "Doubles BPS gained from Cat Treadmills", 500000, UpgradeType.BUILDING,
            (up) => { return buildingsData[5].Amount > 0; },
            () => { buildingsData[5].BaseValue *= 2; }),

            new UpgradeData("Use Medium Quality", "Doubles BPS gained from Cat VR", 1000000, UpgradeType.BUILDING,
            (up) => { return buildingsData[6].Amount > 0; },
            () => { buildingsData[6].BaseValue *= 2; }),

            new UpgradeData("Level Up", "Doubles BPS gained from Cat Simulators", 3000000, UpgradeType.BUILDING,
            (up) => { return buildingsData[7].Amount > 0; },
            () => { buildingsData[7].BaseValue *= 2; }),

            new UpgradeData("Longer Fever", "Increases the time before fever drains by 50%", 10000, UpgradeType.SPECIAL,
            (up) => { return GetCurrencyTotal() >= 1000000; },
            () => { feverData.Persistence *= 1.5f; }),

            new UpgradeData("Win the Game", "Kills the game; you win!", 1000000000, UpgradeType.SPECIAL,
            (up) => { return GetCurrencyTotal() >= 10000000; },
            () =>
            {
                SysManager.ForceUnlockAchievement("Beat the Game");
                SysManager.fileManager.FileSave();
                SysManager.QuitGame();
            }),

            new UpgradeData("Fever Buff", "You've seen fever, now let's make it better: Increase fever progress per click by 1!", 7777, UpgradeType.SPECIAL,
            (up) => { return feverData.TimeActive > 0; },
            () => { ++feverData.Gain; })
        };
    }
}