using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>
{
    public GameObject CharacterName;
    private int _lastPRPacketID = 0;
    private string _name;
    private byte _level, _class;
    private string _playerClassString;
    private Team _team;
    private bool _isAlive;
    private bool _updatedNeeded;
    private byte _playerID;
    private Vector3 _startPostion, _newPosition;
    private Vector3 _startRotation, _newRotation;
    private bool _positionChange, _rotationChange;
    private float _moveElapsed;
    private float _effectTick = 0.5f;
    private Renderer[] _renderers;
    private Dictionary<EffectCode, AppliedEffect> _appliedEffects;
    private GameObject _model;
    private TMP_Text _nameText;
    private List<PeriodicAction> _actionList;
    private PeriodicAction _lookAtCamera, _effectsTick;
    private byte _posture;
    private Animator _animator;
    public AvatarAnimation AvatarAnimation;
    
    void Awake()
    {
        _actionList = new List<PeriodicAction>();  
        _lookAtCamera = new PeriodicAction(0.2f, NameRotate, _actionList);
        _effectsTick = new PeriodicAction(_effectTick, EffectTick, _actionList);
        _nameText = CharacterName.GetComponent<TMP_Text>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _appliedEffects = new Dictionary<EffectCode, AppliedEffect>();
        _moveElapsed = 0.0f;
        _positionChange = false;
        
    }

    private void FixedUpdate()
    {
        if (_positionChange)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _moveElapsed, Game.TickInterval, _startPostion, _newPosition, transform, false, true))
            {
                _positionChange = false;
                AvatarAnimation.SetAnimation(AnimationKeys.Idle, true);
            }
        }
        if (_rotationChange)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _moveElapsed, Game.TickInterval, _startRotation, _newRotation, transform, false, false))
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
                Debug.Log("Time Remaining: " + effect.TimeRemaining);
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
        CharacterName.transform.LookAt(Camera.main.transform.position);
        CharacterName.transform.Rotate(0, 180, 0);
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
        if (!_isAlive)
        {
            RemoveAllEffects();
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
        List<EffectCode> toRemove = new List<EffectCode>();
        foreach(EffectCode key in _appliedEffects.Keys)
        {
            toRemove.Add(key);
        }
        foreach (EffectCode key in toRemove)
        {
            RemoveEffect(key, false);
        }
        RefreshEffectsDisplay();
    }
    public void RemoveEffect(EffectCode toRemove, bool refresh)
    {
        AppliedEffect removed = _appliedEffects[toRemove];
        removed.ReverseEffect();
        _appliedEffects.Remove(toRemove);
        if (refresh)
        {
            RefreshEffectsDisplay();
        }
    }
    private void RefreshEffectsDisplay()
    {
        if(_playerID == MatchParams.IDinMatch)
        {
            BitArray effectBits = new BitArray(16, false);
            for (byte b = 0; b < effectBits.Length; b++) {
                effectBits[b] = _appliedEffects.ContainsKey((EffectCode)b);
            }
            ComponentRegister.EffectsList.RefreshEffects(ByteUtils.BitArrayToBytes(effectBits));
        }
    }
    public void SetAttributes(byte id, string name, byte level, byte playerClass, Team team, byte[] appearance)
    {
        _name = name;
        _posture = Postures.Standing;
        _class = playerClass;
        _level = level;
        _playerClassString = PlayerCharacter.ClassToString((PlayerClass)playerClass);
        _team = team;
        _nameText.text = name;
        _nameText.color = Teams.GetTeamColor(_team);
        _playerID = id;
        Debug.Log("Avatar name: " + _name + ", class: " + _class + ", level: " + _level);
        _model = ComponentRegister.ModelBuilder.ConstructModel(appearance, (byte)team, level, gameObject);
        _animator = _model.GetComponentInChildren<Animator>();
        Debug.Log("ANIMATOR GO: "+ _animator.gameObject.name);
        AvatarAnimation.Init(_animator, appearance[0] == 0);
        gameObject.transform.localPosition = new Vector3(0, -0.08f, 0);
        
        if(MatchParams.IDinMatch == id)
        {
            ComponentRegister.PlayerAvatar = this;
            gameObject.layer = LayerMask.NameToLayer("Player");
            gameObject.transform.SetParent(ComponentRegister.PC.transform, false);
            SharedFunctions.SetLayerRecursive(gameObject, LayerManager.PlayerLayer);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
        }
    }
    public int LastPRPacketID
    {
        get { return _lastPRPacketID; }
        set { _lastPRPacketID = value; }
    }
    public void UpdatePosition(byte[] decrypted, bool instant)
    {
        float x = BitConverter.ToSingle(decrypted, 8);
        float y = BitConverter.ToSingle(decrypted, 12);
        float z = BitConverter.ToSingle(decrypted, 16);

        if (instant)
        {
            gameObject.transform.position = new Vector3(x, y, z);
        }
        else
        {
            _startPostion = transform.position;
            _newPosition = new Vector3(x, y, z);
            _positionChange = true;
            switch (Posture)
            {
                case Postures.Jump:
                    AvatarAnimation.SetAnimation(AnimationKeys.Jump, true);
                    break;
                case Postures.Standing:
                    AvatarAnimation.SetAnimation(AnimationKeys.Walk, true);
                    break;
                case Postures.Airborne:
                    break;
                case Postures.Crouched:
                    break;
            }
        }
    }

    public void UpdateDirection(byte[] decrypted, int index, bool instant)
    {
        float x = BitConverter.ToSingle(decrypted, index);
        float y = BitConverter.ToSingle(decrypted, index + 4);
        float z = BitConverter.ToSingle(decrypted, index + 8);
        if (instant)
        {
            transform.eulerAngles = new Vector3(x, y, z);
        }
        else
        {
            _startRotation = transform.eulerAngles;
            _newRotation = new Vector3(x, y, z);
            _rotationChange = true;
        }
    }
    public byte Posture
    {
        get { return _posture; }
        set { _posture = value; }
    }
    public bool IsMidJump
    {
        get
        {
            return _posture == Postures.Jump;
        }
    }
    public bool IsCrouched
    {
        get { return _posture == Postures.Crouched; }
    }
    public bool IsStanding
    {
        get { return _posture == Postures.Standing; }
    }
    public bool IsAirborne
    {
        get { return _posture == Postures.Airborne; } 
    }
    public bool IsAlive 
    {
        get { return MatchParams.IDinMatch == _playerID?ComponentRegister.PC.IsAlive:_isAlive; }
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
    public PlayerClass PlayerClass
    {
        get { return (PlayerClass)_class; }
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
}
