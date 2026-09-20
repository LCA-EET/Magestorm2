using UnityEngine;
public class OOB : MonoBehaviour, ITrigger
{
    public void EnterAction()
    {
        ComponentRegister.Valhalla.EnterValhalla();
    }

    public void ExitAction()
    {
        return;
    }

    public int GetTriggerID()
    {
        return 0 ;
    }

    public bool HasEntered()
    {
        return false;
    }

    public bool HasExited()
    {
        return false;
    }

}
