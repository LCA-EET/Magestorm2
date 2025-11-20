using System;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>
{
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    

    public void SetAttributes(byte id, string name, byte level, byte playerClass, Team team, byte[] appearance)
    {
        _name = name;
        _class = playerClass;
        _level = level;
        _playerClassString = PlayerCharacter.ClassToString((PlayerClass)playerClass);
        _team = team;
        _playerID = id;
        Debug.Log("Avatar name: " + _name + ", class: " + _class + ", level: " + _level);
        ComponentRegister.ModelBuilder.ConstructModel(appearance, (byte)team, level, gameObject);
        gameObject.transform.localPosition = new Vector3(0, -0.08f, 0);
        if(MatchParams.IDinMatch == id)
        {
            gameObject.transform.SetParent(ComponentRegister.PC.transform, false);
            SharedFunctions.SetLayerRecursive(gameObject, LayerManager.PlayerLayer);
        }
    }
    public void UpdatePosition(byte[] decrypted, bool instant)
    {
        float x = BitConverter.ToSingle(decrypted, 3);
        float y = BitConverter.ToSingle(decrypted, 7);
        float z = BitConverter.ToSingle(decrypted, 11);
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
        set { _isAlive = value; }
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
