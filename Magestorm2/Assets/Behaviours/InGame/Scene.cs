using UnityEngine;
public class Scene : MonoBehaviour
{
    private Transform[] _sceneEntryPoints;
    public GameObject SceneEntryGO;
    private void Awake()
    {
        ComponentRegister.Scene = this;
        _sceneEntryPoints = SceneEntryGO.GetComponentsInChildren<Transform>();
    }

    public void AssignEntryPoint(PC pc)
    {
        if(_sceneEntryPoints.Length > 0)
        {
            Transform toUse = _sceneEntryPoints[SharedFunctions.RandomInt(0, _sceneEntryPoints.Length)];
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
