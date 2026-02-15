using UnityEngine;

public static class LayerManager
{
    public const string Layer_Player = "Player";
    public const string Layer_DeadPlayer = "DeadPlayer";
    public const string Layer_RemotePlayer = "RemotePlayer";

    private static int _playerLayer;
    private static int _remotePlayerLayer;
    private static int _deadPlayerLayer;
    private static int _surfaceMask;
    private static int _interactableMask;
    private static int _projectileImpactMask;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _playerLayer = LayerMask.NameToLayer(Layer_Player);
            _deadPlayerLayer = LayerMask.NameToLayer(Layer_DeadPlayer);
            _remotePlayerLayer = LayerMask.NameToLayer(Layer_RemotePlayer);
            _surfaceMask = LayerMask.GetMask("Surface");
            _interactableMask = LayerMask.GetMask("Interactable");
            _projectileImpactMask = LayerMask.GetMask(new string[] { "Surface", "Default", "RemotePlayer", "Interactable" });
           
            _init = true;
        }
    }
    public static int ProjectileImpactMask
    {
        get { return _projectileImpactMask; }
    }
    public static int PlayerLayer
    {
        
        get { return _playerLayer; }
    }
    public static int DeadPlayerLayer
    {
        get { return _deadPlayerLayer; }
    }
    public static int RemotePlayerLayer
    {
        get { return _remotePlayerLayer; }
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
