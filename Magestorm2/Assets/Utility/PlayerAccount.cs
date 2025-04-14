using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
public static class PlayerAccount
{
    private static int _accountID;
    private static byte[] _accountIDBytes;
    private static List<PlayerCharacter> _characterList;
    public static void Init(int accountID)
    {
        _characterList = new List<PlayerCharacter>();
        _accountID = accountID;
        _accountIDBytes = BitConverter.GetBytes(_accountID);
    }  
    public static void AddCharacter(PlayerCharacter toAdd)
    {
        _characterList.Add(toAdd);
    }
    public static void AddCharacter(int characterID, string characterName, byte characterClass, byte characterLevel)
    {
        PlayerCharacter pc = new PlayerCharacter(characterID, characterName, characterClass, characterLevel);
        AddCharacter(pc);
    }
    public static List<PlayerCharacter> GetCharacterList()
    {
        return _characterList;
    }

    public static int AccountID
    {
        get { return _accountID; }
    }

    public static byte[] AccountIDBytes
    {
        get { return _accountIDBytes; }
    }
}
