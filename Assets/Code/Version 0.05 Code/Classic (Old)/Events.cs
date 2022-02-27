using UnityEngine;

public class Events
{
    public delegate void EventHandler<T>(T e);

    public struct Click_Event
    {
        double click_power_base, click_power_final;
        Vector2 mouse_pos;

        public Click_Event(double cb, double cf, Vector2 mouse_pos)
        {
            click_power_base = cb;
            click_power_final = cf;
            this.mouse_pos = mouse_pos;
        }
    }
    public static event EventHandler<Click_Event> OnClick;
    public static void PerformClick(Click_Event e)
    {
        OnClick?.Invoke(e);
    }
}
