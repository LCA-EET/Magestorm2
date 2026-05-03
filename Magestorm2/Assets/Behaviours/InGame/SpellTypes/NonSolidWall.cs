using UnityEngine;
public class NonSolidWall : Wall, ITrigger
{
    protected int _triggerID;
    protected bool _entered, _exited;
    public virtual void Awake()
    {
        _triggerID = TriggerManager.RegisterTrigger(this);
    }
    
    public void EnterAction()
    {
        Debug.Log("Entered wall " + _castID);
        ComponentRegister.PlayerMovement.IncrementInsideWallCount();
        _entered = true;
        _exited = false;
    }

    public void ExitAction()
    {
        Debug.Log("Exited wall " + _castID);
        ComponentRegister.PlayerMovement.DecrementInsideWallCount();
        _entered = false;
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
