using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Level
{
    private byte _id;
    private string _name;
    private byte _maxPlayers;
    public Level(byte id, string name, byte maxPlayers)
    {
        _id = id;
        _name = name;
        _maxPlayers = maxPlayers;
    }

    public byte LevelID
    {
        get { return _id; }
    }
    public string LevelName
    {
        get { return _name; }
    }

    public byte MaxPlayers
    {
        get { return _maxPlayers; }
    }
}

