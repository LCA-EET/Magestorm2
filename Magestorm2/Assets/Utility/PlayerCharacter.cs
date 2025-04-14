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
    public static byte StringToClass(string playerClass)
    {
        return 0;
    }
    public static string ClassToString(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Arcanist:
                return Language.GetBaseString(6);
            case PlayerClass.Magician:
                return Language.GetBaseString(7);
            case PlayerClass.Cleric:
                return Language.GetBaseString(5);
            case PlayerClass.Mentalist:
                return Language.GetBaseString(8);
        }
        return "";
    }
    public int CharacterID { get { return _characterID; } }
    public string CharacterName { get { return _characterName; } }
    public byte CharacterClass { get { return _characterClass; } }
    public byte CharacterLevel { get { return _characterLevel; } }
    public string CharacterClassString
    {
        get { return ClassToString((PlayerClass)CharacterClass); }
    }
}
