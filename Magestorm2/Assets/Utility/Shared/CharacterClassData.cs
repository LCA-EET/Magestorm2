using System;
using UnityEngine;

public class CharacterClassData
{
    private byte _id, _hpMultiplier, _manaStatCode, _deadSight;
    private short _nameRef;
    private string _abbrev;
    
    public CharacterClassData(byte[] decrypted)
    {
        _id = decrypted[0];
        _hpMultiplier = decrypted[1];
        _manaStatCode = decrypted[2];
        _deadSight = decrypted[3];
        _nameRef = BitConverter.ToInt16(decrypted, 4);
        _abbrev = Language.GetBaseString(_nameRef).Substring(0, 2);
    }
    public bool CanSeeDeadPlayers
    {
        get {
            return _deadSight == 1;
        }
    }
    public string Abbreviation
    {
        get { return _abbrev; }
    }
    public byte HPMultiplier
    {
        get { return _hpMultiplier; }
    }
    public byte CharacterClassID
    {
        get { return _id; }
    }
    public int NameRef
    {
        get { return _nameRef; }
    }
    public string CharacterClassName
    {
        get { return Language.GetBaseString(_nameRef); }
    }
    public byte ManaStatCode
    {
        get { return _manaStatCode; }
    }
    public byte DeadSight
    {
        get { return _deadSight; }
    }
}
