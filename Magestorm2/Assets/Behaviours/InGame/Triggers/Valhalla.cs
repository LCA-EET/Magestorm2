using UnityEngine;

public class Valhalla : Trigger
{
    public GameObject[] EntryPoints;

    protected override void Awake()
    {
        base.Awake();
        ComponentRegister.Valhalla = this;
    }

    public void EnterValhalla()
    {
        Debug.Log("PC has entered Valhalla.");
        byte teamID = MatchParams.MatchTeamID;
        GameObject entry = EntryPoints[teamID];
        ComponentRegister.PC.UpdatePosition(entry.transform.position);
        ComponentRegister.PC.transform.localEulerAngles = entry.transform.localEulerAngles;
    }

    public override void EnterAction()
    {
        base.EnterAction();
        ComponentRegister.PC.InValhalla = true;
        ComponentRegister.ValhallaNotice.Show(true);
    }

    public override void ExitAction()
    {
        base.ExitAction();
        ComponentRegister.PC.InValhalla = false;
        ComponentRegister.ValhallaNotice.Show(false);
    }
}
