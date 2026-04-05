using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>, IDistanced
{
    public GameObject CharacterName;
    private int _lastPRPacketID = 0;
    private string _name;
    private byte _level, _class;
    private string _playerClassString;
    private Team _team;
    private bool _playersAvatar;
    private bool _isAlive;
    private bool _updatedNeeded;
    private bool _isMale;
    private byte _playerID;
    private Vector3 _startPostion, _nextPosition;
    private Vector3 _nextRotation;
    private bool _positionChange, _rotationChange, _bodyShown;
    private float _positionElapsed, _rotationElapsed, _rotationAmount;
    private float _effectTick = 0.5f;
    private Renderer[] _renderers;
    private Dictionary<byte, AppliedEffect> _appliedEffects;
    private GameObject _model;
    private TMP_Text _nameText;
    private List<PeriodicAction> _actionList;
    private PeriodicAction _lookAtCamera, _effectsTick;
    private Animator _animator;
    private PMDByte _pmd; // posture, movement, direction
    private DeadBody _deadBody;
    public AvatarAnimation AvatarAnimation;
    public BoxCollider RPCollider;
    void Awake()
    {
        _actionList = new List<PeriodicAction>();
        _lookAtCamera = new PeriodicAction(0.2f, NameRotate, _actionList);
        _effectsTick = new PeriodicAction(_effectTick, EffectTick, _actionList);
        _nameText = CharacterName.GetComponent<TMP_Text>();
        _pmd = new PMDByte();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _appliedEffects = new Dictionary<byte, AppliedEffect>();
        _positionElapsed = 0.0f;
        _rotationElapsed = 0.0f;
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
        AvatarAnimation.Animate(_pmd);
    }
    public void SetDeadBody(DeadBody deadBody)
    {
        _deadBody = deadBody;
    }
    public void CreateDeadBody()
    {
        ComponentRegister.Spawner.SpawnDeadBody(_model, transform.position, transform.eulerAngles.y, _isMale?AvatarAnimation.MaleDeath: AvatarAnimation.FemaleDeath, this);
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
        int layer;
        if (!_isAlive)
        {
            RemoveAllEffects();
            layer = LayerManager.DeadPlayerLayer;
        }
        else
        {
            layer = _playersAvatar? LayerManager.PlayerLayer : LayerManager.RemotePlayerLayer;
        }
        SharedFunctions.SetLayerRecursive(gameObject, layer);
        if(PlayerAccount.SelectedCharacter.CharacterClass == (byte)PlayerClass.Cleric 
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
                effectBits[b] = _appliedEffects.ContainsKey(b);
            }
            ComponentRegister.EffectsList.RefreshEffects(ByteUtils.BitArrayToBytes(effectBits));
        }
    }
    public void SetAttributes(byte id, string name, byte level, byte playerClass, Team team, byte[] appearance)
    {
        _name = name;
        _class = playerClass;
        _level = level;
        _playerClassString = PlayerCharacter.ClassToString((PlayerClass)playerClass);
        _team = team;
        _nameText.text = name;
        _nameText.color = Teams.GetTeamColor(_team);
        _playerID = id;
        _model = ComponentRegister.ModelBuilder.ConstructModel(appearance, (byte)team, level, gameObject, ref _isMale);
        _animator = _model.GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
        AvatarAnimation.Init(_animator, appearance[0] == 0);
        gameObject.transform.localPosition = new Vector3(0, -0.08f, 0);
        
        if(MatchParams.IDinMatch == id)
        {
            _playersAvatar = true;
            ComponentRegister.PlayerAvatar = this;
            Game.PlayerPMDByte = _pmd;
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
    }
    public PMDByte PMD
    {
        get { return _pmd; }
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
    public float DetermineDistance(Transform remote)
    {
        return Vector3.Distance(remote.position, transform.position);
    }
}
