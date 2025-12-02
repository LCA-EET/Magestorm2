using System;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>
{
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
    private Renderer[] _renderers;
    private Dictionary<EffectCode, AppliedEffect> _appliedEffects;
    private GameObject _model;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _appliedEffects = new Dictionary<EffectCode, AppliedEffect>();
        _moveElapsed = 0.0f;
        _positionChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_positionChange)
        {
            if(SharedFunctions.ProcessVector3Lerp(ref _moveElapsed, Game.TickInterval, _startPostion, _newPosition, transform))
            {
                _positionChange = false;
            }
        }
        if (_rotationChange)
        {
            if (SharedFunctions.ProcessVector3Lerp(ref _moveElapsed, Game.TickInterval, _startRotation, _newRotation, transform))
            {
                _rotationChange = false;
            }
        }
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
    }
    public void AddEffect(AppliedEffect effect)
    {
        if (_appliedEffects.ContainsKey(effect.EffectCode))
        {
            AppliedEffect toCancel = _appliedEffects[effect.EffectCode];
            toCancel.ReverseEffect();
        }
        _appliedEffects.Add(effect.EffectCode, effect);
        effect.ApplyEffect();
    }

    public void SetAttributes(byte id, string name, byte level, byte playerClass, Team team, byte[] appearance)
    {
        _name = name;
        _class = playerClass;
        _level = level;
        _playerClassString = PlayerCharacter.ClassToString((PlayerClass)playerClass);
        _team = team;
        _playerID = id;
        Debug.Log("Avatar name: " + _name + ", class: " + _class + ", level: " + _level);
        _model = ComponentRegister.ModelBuilder.ConstructModel(appearance, (byte)team, level, gameObject);
        gameObject.transform.localPosition = new Vector3(0, -0.08f, 0);
        if(MatchParams.IDinMatch == id)
        {
            gameObject.transform.SetParent(ComponentRegister.PC.transform, false);
            SharedFunctions.SetLayerRecursive(gameObject, LayerManager.PlayerLayer);
        }
    }
    public int LastPRPacketID
    {
        get { return _lastPRPacketID; }
        set { _lastPRPacketID = value; }
    }
    public void UpdatePosition(byte[] decrypted, bool instant)
    {
        float x = BitConverter.ToSingle(decrypted, 7);
        float y = BitConverter.ToSingle(decrypted, 11);
        float z = BitConverter.ToSingle(decrypted, 15);
        if (instant)
        {
            gameObject.transform.position = new Vector3(x, y, z);
        }
        else
        {
            _startPostion = transform.position;
            _newPosition = new Vector3(x, y, z);
            _positionChange = true;
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
    public bool IsAlive 
    {
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
