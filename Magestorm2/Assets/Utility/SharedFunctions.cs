using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class SharedFunctions
{
    public static bool CastDown(Transform origin, int layerMask, float distance, out RaycastHit hitInfo)
    {
        return Physics.Raycast(origin.position, origin.TransformDirection(Vector3.down), out hitInfo, distance);
    }
    public static bool CastDown(Transform origin, int layerMask, float distance)
    {
        RaycastHit hitInfo;
        return CastDown(origin, layerMask, distance, out hitInfo);
    }
}
