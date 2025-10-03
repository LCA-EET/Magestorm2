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
    private Dictionary<byte, byte> _poolData;
    public Level(byte id, string name, byte maxPlayers, byte[] poolData)
    {
        _id = id;
        _name = name;
        _maxPlayers = maxPlayers;
        _poolData = new Dictionary<byte, byte>();
        for (int i = 0; i < poolData.Length; i+=2)
        {
            _poolData.Add(poolData[i], poolData[i+1]);
        }
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

    public byte GetPoolPower(byte poolID)
    {
        return _poolData[poolID];
    }
}

