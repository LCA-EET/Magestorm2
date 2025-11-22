using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    protected List<PeriodicAction> _actionList;
    protected TriggerType _triggerType;

    protected virtual void InitTrigger(TriggerType triggerType)
    {
        _actionList = new List<PeriodicAction>();
        _triggerType = triggerType;
    }
    public TriggerType TriggerType
    {
        get { return _triggerType; }
    }
    public virtual void EnterAction(){
       
    }
    public virtual void ExitAction() { }
}

