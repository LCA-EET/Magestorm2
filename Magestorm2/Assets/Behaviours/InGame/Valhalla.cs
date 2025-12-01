using UnityEngine;

public class Valhalla : MonoBehaviour
{
    public GameObject[] EntryPoints;

    public void Awake()
    {
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
}
