using UnityEngine;

public static class LayerManager
{
    private static int _playerLayer;
    private static int _surfaceMask;
    private static int _interactableMask;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _playerLayer = LayerMask.NameToLayer("Player");
            _surfaceMask = LayerMask.GetMask("Surface");
            _interactableMask = LayerMask.GetMask("Interactable");
            _init = true;
        }
    }
    public static int PlayerLayer
    {
        
        get { return _playerLayer; }
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
