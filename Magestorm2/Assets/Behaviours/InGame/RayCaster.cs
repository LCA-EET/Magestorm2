using UnityEditor.PackageManager;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public bool CastForward(int mask, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.CastForward(transform, mask, distance, out hitInfo);
    }

    public static bool CameraCastForward(int mask, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.CastForward(Camera.main.transform, mask, distance, out hitInfo);
    }
}
