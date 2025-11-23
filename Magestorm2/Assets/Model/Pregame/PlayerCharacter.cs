using JetBrains.Annotations;
using System;
using System.Text;
using UnityEngine;

public class PlayerCharacter
{
    private int _characterID;
    private string _characterName;
    private byte _characterClass;
    private byte _characterLevel;
    private byte[] _characterNameBytes;
    private byte[] _appearanceBytes;
    private byte[] _statBytes;
    private byte[] _idBytes;
    public PlayerCharacter(int characterID, string characterName, byte characterClass, byte characterLevel, byte[] statBytes, byte[] appearanceBytes) { 
        _characterID = characterID;
        _characterName = characterName;
        _characterClass = characterClass;
        _characterLevel = characterLevel;
        _characterNameBytes = Encoding.UTF8.GetBytes(characterName);
        _statBytes = statBytes;
        _appearanceBytes = appearanceBytes;
        _idBytes = BitConverter.GetBytes(characterID);
    }
    public static string ClassToString(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return Language.GetBaseString(7); //
            case PlayerClass.Magician:
                return Language.GetBaseString(8); //
            case PlayerClass.Cleric: 
                return Language.GetBaseString(6); //
            case PlayerClass.Mentalist:
                return Language.GetBaseString(9); //
        }
        return "";
    }
    public byte[] IDBytes
    {
        get { return _idBytes; }
    }
    public byte[] AppearanceBytes
    {
        get
        {
            return _appearanceBytes;
        }
        set
        {
            _appearanceBytes = value;
        }
    }
    public byte[] CharacterNameBytes { get { return _characterNameBytes; } }
    public int CharacterID { get { return _characterID; } }
    public string CharacterName { get { return _characterName; } }
    public byte CharacterClass { get { return _characterClass; } }
    public byte CharacterLevel { get { return _characterLevel; } }
    public string CharacterClassString
    {
        get { return ClassToString((PlayerClass)CharacterClass); }
    }
    private byte HPMultiplier()
    {
        PlayerClass playerClass = (PlayerClass)CharacterClass;
        switch (playerClass)
        {
            case PlayerClass.Cleric:
                return 6;
            case PlayerClass.Magician:
                return 4;
            default:
                return 5;
        }
    }
    public byte GetStat(PlayerStats stat)
    {
        return _statBytes[(byte)stat];
    }
    public float GetMaxHP()
    {
        float multiplier = HPMultiplier();
        float toReturn = (CharacterLevel * (GetStat(PlayerStats.Constitution) / 20.0f) * multiplier * 1.579f) + 10;
        return Mathf.Round(toReturn);
    }
    public float GetMaxMana()
    {
        byte statToUse = (PlayerClass)CharacterClass == PlayerClass.Cleric ? GetStat(PlayerStats.Charisma) : GetStat(PlayerStats.Intellect);
        float manaMultiplier = 1 + ((statToUse - 10) * 0.05f);
        return ((_characterLevel * 4) + 10) * manaMultiplier;
    }
}
