using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RemotePlayerData
{
    private string _name;
    private byte _idInMatch, _level, _teamID;
    private PlayerClass _playerClass;
    public RemotePlayerData(byte idInMatch, byte teamID, string name, byte level, PlayerClass playerClass)
    {
        _name = name;
        _teamID = teamID;
        _idInMatch = idInMatch;
        _level = level;
        _playerClass = playerClass;
    }
    public byte IdInMatch
    {
        get { return _idInMatch; }
    }
    public string Name
    {
        get { return _name; }
    }

    public byte Level
    { 
        get { return _level; } 
    }

    public PlayerClass PlayerClass
    {
        get { return _playerClass; }
    }
}
