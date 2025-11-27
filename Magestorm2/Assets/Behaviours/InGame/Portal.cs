using UnityEngine;

public class Portal : Trigger
{
    public GameObject[] Exits;
    
    public override void EnterAction()
    {
        if (ComponentRegister.PC.IsAlive)
        {
            int exitID = Random.Range(0, Exits.Length);
            GameObject selectedExit = Exits[exitID];
            ComponentRegister.PC.transform.position = selectedExit.transform.position;
            ComponentRegister.PC.transform.eulerAngles = selectedExit.transform.eulerAngles;
        }
    }

}
