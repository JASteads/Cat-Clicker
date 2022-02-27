public class Fever_System
{
    // public delegate void On_Fever();

    public int mode; // 0 = Normal ; 1 = Furnace ; 2 = Angelicat ; 3 = Evolution
    public int max_bars, bars_filled;
    public float val, duration, max;
    public bool active;

    /* Data saved across sessions */
    public struct Fever_Data
    {
        public float gain, drain, persistence, click_multi, bps_multi;
        public long time_active;

        public Fever_Data(int mode, float gain, float drain, float persistence, long time_active)
        {
            click_multi = 2;
            bps_multi = 1.5f;

            this.gain = gain;
            this.drain = drain;
            this.persistence = persistence;
            this.time_active = time_active;
        }
    }

    public void Fuel_Fever()
    {
        val += Database.data.fever_data.gain; // * (float)System.Math.Pow(1.1f, bars_filled);

        if (val > max)
        {
            if (!active) active = true;
            if (bars_filled < max_bars && max_bars > 1)
            {
                ++bars_filled; val = 0;
            }
            else
                val = max;
        }

        // Reset fever drain time
        duration = 15 * Database.data.fever_data.persistence;
    }

    public Fever_System(int mode)
    {
        this.mode = mode;
        max_bars = 1;
        bars_filled = 0;

        val = duration = 0;
        max = 100;

        active = false;
    }
}