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
    public List<UpgradeData> upgradesData;
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
            new BuildingData("Generator", "A plant that blossoms into a bundle of bits. Takes time to grow.", 0.1f, 15),
            new BuildingData("Planter", "Digs into the soil to find bits. May take some extraction.", 1, 85),
            new BuildingData("Digger", "Extracts bits from everyday items. You really can find bits anywhere.", 6, 700),
            new BuildingData("Fabricator", "Turns everyday items into bits. Careful what you wish for.", 20, 5000),

            // Cat-driven Generators
            new BuildingData("Wheel", "A wheel that converts energy into bits. Cats love walks, you know.", 100, 25000),
            new BuildingData("Treadmill", "A treadmill that converts energy into bits. It's like the wheel, but forces the cat to run instead.", 500, 200000),
            new BuildingData("Game System", "Virtual Reality that converts energy into bits. Let's take everything the cat does and makes bits out of that.", 1500, 420690),
            new BuildingData("VR", "A simulation of cats that converts their activities into bits. They're basically competitive gamer cats now.", 3400, 1431100),

            // Hybrid Generators
            new BuildingData("House Cat", "", 10000, 5000000),
            new BuildingData("Farmer Cat", "", 25000, 20000000),
            new BuildingData("Miner Cat", "A cat that digs into soil to find bits. These cats are cunning, so they work faster than your diggers", 500000, 100000000),
            new BuildingData("Science Cat", "A cat that researches bit extraction. They think they're smarter than you, but little do they know.", 1000000, 500000000)
        };
    }

    List<UpgradeData> CreateStandardUpgrades()
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
                GameManagement.profile.achievements.data.Find(achievement => achievement.Title == "Beat the Game").Unlock();
                GameManagement.fileManager.FileSave();
                GameManagement.QuitGame();
            }),

            new UpgradeData("Fever Buff", "You've seen fever, now let's make it better: Increase fever progress per click by 1!", 7777, UpgradeType.SPECIAL,
            (up) => { return feverData.TimeActive > 0; },
            () => { ++feverData.Gain; })
        };
    }
}