using UnityEngine;

public static class LayerManager
{
    private static int _surfaceMask;
    private static int _interactableMask;
    public static void Init()
    {
        _surfaceMask = LayerMask.GetMask(new string[] { "Surface" });
        _interactableMask = LayerMask.GetMask(new string[] { "Interactable" });
    }

    public static int SurfaceMask
    {
        get{
            return _surfaceMask;
        }
    }
    public static int InteractableMask
    {
        get
        {
            return _interactableMask;
        }
    }
}
