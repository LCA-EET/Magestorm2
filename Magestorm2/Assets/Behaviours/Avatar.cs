using System;
using UnityEngine;

public class Avatar : MonoBehaviour, IComparable<Avatar>
{
    private string _name;
    private byte _level;
    private string _playerClass;
    private Team _team;
    private bool _isAlive;
    private bool _updatedNeeded;
    private byte _playerID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttributes(byte id, string name, byte level, string playerClass, Team team)
    {
        _name = name;
        _level = level;
        _playerClass = playerClass;
        _team = team;
        _playerID = id;
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
        set { _team = value; }
    }
    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public byte Level
    {
        get { return _level; }
        set { _level = value; }
    }
    public string PlayerClass
    {
        get { return _playerClass; }
        set { _playerClass = value; }
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
