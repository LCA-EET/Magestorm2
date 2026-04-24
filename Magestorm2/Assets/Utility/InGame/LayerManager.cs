using UnityEngine;

public static class LayerManager
{
    public const string Layer_Player = "Player";
    public const string Layer_DeadPlayer = "DeadPlayer";
    public const string Layer_RemotePlayer = "RemotePlayer";
    public const string Layer_DeadBody = "DeadBody";
    public const string Layer_Surface = "Surface";
    public const string Layer_Default = "Default";
    public const string Layer_Interactable = "Interactable";
    public const string Layer_PlayerWallSolid = "PlayerWall_Solid";
    public const string Layer_PlayerWallNonSolid = "PlayerWall_NonSolid";


    private static int _playerLayer, _playerLayerMask;
    private static int _remotePlayerLayer, _remotePlayerLayerMask;
    private static int _deadPlayerLayer, _deadPlayerLayerMask;
    private static int _deadBodyLayer;
    private static int _surfaceLayerMask;
    private static int _interactableMask;
    private static int _projectileImpactMask;
    private static int _aoeObstructionMask;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _playerLayer = LayerMask.NameToLayer(Layer_Player);
            _deadPlayerLayer = LayerMask.NameToLayer(Layer_DeadPlayer);
            _deadBodyLayer = LayerMask.NameToLayer(Layer_DeadBody);
            _remotePlayerLayer = LayerMask.NameToLayer(Layer_RemotePlayer);
            _playerLayerMask = LayerMask.GetMask(Layer_Player);
            _deadPlayerLayerMask = LayerMask.GetMask(Layer_DeadPlayer);
            _remotePlayerLayerMask = LayerMask.GetMask(Layer_RemotePlayer);
            _surfaceLayerMask = LayerMask.GetMask(Layer_Surface);
            _interactableMask = LayerMask.GetMask(Layer_Interactable);
            _projectileImpactMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default, Layer_RemotePlayer, Layer_Interactable, Layer_PlayerWallSolid, Layer_PlayerWallNonSolid });
            _aoeObstructionMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default });
            _init = true;
        }
    }
    public static int AoEObstructionMask
    {
        get {  return _aoeObstructionMask; }
    }
    public static int DeadBodyLayer
    {
        get { return _deadBodyLayer; }
    }
    public static int ProjectileImpactMask
    {
        get { return _projectileImpactMask; }
    }
    public static int PlayerLayer
    {
        get { return _playerLayer; }
    }
    public static int PlayerLayerMask
    {
        get { return _playerLayerMask; }
    }
    public static int DeadPlayerLayer
    {
        get { return _deadPlayerLayer; }
    }
    public static int DeadPlayerLayerMask
    {
        get { return _deadPlayerLayerMask; }
    }
    public static int RemotePlayerLayer
    {
        get { return _remotePlayerLayer; }
    }
    public static int RemotePlayerLayerMask
    {
        get { return _remotePlayerLayerMask; }
    }
    public static int SurfaceMask
    {
        get{
            return _surfaceLayerMask;
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
