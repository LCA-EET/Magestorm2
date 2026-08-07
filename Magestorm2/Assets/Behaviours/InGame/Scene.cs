using UnityEngine;
public class Scene : MonoBehaviour
{
    public Transform[] SceneEntryPoints;
    private void Awake()
    {
        ComponentRegister.Scene = this;   
    }
    public void AssignEntryPoint(PC pc)
    {
        if(SceneEntryPoints.Length > 0)
        {
            Transform toUse = SceneEntryPoints[SharedFunctions.RandomInt(0, SceneEntryPoints.Length)];
            if(toUse != null)
            {
                pc.UpdatePosition(toUse.position);
                pc.transform.localEulerAngles = toUse.localEulerAngles;
                return;
            }
        }
       ZeroEntry(pc);
    }
    private void ZeroEntry(PC pc)
    {
        pc.UpdatePosition(Vector3.zero);
        pc.transform.position = Vector3.zero;
    }
}
