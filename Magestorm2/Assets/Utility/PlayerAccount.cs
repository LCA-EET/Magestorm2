using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
public static class PlayerAccount
{
    private static int _accountID;
    private static byte[] _accountIDBytes;
    private static Dictionary<int, PlayerCharacter> _characterList;
    public static PlayerCharacter SelectedCharacter; 
    public static bool UpdatesMade;
    public static void Init(int accountID)
    {
        _characterList = new Dictionary<int, PlayerCharacter>();
        _accountID = accountID;
        _accountIDBytes = BitConverter.GetBytes(_accountID);
    }
    public static void DeleteCharacter(int characterID)
    {
        if (_characterList.ContainsKey(characterID))
        {
            _characterList.Remove(characterID);
            UpdatesMade = true;
        }
    }
    public static void AddCharacter(PlayerCharacter toAdd)
    {
        _characterList.Add(toAdd.CharacterID, toAdd);
        UpdatesMade = true;
    }
    public static void AddCharacter(int characterID, string characterName, byte characterClass, byte characterLevel, byte[] statBytes)
    {
        PlayerCharacter pc = new PlayerCharacter(characterID, characterName, characterClass, characterLevel, statBytes);
        AddCharacter(pc);
    }
    public static List<PlayerCharacter> GetCharacterList()
    {
        return _characterList.Values.ToList<PlayerCharacter>();
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
