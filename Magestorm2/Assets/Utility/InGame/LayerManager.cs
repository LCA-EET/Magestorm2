using UnityEngine;

public static class LayerManager
{
    private const string Layer_Player = "Player";
    private const string Layer_DeadPlayer = "DeadPlayer";
    private const string Layer_RemotePlayer = "RemotePlayer";
    private const string Layer_DeadBody = "DeadBody";
    private const string Layer_Surface = "Surface";
    private const string Layer_Default = "Default";
    private const string Layer_Interactable = "Interactable";
    private const string Layer_PlayerWallSolid = "PlayerWall_Solid";
    private const string Layer_PlayerWallNonSolid = "PlayerWall_NonSolid";
    private const string Layer_TeamIndicator_Chaos = "TeamChaos";
    private const string Layer_TeamIndicator_Balance = "TeamBalance";
    private const string Layer_TeamIndicator_Order = "TeamOrder";
    private const string Layer_Woosh = "Woosh";
    private const string Layer_Biasable = "Biasable";

    private static int _biasableLayer;
    private static int _wooshLayer;
    private static int _teamChaosLayer, _teamBalanceLayer, _teamOrderLayer;
    private static int _teamChaosLayerMask, _teamBalanceLayerMask, _teamOrderLayerMask;
    private static int _playerLayer, _playerLayerMask;
    private static int _remotePlayerLayer, _remotePlayerLayerMask;
    private static int _deadPlayerLayer, _deadPlayerLayerMask;
    private static int _deadBodyLayer;
    private static int _surfaceLayerMask;
    private static int _interactableMask;
    private static int _projectileImpactMask;
    private static int _mindImpactMask;
    private static int _aoeObstructionMask, _resistableObstructionMask;
    private static int _floorMask;
    private static bool _init = false;
    public static void Init()
    {
        if (!_init)
        {
            _biasableLayer = LayerMask.NameToLayer(Layer_Biasable);
            _wooshLayer = LayerMask.NameToLayer(Layer_Woosh);
            _teamChaosLayer = LayerMask.NameToLayer(Layer_TeamIndicator_Chaos);
            _teamBalanceLayer = LayerMask.NameToLayer(Layer_TeamIndicator_Balance);
            _teamOrderLayer = LayerMask.NameToLayer(Layer_TeamIndicator_Order);
            _teamChaosLayerMask = LayerMask.GetMask(new string[] { Layer_TeamIndicator_Chaos });
            _teamBalanceLayerMask = LayerMask.GetMask(new string[] { Layer_TeamIndicator_Balance });
            _teamOrderLayerMask = LayerMask.GetMask(new string[] { Layer_TeamIndicator_Order });
            _playerLayer = LayerMask.NameToLayer(Layer_Player);
            _deadPlayerLayer = LayerMask.NameToLayer(Layer_DeadPlayer);
            _deadBodyLayer = LayerMask.NameToLayer(Layer_DeadBody);
            _remotePlayerLayer = LayerMask.NameToLayer(Layer_RemotePlayer);
            _playerLayerMask = LayerMask.GetMask(Layer_Player);
            _deadPlayerLayerMask = LayerMask.GetMask(Layer_DeadPlayer);
            _remotePlayerLayerMask = LayerMask.GetMask(Layer_RemotePlayer);
            _surfaceLayerMask = LayerMask.GetMask(Layer_Surface);
            _interactableMask = LayerMask.GetMask(Layer_Interactable);
            _floorMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default, Layer_PlayerWallSolid, Layer_Interactable });
            _projectileImpactMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default, Layer_RemotePlayer, Layer_Interactable, Layer_PlayerWallSolid, Layer_PlayerWallNonSolid });
            _mindImpactMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default, Layer_RemotePlayer, Layer_Interactable});
            _aoeObstructionMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default, Layer_PlayerWallSolid , Layer_PlayerWallNonSolid });
            _resistableObstructionMask = LayerMask.GetMask(new string[] { Layer_Surface, Layer_Default });
            _init = true;
        }
    }
    public static int BiasableLayer
    {
        get
        {
            return _biasableLayer;
        }
    }
    public static int WooshLayer
    {
        get
        {
            return _wooshLayer;
        }
    }
    public static int TeamLayerMask_Chaos
    {
        get { return _teamChaosLayerMask; }
    }

    public static int TeamLayerMask_Balance
    {
        get { return _teamBalanceLayerMask; }
    }

    public static int TeamLayerMask_Order
    {
        get { return _teamOrderLayerMask; }
    }

    public static int TeamLayer_Chaos
    {
        get
        {
            return _teamChaosLayer;
        }
    }
    public static int TeamLayer_Balance
    {
        get
        {
            return _teamBalanceLayer;
        }
    }
    public static int TeamLayer_Order
    {
        get
        {
            return _teamOrderLayer;
        }
    }
    public static int ResistableObstructionMask
    {
        get { return _resistableObstructionMask;}
    }
    public static int MindImpactMask
    {
        get { return _mindImpactMask; }
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
    public static int FloorMask
    {
        get
        {
            return _floorMask;
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
