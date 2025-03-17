using UnityEngine;

public static class LayerManager
{
    private static int _surfaceLayer;
    public static void Init()
    {
        _surfaceLayer = LayerMask.GetMask(new string[] { "Surface" });
    }

    public static int SurfaceMask
    {
        get{
            return _surfaceLayer;
        }
    }
}
