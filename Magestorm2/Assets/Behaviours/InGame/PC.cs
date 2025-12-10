using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PC : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public RayCaster DownwardCaster;
    public RayCaster ForwardCaster;
    private float _positionLimit = 0.067f;
    private float _rotationLimit = 5f;
    private float _staminaRegen;
    private BoxCollider _playerCollider;
    private Camera _camera;
    public SFXPlayer SFXPlayer;
    public MusicPlayer MusicPlayer;
    public bool JoinedMatch;

    private PlayerClass _class;
    
    private Vector3 _priorPosition, _priorRotation;
    private List<PeriodicAction> _actionList;

    private ManaPool _enteredPool;
    private Dictionary<PlayerIndicator, HMLUpdater> _hml;
    private Dictionary<byte, LeyInfluencer> _activeInfluencers;
    private HMLUpdater _hp, _mana, _ley, _stamina;
    private PeriodicAction _joinRerequest;
    private int _prPacketID = 0;
    public bool InValhalla = false;
    public HashSet<int> _inTriggers;
    public HashSet<int> _priorInTriggers;
    
    private Dictionary<EffectCode, AppliedEffect> _effects;
    public void Awake()
    {
        if (!Game.Running)
        {
            SceneManager.LoadScene("Pregame");
        }
        else
        {
            _effects = new Dictionary<EffectCode, AppliedEffect>();
            _inTriggers = new HashSet<int>();
            _priorInTriggers = new HashSet<int>();
            _activeInfluencers = new Dictionary<byte, LeyInfluencer>();
            ComponentRegister.PC = this;
            PlayerMovement.SetPC(this);
            _staminaRegen = MatchParams.MaxStamina / 8.0f;
            _playerCollider = GetComponent<BoxCollider>();
            _hml = new Dictionary<PlayerIndicator, HMLUpdater>();
            _class = (PlayerClass)PlayerAccount.SelectedCharacter.CharacterClass;
            _actionList = new List<PeriodicAction>();
            new PeriodicAction(Game.TickInterval, ReportMovement, _actionList);
            new PeriodicAction(Game.TickInterval, UpdateIndicators, _actionList);
            if(MatchParams.MatchTeam != Team.Neutral)
            {
                if (_class == PlayerClass.Cleric || _class == PlayerClass.Magician)
                {
                    new PeriodicAction(1.0f, ComputeLey, _actionList);
                }
            }
        }
    }
    
    public void Start()
    {
        _hp = new HMLUpdater(0.1f, MatchParams.MaxHP, PlayerIndicator.Health, _hml);
        _mana = new HMLUpdater(0.1f, MatchParams.MaxMana, PlayerIndicator.Mana, _hml);
        _ley = new HMLUpdater(0.1f, 1.0f, PlayerIndicator.Ley, _hml);
        _stamina = new HMLUpdater(0.1f, MatchParams.MaxStamina, PlayerIndicator.Stamina, _hml);
        _camera = Camera.main;
        if(_class == PlayerClass.Mentalist)
        {
            _ley.UpdateValue(0.6f);
        }
        if (!JoinedMatch)
        {
            _joinRerequest = new PeriodicAction(1.0f, JoinRerequest, null);
        }
    }
    public void ApplyEffect(AppliedEffect effect)
    {
        EffectCode effectCode = effect.EffectCode;
        if (_effects.ContainsKey(effectCode))
        {
            AppliedEffect toCancel = _effects[effectCode];
        }
    }
    public void FixedUpdate()
    {
        if (_priorInTriggers.Count > 0 || _inTriggers.Count > 0)
        {
            List<int> exited = new List<int>();
            foreach (int id in _priorInTriggers)
            {
                if (!_inTriggers.Contains(id))
                {
                    exited.Add(id);
                }
            }
            _priorInTriggers.Clear();
            foreach(int id in _inTriggers)
            {
                _priorInTriggers.Add(id);
            }
            _inTriggers.Clear();
            foreach(int id in exited)
            {
                Trigger exitedTrigger = null;
                if(TriggerManager.GetTrigger(id, ref exitedTrigger))
                {
                    if(exitedTrigger.Entered && !exitedTrigger.Exited)
                    {
                        exitedTrigger.ExitAction();
                    }
                }
            }
        }
    }
    public void Update()
    {
        if (!JoinedMatch)
        {
            _joinRerequest.ProcessAction(Time.deltaTime);
            Debug.Log("Re-requesting to Join Match");
        }
        if (InputControls.Action)
        {
            if (IsAlive)
            {
                Activate();
            }
            else
            {
                Tap();
            }
        }
        PeriodicAction.PerformActions(Time.deltaTime, _actionList);
        MenuCheck();
    }
    private void Tap()
    {
        Debug.Log("Sending tap packet.");
        Game.SendInGameBytes(InGame_Packets.TapPacket());
    }
    public PlayerClass CharacterClass
    {
        get
        {
            return _class;
        }
    }
    private void JoinRerequest()
    {
        Game.SendInGameBytes(InGame_Packets.MatchJoinedPacket(InGame_Send.JoinedMatch));
    }
    private void UpdateIndicators()
    {
        foreach(HMLUpdater updater in _hml.Values)
        {
            if (updater.UpdateNeeded)
            {
                updater.UpdateIndication();
            }
        }
    }
    private void ComputeLey()
    {
        float newLey = 0.0f;
        foreach(LeyInfluencer influence in _activeInfluencers.Values)
        {
            newLey += influence.GetLeyContribution();
        }
        newLey = (float)Math.Round(newLey, 1);
        if(newLey > 1.0f)
        {
            newLey = 1.0f;
        }
        else if(newLey < 0.0f)
        {
            newLey = 0.0f;
        }
        if(newLey != _ley.Value)
        {
            Game.SendInGameBytes(InGame_Packets.UpdateLeyPacket(newLey));
        }
    }
    
    private void ReportMovement()
    {
        byte posture = Game.PCAvatar.Posture;
        if (MinimumReportingExceedance(transform.position, ref _priorPosition, _positionLimit) && MinimumReportingExceedance(transform.eulerAngles, ref _priorRotation, _rotationLimit))
        {
            byte[] prData = new byte[28];
            ByteUtils.FillArray(ref prData, 0, _priorPosition);
            ByteUtils.FillArray(ref prData, 12, _priorRotation);
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(2, posture, prData, ref _prPacketID));
        }
        else if (MinimumReportingExceedance(transform.position, ref _priorPosition, _positionLimit))
        {
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(0, posture, ByteUtils.Vector3ToBytes(_priorPosition), ref _prPacketID));
        }
        else if (MinimumReportingExceedance(transform.eulerAngles, ref _priorRotation, _rotationLimit))
        {
            Game.SendInGameBytes(InGame_Packets.PlayerMovedPacket(1, posture, ByteUtils.Vector3ToBytes(_priorRotation), ref _prPacketID));
        }
    }
    private bool MinimumReportingExceedance(Vector3 current, ref Vector3 prior, float limit)
    {
        if (Vector3.Distance(current, prior) > limit)
        {
            prior = current;
            return true;
        }
        return false;
    }
    private void MenuCheck()
    {
        if (InputControls.InGameMenu && !Game.ControlMode)
        {
            if (!Game.MenuMode)
            {
                ComponentRegister.UIPrefabManager.InstantiateInGameMenu();
            }
            else
            {
                ComponentRegister.UIPrefabManager.PopFromStack();
            }
            Game.MenuMode = !Game.MenuMode;
            Debug.Log("Menu Mode? " + Game.MenuMode);
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXPlayer.PlayClip(clip);
    }

    public void OnTriggerEnter(Collider other)
    {
        Trigger toProcess = null;
        if (ObtainTrigger(other, ref toProcess))
        {
            toProcess.EnterAction();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        /*
        Trigger toProcess = null;
        if (ObtainTrigger(other, ref toProcess))
        {
            toProcess.ExitAction();
        }
        */
    }

    public void OnTriggerStay(Collider other)
    {
        Trigger toProcess = null;
        if (ObtainTrigger(other, ref toProcess))
        {
            _inTriggers.Add(toProcess.TriggerID);
        }
    }
    private bool ObtainTrigger(Collider other, ref Trigger trigger)
    {
        trigger = other.GetComponent<Trigger>();
        return trigger != null;
    }
    public void HPandManaUpdate(byte[] decrypted)
    {
        _hp.UpdateValue(BitConverter.ToSingle(decrypted, 1));
        _mana.UpdateValue(BitConverter.ToSingle(decrypted, 5));
    }
    public void HPorManaorLeyUpdate(byte[] decrypted)
    {
        float value = BitConverter.ToSingle(decrypted, 1);
        switch (decrypted[0])
        {
            case InGame_Receive.HPUpdate:
                Debug.Log("HP Update");
                _hp.UpdateValue(value);
                break;
            case InGame_Receive.ManaUpdate:
                Debug.Log("Mana Update");
                _mana.UpdateValue(value);
                break;
            case InGame_Receive.LeyUpdate:
                Debug.Log("Ley Update");
                _ley.UpdateValue(value);
                break;
        }
    }
    public bool IsAlive
    {
        get
        {
            return _hp.Value > 0;
        }
    }

    private void Activate()
    {
        RaycastHit hitInfo;
        if (RayCaster.CameraCastForward(LayerManager.InteractableMask, 2.0f, out hitInfo))
        {
            Debug.Log(hitInfo.collider.name);
            hitInfo.collider.gameObject.GetComponent<ActivateableObject>().StateChangeRequest();
        }
    }

    public void RegisterLeyInfluencer(byte id, LeyInfluencer influencer)
    {
        _activeInfluencers.Add(id, influencer);
    }

    public void DeregisterLeyInfluencer(byte id)
    {
        _activeInfluencers.Remove(id);
    }
    public float CurrentMana
    {
        get
        {
            return _mana.Value;
        }
    }
    public float CurrentStamina
    {
        get
        {
            return _stamina.Value;
        }
    }
    public void UpdateHP(float value)
    {
        _hp.UpdateValue(value);
    }
    public void UseStamina(float amount)
    {
        _stamina.UpdateValue(_stamina.Value - amount);
    }
    public void RegenStamina(float deltaTime, bool moving)
    {
        float regen = moving ? _staminaRegen / 2.0f : _staminaRegen;
        _stamina.UpdateValue(_stamina.Value + (deltaTime * regen));
    }
    public void UpdatePosition(Vector3 position)
    {
        ComponentRegister.PlayerController.enabled = false;
        transform.position = position;
        ComponentRegister.PlayerController.enabled = true;
    }
}
