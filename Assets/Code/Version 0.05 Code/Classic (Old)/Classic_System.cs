using UnityEngine;

using static Database;

public class Classic_System : MonoBehaviour
{
    public int update_tick = 0, tick_second = 0;

    public Buildings_System buildings_system = new Buildings_System();
    public Upgrades_System upgrades_system = new Upgrades_System();
    public Fever_System fever_system = new Fever_System(0);
    public Classic_Interface c_interface;

    public void Click()
    {
        ++data.total_clicks;
        double click_power_base = 0;

        for (int i = 0; i < data.b_data.Length; i++)
            click_power_base += data.b_data[i].bps * data.b_data[i].click_power_amplifier * data.b_data[i].count;
        
        foreach (Buildings_System.Building_Data b in data.b_data)
            if (b.cp_amp_toggle) click_power_base += b.count * b.click_power_amplifier;

        double click_power_final = click_power_base + (data.click_power * (fever_system.active ? data.fever_data.click_multi : 1)); // Apply fever multi

        fever_system.Fuel_Fever();
        double ether_multi = 1;
        for (int i = 0; i < fever_system.bars_filled; i++)
            ether_multi *= 1.3f;

        click_power_final *= System.Math.Round(ether_multi);

        data.currency += click_power_final;
        data.total_money += click_power_final;

        c_interface.Status_Update(c_interface.click_popups, $"+ {click_power_final.ToString("#,0.#")} bits", Status_Msg.Type.News);
    }
    public void Update_BPS()
    {
        data.bps = 0;
        for (int i = 0; i < data.b_data.Length; i++)
            data.bps += data.b_data[i].bps * data.b_data[i].count;
    }

    public void Purchase_Building(Building_Button shop_item)
    {
        if (data.currency + 0.5f >= data.b_data[shop_item.id].price)
        {
            data.currency -= data.b_data[shop_item.id].price;
            ++data.b_data[shop_item.id].count;
            data.b_data[shop_item.id].price = (data.b_data[shop_item.id].price * 1.15f) + 0.5f;

            // Update building text
            shop_item.price.text = "$ " + To_Bit_Notation(data.b_data[shop_item.id].price, "#,0");
            shop_item.count.text = To_Bit_Notation(data.b_data[shop_item.id].count, "#,0");

            Update_BPS();
        }
        else
            c_interface.Status_Update(c_interface.status_msgs, $"You can't afford to purchase {buildings_system.buildings[shop_item.id].name} ...", Status_Msg.Type.Warning);
    }
    public void Purchase_Upgrade(Upgrade_Button upgrade)
    {
        // If currency (rounded up) meets the required price, activate upgrade.
        if (data.currency + 0.5f >= upgrades_system.upgrades[upgrade.id].price)
        {
            upgrades_system.Activate_Upgrade(upgrades_system.upgrades, upgrade.id);
            c_interface.Refresh_Upgrades(c_interface.active_tab, false);

            Update_BPS();
            c_interface.Status_Update(c_interface.status_msgs, $"You've purchased {upgrades_system.upgrades[upgrade.id].title}!", Status_Msg.Type.News);
        }
        c_interface.Hide_Tooltip();
    }
    
    void Start()
    {
        data.u_statuses = new int[upgrades_system.upgrades.Length];
        c_interface = gameObject.AddComponent<Classic_Interface>();
    }
    void Update()
    {
        // Stop FEVER if below 95%.
        if (fever_system.active && fever_system.val <= 95 && fever_system.bars_filled < 1)
            fever_system.active = false;
    }
    void FixedUpdate()
    {
        // If Fever is not kept active, start draining
        if (fever_system.val > 0)
        {
            fever_system.val -= 0.04f;
            if (fever_system.duration <= 0)
            {
                if (fever_system.bars_filled == 0)
                {
                    if (fever_system.val - data.fever_data.drain > 0)
                        fever_system.val -= data.fever_data.drain;
                    else
                        fever_system.val = 0;
                }
                else
                {
                    if (fever_system.val - data.fever_data.drain > 0)
                        fever_system.val -= data.fever_data.drain;
                    else
                    {
                        --fever_system.bars_filled;
                        fever_system.val = fever_system.max - 1;
                    }
                }
            }
        }

        if (fever_system.duration > 0) --fever_system.duration;

        if (++update_tick == 1) // 30 updates/sec ; Multiply increments by 0.02 to match update rate
        {

            double update_multi = 0.02f;

            for (int i = 0; i < data.b_data.Length; i++)
                data.b_data[i].value += data.b_data[i].bps * data.b_data[i].count * update_multi;

            double gains = data.bps * update_multi;
            if (fever_system.active)
            {
                gains *= data.fever_data.bps_multi;
                ++data.fever_data.time_active;
            }

            data.currency += gains;
            data.total_money += gains;

            update_tick = 0;
        }

        if (++tick_second == 59)
        {
            ++data.time_played; tick_second = 0;
            Achievements_Data.Check_Achievements();
        }
    }
}