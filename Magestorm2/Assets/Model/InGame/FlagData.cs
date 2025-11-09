using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class FlagData
{
    public const byte NOT_HELD = 0;
    public const byte DROPPED = 1;

    private byte _holder;
    private byte _team;
    private int _endIndex;
    private Vector3 _position;
    public FlagData(byte[] decrypted, int index)
    {
        _team = decrypted[index];
        Debug.Log("Team: " + _team);
        _holder = decrypted[index + 1];
        index += 2;
        if (_holder == DROPPED)
        {
            _position = ByteUtils.BytesToVector3(decrypted, index);
            index += 12;
        }
        _endIndex = index;
    }
    public bool IsHeld
    {
        get
        {
            return _holder != NOT_HELD;
        }
    }
    public byte HolderID
    {
        get
        {
            return _holder;
        }
    }
    public Vector3 Position
    {
        get { return _position; }
    }
    public int EndIndex
    {
        get { return _endIndex; }
    }
    public byte Team
    {
        get { return _team; }
    }
}
