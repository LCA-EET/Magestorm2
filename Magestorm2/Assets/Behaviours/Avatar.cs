using UnityEngine;

public class Avatar : MonoBehaviour
{
    private string _name;
    private string _level;
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

    public void SetAttributes(byte id, string name, string level, string playerClass, Team team)
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
}
