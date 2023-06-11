public class CLUpgrade
{
    public delegate bool Predicate(CLSaveData cl);
    public delegate void Effect();

    public string Name { get; }
    public string Desc { get; }
    public int Price { get; }

    public UpgradeType Type { get; }
    public Status Status { get; set; }

    readonly Predicate predicate;
    Effect Buy { get; }

    public CLUpgradeData[] Parents { get; }
    public CLUpgradeData[] Children { get; }
    public bool AnyParent { get; }

    public CLUpgrade(string name, string desc,
        int price, UpgradeType type, Predicate pred, Effect onBuy)
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
        SysManager.activeProfile.cl.CurrencyCurrent -= Price;
        Status = Status.PURCHASED;
        Buy();
    }

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

            return predicate(SysManager.activeProfile.cl);
        }

        return false;
    }
}

public enum Status
{
    LOCKED,
    UNLOCKED,
    PURCHASED
}

public enum UpgradeType
{
    BUILDING,
    CP,
    SPECIAL
}