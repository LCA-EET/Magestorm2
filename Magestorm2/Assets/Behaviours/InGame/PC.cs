using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PC : MonoBehaviour
{
    public CharacterController CharacterController;
    public PlayerMovement PlayerMovement;
    public RayCaster DownwardCaster;
    public RayCaster ForwardCaster;
    
    private float _staminaRegen;
    private Camera _camera;
    public SFXPlayer SFXPlayer;
    public MusicPlayer MusicPlayer;
    public bool JoinedMatch;

    private PlayerClass _class;
    private List<PeriodicAction> _actionList;

    private ManaPool _enteredPool;
    private Dictionary<PlayerIndicator, HMLUpdater> _hml;
    private Dictionary<byte, LeyInfluencer> _activeInfluencers;
    private HMLUpdater _hp, _mana, _ley, _stamina;
    private PeriodicAction _joinRerequest;
    
    private float _coolDownRemaining = 0.0f;
    public bool InValhalla = false;
    public HashSet<int> _inTriggers;
    public HashSet<int> _priorInTriggers;
    
    private Dictionary<byte, AppliedEffect> _effects;
    private SpellData _primarySpell, _secondarySpell;
    private DistanceSorter _distanceSorter;
    public void Awake()
    {
        if (!Game.Running)
        {
            SceneManager.LoadScene("Pregame");
        }
        else
        {
            _distanceSorter = new DistanceSorter(transform, false);
            _effects = new Dictionary<byte, AppliedEffect>();
            _inTriggers = new HashSet<int>();
            _priorInTriggers = new HashSet<int>();
            _activeInfluencers = new Dictionary<byte, LeyInfluencer>();
            ComponentRegister.PC = this;
            PlayerMovement.SetPC(this);
            _staminaRegen = MatchParams.MaxStamina / 1.67f;
            _hml = new Dictionary<PlayerIndicator, HMLUpdater>();
            _class = (PlayerClass)PlayerAccount.SelectedCharacter.CharacterClass;
            _actionList = new List<PeriodicAction>();
            new PeriodicAction(Game.TickInterval, UpdateIndicators, _actionList);
            if(MatchParams.MatchTeam != Team.Neutral)
            {
                if (_class == PlayerClass.Cleric || _class == PlayerClass.Magician)
                {
                    Debug.Log("Ley computation enabled.");
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
            _joinRerequest = new PeriodicAction(0.25f, Game.SendJoinMatchPacket, null);
        }
    }
    public void ApplyEffect(AppliedEffect effect)
    {
        byte effectCode = effect.EffectCode;
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
        if (Game.Disconnected)
        {
            return;
        }
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
        CheckSpellSlot();
        if(_coolDownRemaining <= 0 && !InValhalla && IsAlive && !Game.PlayerPMDByte.IsRunning)
        {
            CheckCast();
        }
        else
        {
            _coolDownRemaining -= Time.deltaTime;
        }
        PeriodicAction.PerformActions(Time.deltaTime, _actionList);
        MenuCheck();
    }
    private void CheckCast()
    {
        SpellData toCast = null ;
        if (InputControls.ShootPrimary)
        {
            toCast = _primarySpell;
        }
        else if (InputControls.ShootSecondary)
        {
            toCast = _secondarySpell;
        }
        if (toCast != null)
        {
            toCast.CastSpell();
            _coolDownRemaining = 0.5f;
        }
    }
    
    private void CheckSpellSlot()
    {
        byte spellID = InputControls.GetSlottedSpellID();
        if (spellID > 0)
        {
            SpellData spellData = null;
            if (SpellManager.GetSpell(spellID, ref spellData))
            {
                _primarySpell = spellData;
                ComponentRegister.SpellPanel.UpdatePrimaryReference(spellData.SpellNameReference);
            }
        }
        else
        {
            if (InputControls.SetSecondary)
            {
                if(_primarySpell != null)
                {
                    _secondarySpell = _primarySpell;
                    ComponentRegister.SpellPanel.UpdateSecondaryReference(_secondarySpell.SpellNameReference);
                }
            }
        }
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
    public void RestoreHPandMana()
    {
        _hp.UpdateValue(MatchParams.MaxHP);
        _mana.UpdateValue(MatchParams.MaxMana);
    }
    private void ComputeLey()
    {
        //Debug.Log("COMPUTING LEY, INFLUENCER COUNT: " + _activeInfluencers.Count);
        float newLey = 0.0f;
        foreach(LeyInfluencer influence in _activeInfluencers.Values)
        {
            newLey += influence.GetLeyContribution();
        }
        newLey = (float)Math.Round(newLey, 2);
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
            _ley.UpdateValue(newLey);
        }
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
                _hp.UpdateValue(value);
                break;
            case InGame_Receive.ManaUpdate:
                _mana.UpdateValue(value);
                break;
            case InGame_Receive.LeyUpdate:
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
    public byte[] Summon(byte spellID)
    {
        List<Avatar> deadTeammates = Match.DeadAvatars;
        if (deadTeammates.Count > 0)
        {
            Debug.Log(deadTeammates.Count + " dead teammates.");
            deadTeammates.Sort(_distanceSorter);
            return InGame_Packets.SummonPacket(spellID, deadTeammates[0].PlayerID);
        }
        else
        {
            Debug.Log("No dead teammates.");
        }
        return null;
    }
    public void RegisterLeyInfluencer(byte id, LeyInfluencer influencer)
    {
        if (!_activeInfluencers.ContainsKey(id))
        {
            _activeInfluencers.Add(id, influencer);
        }
    }

    public void DeregisterLeyInfluencer(byte id)
    {
        if (_activeInfluencers.ContainsKey(id))
        {
            _activeInfluencers.Remove(id);
        }
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
        if(value <= 0)
        {
            _activeInfluencers.Clear();
        }
    }
    public void UseStamina(float amount)
    {
        _stamina.UpdateValue(_stamina.Value - amount);
    }
    public void RegenStamina(float deltaTime, bool moving)
    {
        if (_stamina.IsLessThanMax)
        {
            float regen = moving ? _staminaRegen / 2.0f : _staminaRegen;
            _stamina.UpdateValue(_stamina.Value + (deltaTime * regen));
        }
    }
    public void UpdatePosition(Vector3 position)
    {
        ComponentRegister.PlayerController.enabled = false;
        transform.position = position;
        ComponentRegister.PlayerController.enabled = true;
    }
}
