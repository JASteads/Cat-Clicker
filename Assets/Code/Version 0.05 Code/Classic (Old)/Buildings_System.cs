public class Buildings_System
{
    public Building[] buildings = new Building[12];

    public struct Building
    {
        static int nextId = 0;
        public int id;
        public string name, desc;

        public Building(string name, string desc, double base_bps, double base_price)
        {
            id = nextId++;
            this.name = name;
            this.desc = desc;
        }
    }
    public struct Building_Data
    {
        public static int nextId = 0;

        public int count, id;
        public double price, value, click_power_amplifier, bps;
        public bool cp_amp_toggle, available;

        public Building_Data(int count, double price, double value, double click_power_amplifier, double bps)
        {
            id = nextId++;
            this.count = count;
            this.value = value;
            this.price = price;
            this.click_power_amplifier = click_power_amplifier;
            this.bps = bps;
            cp_amp_toggle = available = false;
        }
        public Building_Data(int count, double price, double value, double click_power_amplifier, double bps, bool cp_amp_toggle, bool available)
        {
            id = nextId++;
            this.count = count;
            this.value = value;
            this.price = price;
            this.click_power_amplifier = click_power_amplifier;
            this.bps = bps;
            this.cp_amp_toggle = cp_amp_toggle;
            this.available = available;
        }
    }

    public void Init_Buildings()
    {
        // Basic Generators
        buildings[0] = new Building("Generator", "A plant that blossoms into a bundle of bits. Takes time to grow.", 0.1f, 15);
        buildings[1] = new Building("Planter", "Digs into the soil to find bits. May take some extraction.", 1, 85);
        buildings[2] = new Building("Digger", "Extracts bits from everyday items. You really can find bits anywhere.", 6, 700);
        buildings[3] = new Building("Fabricator", "Turns everyday items into bits. Careful what you wish for.", 20, 5000);

        // Cat-driven Generators
        buildings[4] = new Building("Wheel", "A wheel that converts energy into bits. Cats love walks, you know.", 100, 25000);
        buildings[5] = new Building("Treadmill", "A treadmill that converts energy into bits. It's like the wheel, but forces the cat to run instead.", 500, 200000);
        buildings[6] = new Building("Game System", "Virtual Reality that converts energy into bits. Let's take everything the cat does and makes bits out of that.", 1500, 420690);
        buildings[7] = new Building("VR", "A simulation of cats that converts their activities into bits. They're basically competitive gamer cats now.", 3400, 1431100);

        // Hybrid Generators
        buildings[8] = new Building("House Cat", "", 10000, 5000000);
        buildings[9] = new Building("Farmer Cat", "", 25000, 20000000);
        buildings[10] = new Building("Miner Cat", "A cat that digs into soil to find bits. These cats are cunning, so they work faster than your diggers", 500000, 100000000);
        buildings[11] = new Building("Science Cat", "A cat that researches bit extraction. They think they're smarter than you, but little do they know.", 1000000, 500000000);
    }

    public Buildings_System()
    {
        Init_Buildings();
    }
}
