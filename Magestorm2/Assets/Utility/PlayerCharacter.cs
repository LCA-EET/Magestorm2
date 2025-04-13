using JetBrains.Annotations;
using UnityEngine;

public class PlayerCharacter
{
    private int _characterID;
    private string _characterName;
    private byte _characterClass;
    private byte _characterLevel;

    public PlayerCharacter(int characterID, string characterName, byte characterClass, byte characterLevel) { 
        _characterID = characterID;
        _characterName = characterName;
        _characterClass = characterClass;
        _characterLevel = characterLevel;
    }

    public int CharacterID { get { return _characterID; } }
    public string CharacterName { get { return _characterName; } }
    public byte CharacterClass { get { return _characterClass; } }
    public byte CharacterLevel { get { return _characterLevel; } }
}
