[System.Serializable]
public class UpgradeData
{
    public delegate bool Predicate(CLSCSaveData clscSaveData);
    public delegate void Effect();

    public string Name { get; }
    public string Desc { get; }
    public int Price { get; }
    
    public UpgradeType Type { get; }
    public Status Status { get; set; }

    readonly Predicate predicate;
    Effect Buy { get; }

    public UpgradeData[] Parents { get; }
    public UpgradeData[] Children { get; }
    public bool AnyParent { get; }

    
    
    public UpgradeData(string name, string desc, int price, UpgradeType type, Predicate pred, Effect onBuy,
        UpgradeData[] parents, UpgradeData[] children, bool anyParent)
    {
        Name = name;
        Desc = desc;
        Price = price;
        
        Type = type;
        Buy = onBuy;
        predicate = pred;

        Parents = parents;
        Children = children;
        AnyParent = anyParent;

        Status = Status.LOCKED;
    }
    public UpgradeData(string name, string desc, int price, UpgradeType type, Predicate pred, Effect onBuy)
    {
        Name = name;
        Desc = desc;
        Price = price;

        Type = type;
        Buy = onBuy;
        predicate = pred;

        Parents = null;
        Children = null;
        AnyParent = true;

        Status = Status.LOCKED;
    }

    public void Unlock()
    {
        Status = Status.UNLOCKED;
    }

    public void Purchase()
    {
        GameManagement.profile.clscSaveData.CurrencyCurrent -= Price;
        Status = Status.PURCHASED;
        Buy();
    }

    /* Check_Prereq() : Checks prerequisite for upgrade. */
    public bool CheckRequirement()
    {
        if (predicate != null)
        {
            if (Parents != null)
            {
                bool found = false;

                for (int i = 0; i < Parents.Length && !found; i++)
                {
                    if (Parents[i].Status != Status.PURCHASED)
                    {
                        if (!AnyParent)
                            return false;
                        else if (AnyParent)
                            found = true;
                    }
                        
                    if (AnyParent && !found)
                        return false;
                }
            }

            return predicate(GameManagement.profile.clscSaveData);
        }

        return false;
    }
}

public enum Status
{
    LOCKED,
    UNLOCKED,
    PURCHASED
};

public enum UpgradeType
{
    CP,
    BUILDING,
    CPS,
    SPECIAL,
    NONE
};