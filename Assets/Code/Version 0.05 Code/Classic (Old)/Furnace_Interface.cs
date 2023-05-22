using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Furnace_Item
{
    // Type : Resource = 0 ; Component = 1 ; Subcore = 2 ; Core = 3

    public int id, price, type;
    public FItem_Bonus bonus;
    public string name, desc;
    public Image icon;

    public Furnace_Item(int id, int price, FItem_Bonus bonus, int type, string name, string desc)
    {
        this.id = id;
        this.price = price;
        this.bonus = bonus;
        this.type = type;
        this.name = name;
        this.desc = desc;
        icon = null;
    }
}

public struct FItem_Bonus
{
    public float cp, fcp, fbps, ebgain;

    public FItem_Bonus(int val)
    {
        cp = fcp = fbps = ebgain = val;
    }

    public FItem_Bonus(float cp, float fcp, float fbps, float ebgain)
    {
        this.cp = cp;
        this.fcp = fcp;
        this.fbps = fbps;
        this.ebgain = ebgain;
    }
}


public class Furnace_Interface : MonoBehaviour
{
    GameObject frame_canvas, shop_canvas, inv_canvas;
    Furnace_Item[] f_items;
    public List<int> inventory = new List<int>();
    public int ether_bits, ebit_tick;
    public static bool ether_fever_on = false;

    void Init_Furnace_Items()
    {
        f_items = new Furnace_Item[5];

        f_items[0] = new Furnace_Item(0, 100, new FItem_Bonus(0), 0, "Red Coal", "A basic necessity for crating materials from the Furnace. Not very useful on its own");
        f_items[1] = new Furnace_Item(1, 200, new FItem_Bonus(0), 0, "Red Ingot", "Another basic necessiry for crafting materials from the Furnace.");
        f_items[2] = new Furnace_Item(2, 500, new FItem_Bonus(0), 0, "Super-Compressed Ether Bit", "Smashing a bunch of Ether bits together makes one really powerful bit!");
        f_items[3] = new Furnace_Item(3, 2000, new FItem_Bonus(0, 0, .3f, 0), 1, "Fuel", "The stuff any machine needs for power.");
        f_items[4] = new Furnace_Item(4, 5000, new FItem_Bonus(0), 2, "Burner", "Placeholder subcore");
        f_items[5] = new Furnace_Item(5, 10000, new FItem_Bonus(1, 2, 1, 0), 3, "Blast Core", "The most basic core, but still incredibly powerful!");
    }

    void Start()
    {
        frame_canvas = UI_Tool.CanvasSetup("Frame Canvas", transform);
        shop_canvas = UI_Tool.CanvasSetup("Shop Canvas", transform);
        inv_canvas = UI_Tool.CanvasSetup("Inventory Canvas", transform);
    }

    void FixedUpdate()
    {
        /*
        if (fever_system.bars_filled == 8 && fever_system.active)
        { if (++ebit_tick == 59) { ++ether_bits; ebit_tick = 0; } ether_fever_on = true; }
        else { ebit_tick = 0; ether_fever_on = false; }
        */
    }

    
}
