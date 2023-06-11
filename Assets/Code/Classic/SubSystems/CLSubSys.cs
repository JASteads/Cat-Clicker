using System;
using UnityEngine;

public abstract class CLSubSys
{
    public Action onUpdate, onFixedUpdate;
    public Action<double> onClick, onCalcCP;
    protected CLSubPanel panel;

    public CLSubSys(CLSystem sys, Transform parent)
    {
        onUpdate = onFixedUpdate = null;
        onClick = onCalcCP = null;
        Init(sys, parent);
    }

    protected abstract void Init(CLSystem sys, Transform parent);
}