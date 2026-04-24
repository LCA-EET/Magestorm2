using UnityEngine;
public class NonSolidWall : Wall, ITrigger
{
    protected int _triggerID;
    protected bool _entered, _exited;
    void Awake()
    {
        _triggerID = TriggerManager.RegisterTrigger(this);
    }
    
    public void EnterAction()
    {
        Debug.Log("Entered wall " + _castID);
        _entered = true;
    }

    public void ExitAction()
    {
        Debug.Log("Exited wall " + _castID);
        _exited = true;
    }

    public int GetTriggerID()
    {
        return _triggerID;
    }

    public bool HasEntered()
    {
        return _entered;
    }

    public bool HasExited()
    {
        return _exited;
    }
}
