using System.Collections.Generic;

public class Upgrades_System
{
    public delegate bool Predicate(Upgrade up);
    public delegate void Effect();
    public UI_Tool ui_tool = new UI_Tool();
    public Upgrade[] upgrades;

    public struct Upgrade
    {
        public enum Type
        { CP, BUILDING, CPS, SPECIAL };

        public double price;
        public int id, status; // Status : 0 = Purhcased ; 1 = Unlocked ; 2 = Locked
        public Type type;
        public bool anyParent;
        public string title, desc;
        public Upgrade[] parents, children;
        public Predicate pred;
        public Effect buy;

        public static int nextId = 100; // Allows for dynamic ID assignment

        public Upgrade(string t, string d, double price, Type type, bool anyP, Upgrade[] p, Upgrade[] c, Predicate pd, Effect effect)
        {
            id = nextId++;
            status = 2;
            title = t;
            desc = d;
            this.price = price;
            this.type = type;
            anyParent = anyP;
            parents = p;
            children = c;
            pred = pd;
            buy = effect;
        }
        public Upgrade(string t, string d, double price, Type type, Predicate pd, Effect effect)
        {
            id = nextId++;
            status = 2;
            title = t;
            desc = d;
            this.price = price;
            this.type = type;
            anyParent = true;
            parents = null;
            children = null;
            pred = pd;
            buy = effect;
        }
    }

    public void Init_Upgrades()
    {
        List<Upgrade> upgrade_list = new List<Upgrade>
        {
            new Upgrade("Double Sprouts", "Doubles BPS gained from Bit Plants", 100, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[0].count > 0; },
            () => { Database.data.b_data[0].bps *= 2; }),

            new Upgrade("Efficient Diggers", "Doubles BPS gained from Bit Diggers", 500, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[1].count > 0; },
            () => { Database.data.b_data[1].bps *= 2; }),

            new Upgrade("Double Click", "Doubles your Click Power", 1000, Upgrade.Type.CP,
            (up) => { return Database.data.b_data[0].count >= 10; },
            () => { Database.data.click_power *= 2; }),

            new Upgrade("Shiny Plants", "Add some polish to your Bit Plants. Oh, and double their BPS too.", 5000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[0].count >= 100; },
            () => { Database.data.b_data[0].bps *= 2; }),

            new Upgrade("Green Thumb", "Click Power gains +0.1 for each Bit Plant owned", 25000, Upgrade.Type.CP,
            (up) => { return Database.data.b_data[0].value >= 500; },
            () => { Database.data.b_data[0].click_power_amplifier += 0.1f; }),

            new Upgrade("Precise Extraction", "Doubles BPS gained from Bit Extractors", 4000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[2].count > 0; },
            () => { Database.data.b_data[2].bps *= 2; }),

            new Upgrade("New Fabrication Recipes", "Doubles BPS gained from Bit Fabricators", 20000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[3].count > 0; },
            () => { Database.data.b_data[3].bps *= 2; }),

            new Upgrade("Greased Wheels", "Doubles BPS gained from Cat Wheels", 100000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[4].count > 0; },
            () => { Database.data.b_data[4].bps *= 2; }),

            new Upgrade("Polished Gears", "Doubles BPS gained from Cat Treadmills", 500000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[5].count > 0; },
            () => { Database.data.b_data[5].bps *= 2; }),

            new Upgrade("Use Medium Quality", "Doubles BPS gained from Cat VR", 1000000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[6].count > 0; },
            () => { Database.data.b_data[6].bps *= 2; }),

            new Upgrade("Level Up", "Doubles BPS gained from Cat Simulators", 3000000, Upgrade.Type.BUILDING,
            (up) => { return Database.data.b_data[7].count > 0; },
            () => { Database.data.b_data[7].bps *= 2; }),

            new Upgrade("Longer Fever", "Increases the time before fever drains by 50%", 10000, Upgrade.Type.SPECIAL,
            (up) => { return Database.data.total_money >= 1000000; },
            () => { Database.data.fever_data.persistence *= 1.5f; }),

            new Upgrade("Win the Game", "Kills the game; you win!", 1000000000, Upgrade.Type.SPECIAL,
            (up) => { return Database.data.total_money >= 10000000; },
            () =>
            {
                Achievements_Data.Activate_Achievement(Achievements_Data.achievements[4]);
                File_Manager.File_Save();
                Database.Quit_Game();
            }),

            new Upgrade("Fever Buff", "You've seen fever, now let's make it better: Increase fever progress per click by 1!", 7777, Upgrade.Type.SPECIAL,
            (up) => { return Achievements_Data.achievements[3].status == 0; },
            () => { ++Database.data.fever_data.gain; })
        };

        upgrades = upgrade_list.ToArray();
    }

    public bool Check_Upgrades()
    {
        for (int i = 0; i < upgrades.Length; i++)
            if (upgrades[i].status == 2 && Check_Prereq(upgrades[i]))
            {
                upgrades[i].status = 1;
                return true;
            }
        return false;
    }

    /* Check_Prereq() : Checks prerequisite for upgrade. */
    public bool Check_Prereq(Upgrade up)
    {
        if (up.pred != null)
        {
            if (up.parents != null)
            {
                bool found = false;
                for (int i = 0; i < up.parents.Length && !found; i++)
                    if (up.parents[i].status != 0)
                        if (!up.anyParent) return false;
                        else if (up.anyParent) found = true;
                if (up.anyParent && !found) return false;
            }
            return up.pred(up);
        }
        return false;
    }

    public void Activate_Upgrade(Upgrade[] up_arr, int id)
    {
        up_arr[id].buy();
        up_arr[id].status = 0;
        Database.data.currency -= up_arr[id].price;
    }

    public Upgrades_System()
    { Init_Upgrades(); }
}