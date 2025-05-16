using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RemotePlayerData
{
    private string _name;
    private byte _level;
    private PlayerClass _playerClass;
    public RemotePlayerData(string name, byte level, PlayerClass playerClass)
    {
        _name = name;
        _level = level;
        _playerClass = playerClass;
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
