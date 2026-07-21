using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour, ITrigger
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
        Debug.Log("Entered trigger " + gameObject.name);
    }
    public virtual void ExitAction() 
    {
        _entered = false;
        _exited = true;
        Debug.Log("Exited trigger " + gameObject.name);
    }
    public bool HasEntered()
    {
        return _entered;
    }
    public bool HasExited()
    {
        return _exited;
    }
    public int GetTriggerID()
    {
        return _triggerID;
    }
}

