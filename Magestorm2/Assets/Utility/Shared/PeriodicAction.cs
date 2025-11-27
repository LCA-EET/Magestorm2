using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class PeriodicAction
{
    private float _elapsed;
    private float _interval;

    public delegate void ActionDelegate();

    private Action _action;

    public PeriodicAction(float interval, Action toPerform, List<PeriodicAction> owner)
    {
        _interval = interval;
        _action = toPerform;
        if (owner != null)
        {
            owner.Add(this);
        }
    }

    public void ProcessAction(float deltaTime)
    {
        _elapsed += deltaTime;
        if (_elapsed >= _interval)
        {
            _elapsed -= _interval;
            _action();
        }
    }
    public float PercentComplete
    {
        get
        {
            return _elapsed / _interval;
        }
    }
    public static void PerformActions(float elapsed, List<PeriodicAction> toPerform)
    {
        foreach(PeriodicAction pa in toPerform)
        {
            pa.ProcessAction(elapsed);
        }
    }
}
