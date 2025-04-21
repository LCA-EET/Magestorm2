using UnityEngine;

public static class ComponentRegister
{
    private static Transform _playerTransform;
    private static Player _player;
    private static PlayerMovement _playerMovement;
    private static CharacterController _playerController;
    private static Camera _mainCamera;
    private static AudioSource _playerAudioSource;
    private static HUD _hud;
    private static MatchTimer _matchTimer;
    private static Notifier _notifier;
    private static AvatarList _avatarList;
    private static EffectsList _effectsList;
    private static ShrinePanel _shrinePanel;
    private static PlayerStatusPanel _playerStatusPanel;
    private static UIPrefabManager _uiprefabManager;
    private static UILoginForm _loginForm;
    private static Transform _uiParent;
    private static PregamePacketProcessor _pregamePacketProcessor;
    private static AudioPlayer _audioPlayer;

    public static AudioPlayer AudioPlayer
    {
        get { return _audioPlayer; }
        set { _audioPlayer = value; }
    }

    public static PregamePacketProcessor PregamePacketProcessor
    {
        get { return _pregamePacketProcessor; }
        set { _pregamePacketProcessor = value; }
    }
    public static Transform UIParent
    {
        get {  return _uiParent; }
        set { _uiParent = value; }
    }
    public static UILoginForm UILoginForm
    {
        get { return _loginForm; }
        set { _loginForm = value; }
    }
    public static UIPrefabManager UIPrefabManager
    {
        get { return _uiprefabManager; }
        set { _uiprefabManager = value; }
    }
    public static PlayerStatusPanel PlayerStatusPanel
    {
        get { return _playerStatusPanel; }
        set { _playerStatusPanel = value; }
    }
    public static ShrinePanel ShrinePanel
    {
        get { return _shrinePanel; }
        set { _shrinePanel = value; }
    }
    public static AvatarList AvatarList
    {
        get{ return _avatarList; }
        set{ _avatarList = value; } 
    }

    public static EffectsList EffectsList
    {
        get { return _effectsList; }
        set{ _effectsList = value; }
    }

    public static Notifier Notifier
    {
        get
        {
            return _notifier;
        }
        set
        {
            _notifier = value;
        }
    }
    public static MatchTimer MatchTimer
    {
        get
        {
            return _matchTimer;
        }
        set
        {
            _matchTimer = value;
        }
    }
    public static Transform PlayerTransform
    {
        get
        {
            return _playerTransform;
        }
        set
        {
            _playerTransform = value;
        }
    }
    public static PlayerMovement PlayerMovement
    {
        get
        {
            return _playerMovement;
        }
        set
        {
            _playerMovement = value;
        }
    }

    public static CharacterController PlayerController
    {
        get
        {
            return _playerController;
        }
        set
        {
            _playerController = value;
        }
    }

    public static Camera MainCamera
    {
        get
        {
            return _mainCamera;
        }
        set
        {
            _mainCamera = value;
        }
    }
    
    public static AudioSource PlayerAudioSource
    {
        get
        {
            return _playerAudioSource;
        }
        set
        {
            _playerAudioSource = value;
        }
    }
    public static Player Player
    {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
        }
    }
    public static HUD HUD
    {
        get
        {
            return _hud;
        }
        set
        {
            _hud = value;
        }
    }
}
