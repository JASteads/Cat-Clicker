using System.Collections.Generic;
using UnityEngine;

public class CLSystem : MonoBehaviour
{
    public CLSaveData data;

    CLInterface cInterface;
    List<CLUpgrade> upgradeDB;
    List<CLSubSys> subSystems;


    void Update()
    {
        subSystems.ForEach(s => s.onUpdate?.Invoke());
    }

    void FixedUpdate()
    {
        subSystems.ForEach(s => s.onFixedUpdate?.Invoke());
    }

    public void OnClick()
    {
        double totalCP = GetTotalCP();

        ++data.TotalClicks;
        subSystems.ForEach(s => s.onClick?.Invoke(totalCP));
        ApplyCP(totalCP);
    }

    public double GetBPS(bool isRaw)
    {
        return data.BitsPerSecond *
            (isRaw ? 1 : data.feverData.BPSMultiplier);
    }

    public void UpdateBPS()
    {
        data.BitsPerSecond = 0;
        data.buildingData.ForEach(b =>
            data.BitsPerSecond += b.BaseValue * b.Amount);
    }

    public double GetTotalCP()
    {
        double totalCP = data.ClickPower;

        subSystems.ForEach(s => s.onCalcCP?.Invoke(totalCP));

        return totalCP;
    }

    public int GetCurrencyCurrent()
    {
        return Mathf.RoundToInt((float)data.CurrencyCurrent);
    }
    public int GetCurrencyTotal()
    {
        return Mathf.RoundToInt((float)data.CurrencyTotal);
    }

    public void DeductBits(double amount)
    { data.CurrencyCurrent -= amount; }

    public void AssignTooltip(GameObject obj)
    {
        cInterface.tooltip.Assign(obj);
    }

    void ApplyCP(double totalCP)
    {
        data.CurrencyCurrent += totalCP;
        data.CurrencyTotal += totalCP;
    }

    public void Init(Transform parent)
    {
        data = SysManager.activeProfile.cl;
        cInterface = new CLInterface();
        cInterface.Init(this, parent);

        subSystems = new List<CLSubSys>()
        {
            new CLBuildingSys(this, parent),
            new CLClickSys(this, parent),
            new CLFeverSys(this, parent)
        };
        InitUpgradeDB();
    }

    void InitUpgradeDB()
    {
        upgradeDB = new List<CLUpgrade>
        {
            new CLUpgrade("Double Sprouts", "Doubles BPS gained from Bit Plants", 100, UpgradeType.BUILDING,
            (up) => { return data.buildingData[0].Amount > 0; },
            () => { data.buildingData[0].BaseValue *= 2; }),

            new CLUpgrade("Efficient Diggers", "Doubles BPS gained from Bit Diggers", 500, UpgradeType.BUILDING,
            (up) => { return data.buildingData[1].Amount > 0; },
            () => { data.buildingData[1].BaseValue *= 2; }),

            new CLUpgrade("Double Click", "Doubles your Click Power", 1000, UpgradeType.CP,
            (up) => { return data.buildingData[0].Amount >= 10; },
            () => { data.ClickPower *= 2; }),

            new CLUpgrade("Shiny Plants", "Add some polish to your Bit Plants. Oh, and double their BPS too.", 5000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[0].Amount >= 100; },
            () => { data.buildingData[0].BaseValue *= 2; }),

            new CLUpgrade("Green Thumb", "Click Power gains +0.1 for each Bit Plant owned", 25000, UpgradeType.CP,
            (up) => { return data.buildingData[0].AccumulativeValue >= 500; },
            () => { data.buildingData[0].ClickPowerAmplifier += 0.1f; }),

            new CLUpgrade("Precise Extraction", "Doubles BPS gained from Bit Extractors", 4000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[2].Amount > 0; },
            () => { data.buildingData[2].BaseValue *= 2; }),

            new CLUpgrade("New Fabrication Recipes", "Doubles BPS gained from Bit Fabricators", 20000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[3].Amount > 0; },
            () => { data.buildingData[3].BaseValue *= 2; }),

            new CLUpgrade("Greased Wheels", "Doubles BPS gained from Cat Wheels", 100000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[4].Amount > 0; },
            () => { data.buildingData[4].BaseValue *= 2; }),

            new CLUpgrade("Polished Gears", "Doubles BPS gained from Cat Treadmills", 500000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[5].Amount > 0; },
            () => { data.buildingData[5].BaseValue *= 2; }),

            new CLUpgrade("Use Medium Quality", "Doubles BPS gained from Cat VR", 1000000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[6].Amount > 0; },
            () => { data.buildingData[6].BaseValue *= 2; }),

            new CLUpgrade("Level Up", "Doubles BPS gained from Cat Simulators", 3000000, UpgradeType.BUILDING,
            (up) => { return data.buildingData[7].Amount > 0; },
            () => { data.buildingData[7].BaseValue *= 2; }),

            new CLUpgrade("Longer Fever", "Increases the time before fever drains by 50%", 10000, UpgradeType.SPECIAL,
            (up) => { return data.GetCurrencyTotal() >= 1000000; },
            () => { data.feverData.Persistence *= 1.5f; }),

            new CLUpgrade("Win the Game", "Kills the game; you win!", 1000000000, UpgradeType.SPECIAL,
            (up) => { return data.GetCurrencyTotal() >= 10000000; },
            () =>
            {
                SysManager.achieveSys.ForceUnlock("Beat the Game");
                SysManager.fileManager.FileSave();
                SysManager.QuitGame();
            }),

            new CLUpgrade("Fever Buff", "You've seen fever, now let's make it better: Increase fever progress per click by 1!", 7777, UpgradeType.SPECIAL,
            (up) => { return data.feverData.TimeActive > 0; },
            () => { ++data.feverData.Gain; })
        };
    }
}