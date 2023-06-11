using UnityEngine;

public class CLFeverSys : CLSubSys
{
    const float CONSTANT_DRAIN = 0.04f, LIMIT = 0.95f;
    const int BASE_DURATION = 45;

    float current, max, gain;
    int duration, persist;
    bool isActive;


    public CLFeverSys(CLSystem sys, Transform parent)
        : base(sys, parent)
    {
        
    }

    protected override void Init(CLSystem sys, Transform parent)
    {
        current = duration = 0;
        max = 100;
        gain = 8;
        persist = 1;

        onClick = (up) => Fuel();
        onCalcCP = (cp) => cp *= GetFeverBonus(sys);

        isActive = false;
        onFixedUpdate = CheckSystem;

        panel = new CLFeverPanel(sys, parent);
        ((CLFeverPanel)panel).limit = LIMIT;
    }

    public void Fuel()
    {
        current += gain;
        if (current >= max) current = max;
        ResetDuration(); 
    }

    public float GetFeverBonus(CLSystem sys)
    {
        return isActive ? 1 : sys.data.feverData.ClickMultiplier;
    }

    public bool IsFever()
    {
        return isActive;
    }

    void CheckSystem()
    {
        float fPercent = current / max;
        isActive = fPercent >= 1 || (isActive && fPercent >= LIMIT);

        if (duration > 0)
        {
            current -= CONSTANT_DRAIN;
            --duration;
        }
        else if (current > 0) current -= 1; // Replace w Drain amount
        else current = 0;

        ((CLFeverPanel)panel).fuelPercent = fPercent;
        ((CLFeverPanel)panel).isActive = isActive;
        panel.onRefresh();
    }

    void ResetDuration()
    {
        duration = BASE_DURATION * persist;
    }
}
