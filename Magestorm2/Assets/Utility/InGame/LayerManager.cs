using UnityEngine;

public static class LayerManager
{
    private static int _surfaceMask;
    private static int _interactableMask;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _surfaceMask = LayerMask.GetMask(new string[] { "Surface" });
            _interactableMask = LayerMask.GetMask(new string[] { "Interactable" });
            _init = true;
        }
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
