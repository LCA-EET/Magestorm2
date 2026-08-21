using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public bool GetSurface(Transform origin, out Surface surface)
    {
        RaycastHit hitInfo;
        if(CastDownward(LayerManager.FloorMask, 1.0f, out hitInfo))
        {
            surface = hitInfo.collider.GetComponent<Surface>();
            return true;
        }
        else
        {
            surface = null;
            return false;
        }
    }
    public bool CastDownward(int mask, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.CastDown(transform, LayerManager.FloorMask, distance, out hitInfo);
    }
    public bool CastDownward(int mask, float distance)
    {
        RaycastHit hitInfo;
        return SharedFunctions.CastDown(transform, LayerManager.FloorMask, distance, out hitInfo);
    }
    public bool CastForward(int mask, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.CastForward(transform, mask, distance, out hitInfo);
    }
    public bool SphereCastForward(int mask, float radius, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.SphereCastForward(transform, mask, radius, distance, out hitInfo);
    }
    public static bool CameraCastForward(int mask, float distance, out RaycastHit hitInfo)
    {
        return SharedFunctions.CastForward(Camera.main.transform, mask, distance, out hitInfo);
    }
}
