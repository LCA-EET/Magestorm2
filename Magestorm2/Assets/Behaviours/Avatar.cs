using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>, IDistanced
{
    public AudioSource AudioSource;
    public GameObject CharacterName;
    public RayCaster RayCaster;
    private int _lastPRPacketID = 0;
    private string _name;
    private byte _level, _class;
    private CharacterClassData _classData;
    private string _playerClassString;
    private Team _team;
    private bool _playersAvatar;
    private bool _isAlive;
    private bool _updatedNeeded;
    private bool _isMale;
    private byte _playerID;
    private Vector3 _startPostion, _nextPosition, _priorPosition;
    private Vector3 _nextRotation;
    private bool _positionChange, _rotationChange, _bodyShown;
    private float _positionElapsed, _rotationElapsed, _rotationAmount;
    private float _distanceTravelled;
    private float _yPeak;
    private float _effectTick = 0.5f;
    private Renderer[] _renderers;
    private Dictionary<byte, AppliedEffect> _appliedEffects;
    private CharacterModel _model;
    
    private TMP_Text _nameText;
    private List<PeriodicAction> _actionList;
    private PeriodicAction _lookAtCamera, _effectsTick, _stepSound;
    private Animator _animator;
    private PMDByte _pmd; // posture, movement, direction
    private DeadBody _deadBody;
    public AvatarAnimation AvatarAnimation;
    public BoxCollider RPCollider;
    public GameObject TeamIndicator, InnerIndicator;
    void Awake()
    {
        _appliedEffects = new Dictionary<byte, AppliedEffect>();
        _positionElapsed = 0.0f;
        _rotationElapsed = 0.0f;
        _nameText = CharacterName.GetComponent<TMP_Text>();
        _pmd = new PMDByte();
    }
    void Start()
    {
        _actionList = new List<PeriodicAction>();
        _lookAtCamera = new PeriodicAction(0.2f, NameRotate, _actionList);
        _effectsTick = new PeriodicAction(_effectTick, EffectTick, _actionList);
        _stepSound = new PeriodicAction(0.1f, StepSoundCheck, _actionList);
    }

    private void StepSoundCheck()
    {
        if(transform.position.y > _yPeak)
        {
            _yPeak = transform.position.y;
        }
        if (IsAlive)
        {
            if (Vector3.Distance(Game.PCAvatar.transform.position, transform.position) <= Game.Clips.FootstepAudioDistance)
            {
                Surface standingOn;
                if (RayCaster.GetSurface(transform, out standingOn))
                {
                    Footstep footstep = standingOn == null ? Footstep.Stone : standingOn.FootstepType;
                    bool playFootstep = false;
                    if (_yPeak - transform.position.y > 1.0f)
                    {
                        _yPeak = transform.position.y;
                        playFootstep = true;
                    }
                    else
                    {
                        if (_pmd.IsRunning)
                        {
                            _distanceTravelled += Vector3.Distance(_priorPosition, transform.position);
                            _priorPosition = transform.position;
                            if (_distanceTravelled >= 2.0f)
                            {
                                _distanceTravelled = 0;
                                if (RayCaster.GetSurface(transform, out standingOn))
                                {
                                    playFootstep = true;
                                }
                            }
                        }
                    }
                    if (playFootstep)
                    {
                        if(footstep == Footstep.Water)
                        {
                            ComponentRegister.Spawner.SpawnVFX(ControlCodes.VFX_Splash, transform.position);
                        }
                        else
                        {
                            Game.Clips.PlayFootstep(footstep, AudioSource);
                        }
                    }
                }
            }
        }
    }
    private void AssignIndicatorLayer()
    {
        switch (_team)
        {
            case Team.Chaos:
                TeamIndicator.layer = LayerManager.TeamLayer_Chaos;
                break;
            case Team.Balance:
                TeamIndicator.layer = LayerManager.TeamLayer_Balance;
                break;
            case Team.Order:
                TeamIndicator.layer = LayerManager.TeamLayer_Order;
                break;
            case Team.Neutral:
                TeamIndicator.gameObject.SetActive(false);
                break;
        }
        InnerIndicator.layer = TeamIndicator.layer;
    }
    public void PlayAudioClip(AudioClip toPlay)
    {
        AudioSource.PlayOneShot(toPlay);
    }
    private void AssignIndicatorColor()
    {
        Color toUse = Colors.Neutral;
        switch (_team)
        {
            case Team.Chaos:
                toUse = Colors.Chaos;
                break;
            case Team.Balance:
                toUse = Colors.Balance;
                break;
            case Team.Order:
                toUse = Colors.Order;
                break;
        }
        InnerIndicator.GetComponent<SpriteRenderer>().color = toUse;
    }
    private void FixedUpdate()
    {
        if (_positionChange)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _positionElapsed, Game.MovementPolling, _startPostion, _nextPosition, transform, false, true))
            {
                _positionChange = false;
            }
            
        }
        if (_rotationChange)
        {
            if(SharedFunctions.ProcessRotation(_rotationAmount, transform, ref _rotationElapsed, Game.MovementPolling))
            {
                _rotationChange = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        PeriodicAction.PerformActions(Time.deltaTime, _actionList);
        AvatarAnimation.SetElapsed(Time.deltaTime);
        AvatarAnimation.Animate(_pmd, false);
        
    }
    public void ForceIdleAnimation()
    {
        AvatarAnimation.SwitchRTAC(AnimationKeys.Idle_Standing);
    }
    public void SetDeadBody(DeadBody deadBody)
    {
        _deadBody = deadBody;
    }
    public void CreateDeadBody()
    {
        ComponentRegister.Spawner.SpawnDeadBody(_model.gameObject, transform.position, transform.eulerAngles.y, _isMale?AvatarAnimation.MaleDeath: AvatarAnimation.FemaleDeath, this);
    }
    private void EffectTick()
    {
        if (_appliedEffects.Count > 0)
        {
            List<AppliedEffect> expired = new List<AppliedEffect>();
            foreach (AppliedEffect effect in _appliedEffects.Values)
            {
                if (effect.Tick(_effectTick))
                {
                    expired.Add(effect);
                }
                //Debug.Log("Time Remaining: " + effect.TimeRemaining);
            }
            foreach(AppliedEffect effect in expired)
            {
                RemoveEffect(effect.EffectCode, false);
            }
            if(expired.Count > 0)
            {
                RefreshEffectsDisplay();
            }
        }
    }
    private void NameRotate()
    {
        SharedFunctions.RotateToCamera(CharacterName.transform);
    }
    private void SwapMaterials(bool opaque)
    {
        if(_renderers == null)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        for(int i = 0; i < _renderers.Length; i++)
        {
            Renderer renderer = _renderers[i];
            string materialName = renderer.material.name.ToLower().Replace (" (instance)", "");
            Material toUse = null;
            if(ComponentRegister.ModelBuilder.GetMaterial(materialName, opaque, ref toUse))
            {
                renderer.material = toUse;
            }
        }
    }
    public void SetAlive(bool alive)
    {
        SwapMaterials(alive);
        _isAlive = alive;
        int layer;
        if (!_isAlive)
        {
            RemoveAllEffects();
            Debug.Log("Removed effects for player " + _name);
            layer = _playersAvatar ? LayerManager.PlayerLayer : LayerManager.DeadPlayerLayer;
        }
        else
        {
            layer = _playersAvatar? LayerManager.PlayerLayer : LayerManager.RemotePlayerLayer;
        }
        SharedFunctions.SetLayerRecursive(gameObject, layer);
        if (alive)
        {
            AssignIndicatorLayer();
        }
        if(_classData.CanSeeDeadPlayers
            && PlayerTeam == MatchParams.MatchTeam
            && MatchParams.MatchTeam != Team.Neutral)
        {
            if (_isAlive)
            {
                Match.RemoveDeadAvatar(PlayerID);
            }
            else
            {
                Match.AddDeadAvatar(this);
            }
        }
        if (_isAlive && _deadBody != null)
        {
            _deadBody.DestroySelf();
        }
    }
    public void AddEffect(AppliedEffect effect)
    {
        if (_appliedEffects.ContainsKey(effect.EffectCode))
        {
            RemoveEffect(effect.EffectCode, true);
        }
        effect.ApplyEffect(this);
        _appliedEffects.Add(effect.EffectCode, effect);
        RefreshEffectsDisplay();
    }
    public void RemoveAllEffects()
    {
        List<byte> toRemove = new List<byte>();
        foreach(byte key in _appliedEffects.Keys)
        {
            toRemove.Add(key);
        }
        foreach (byte key in toRemove)
        {
            RemoveEffect(key, false);
        }
        RefreshEffectsDisplay();
    }
    public void RemoveEffect(byte toRemove, bool refresh)
    {
        if (_appliedEffects.ContainsKey(toRemove))
        {
            AppliedEffect removed = _appliedEffects[toRemove];
            removed.ReverseEffect();

            _appliedEffects.Remove(toRemove);
            if (refresh)
            {
                RefreshEffectsDisplay();
            }
        }
    }
    public void RefreshEffectsDisplay()
    {
        if(_playerID == MatchParams.IDinMatch)
        {
            BitArray effectBits = new BitArray(16, false);
            for (byte b = 0; b < effectBits.Length; b++) {
                effectBits[b] = _appliedEffects.ContainsKey(b);
            }
            ComponentRegister.EffectsList.RefreshEffects(ByteUtils.BitArrayToBytes(effectBits));
        }
    }
    public void UpdateModelRotation()
    {
        if (PMD.IsAirborne)
        {
            _model.SetFlyingRotation();
        }
        else
        {
            _model.SetUprightRotation();
        }
    }
    public void SetAttributes(byte id, string name, byte level, byte playerClass, Team team, byte[] appearance, bool alive)
    {
        _name = name;
        _class = playerClass;
        _classData = CharacterClassManager.GetCharacterClassData(playerClass);
        _level = level;
        _playerClassString = _classData.CharacterClassName;
        _team = team;
        _nameText.text = name;
        _nameText.color = Teams.GetTeamColor(_team);
        _playerID = id;
        _model = ComponentRegister.ModelBuilder.ConstructModel(appearance, (byte)team, level, gameObject, ref _isMale).GetComponent<CharacterModel>();
        _animator = _model.GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
        AvatarAnimation.Init(_animator, appearance[0] == 0);
        _model.transform.localPosition = new Vector3(0, -0.08f, 0);
        if(MatchParams.IDinMatch == id)
        {
            _playersAvatar = true;
            ComponentRegister.PlayerAvatar = this;
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.transform.SetParent(ComponentRegister.PC.transform, false);
            SharedFunctions.SetLayerRecursive(gameObject, LayerManager.PlayerLayer);
            CharacterName.gameObject.SetActive(false);
            RPCollider.enabled = false;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
        SetAlive(alive);
        AssignIndicatorColor();
        AssignIndicatorLayer();
        _priorPosition = transform.position;
    }
    public PMDByte PMD
    {
        get { return _pmd; }
    }
    public bool IsEffectActive(byte effectID)
    {
        return _appliedEffects.ContainsKey(effectID);
    }
    public int LastPRPacketID
    {
        get { return _lastPRPacketID; }
        set { _lastPRPacketID = value; }
    }
    public void UpdatePosition(byte[] decrypted, int index, bool instant)
    {
        float x = BitConverter.ToSingle(decrypted, index);
        float y = BitConverter.ToSingle(decrypted, index + 4);
        float z = BitConverter.ToSingle(decrypted, index + 8);

        if (instant)
        {
            transform.position = new Vector3(x, y, z);
        }
        else
        {
            _startPostion = transform.position;
            _nextPosition = new Vector3(x, y, z);
            if(_positionElapsed > 0.0f)
            {
                _positionElapsed = 0.0f -  (Game.MovementPolling - _positionElapsed);
            }
            _positionChange = true;
        }
    }

    public void UpdateDirection(byte[] decrypted, int index, bool instant)
    {
        float y = BitConverter.ToSingle(decrypted, index);
        _nextRotation = new Vector3(0, y, 0);
        if (instant)
        {
            transform.eulerAngles = _nextRotation;
        }
        else
        {
            _rotationAmount = y - transform.eulerAngles.y;
            if(_rotationAmount < -180)
            {
                _rotationAmount += 365;
            }
            else if(_rotationAmount > 180)
            {
                _rotationAmount -= 365;
            }
            
            if (_rotationElapsed > 0.0f)
            {
                _rotationElapsed = 0.0f - (Game.MovementPolling - _rotationElapsed);
            }
            _rotationChange = true;
        }
    }
   
    public bool IsAlive 
    {
        //get { return MatchParams.IDinMatch == _playerID?ComponentRegister.PC.IsAlive:_isAlive; }
        get { return _isAlive; }
    }
    public bool UpdateNeeded
    {
        get { return _updatedNeeded; }
        set { _updatedNeeded = value; }
    }
    public byte PlayerID
    {
        get { return _playerID; }
    }
    public Team PlayerTeam
    {
        get { return _team; }
    }
    public string Name
    {
        get { return _name; }
    }
    public byte Level
    {
        get { return _level; }
    }
    public string PlayerClassString
    {
        get { return _playerClassString; }
    }
    public byte PlayerClass
    {
        get { return _class; }
    }
    public int CompareTo(Avatar other)
    {
        if(_team < other.PlayerTeam)
        {
            return -1;
        }
        else if (_team > other.PlayerTeam)
        {
            return 1;             
        }
        else
        {
            return _name.CompareTo(other.Name);
        }
    }
    public float DetermineDistance(Transform remote)
    {
        return Vector3.Distance(remote.position, transform.position);
    }
}
