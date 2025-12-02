using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    protected List<PeriodicAction> _actionList;
    protected TriggerType _triggerType;
    protected bool _entered, _exited;
    protected int _triggerID;
    protected virtual void Awake()
    {
        _triggerID = TriggerManager.RegisterTrigger(this);
    }
    protected virtual void InitTrigger(TriggerType triggerType)
    {
        _actionList = new List<PeriodicAction>();
        _triggerType = triggerType;
    }
    public TriggerType TriggerType
    {
        get { return _triggerType; }
    }
    public virtual void EnterAction()
    {
        _entered = true;
        _exited = false;
    }
    public virtual void ExitAction() 
    {
        _exited = false;
        _exited = true;
    }
    public int TriggerID
    {
        get { return _triggerID; }
    }
    public bool Entered
    {
        get { return _entered; }
    }
    public bool Exited
    {
        get { return _exited; }
    }
}

