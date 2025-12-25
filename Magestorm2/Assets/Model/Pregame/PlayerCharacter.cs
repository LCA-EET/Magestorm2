using JetBrains.Annotations;
using System;
using System.Collections.Generic;
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
    private byte[] _slottedSpells;
    private Dictionary<SpellDiscipline, byte> _skills;
    public PlayerCharacter(int characterID, string characterName, byte characterClass, byte characterLevel, byte[] statBytes, byte[] appearanceBytes, byte[] slots, int skills) {
        _skills = new Dictionary<SpellDiscipline, byte>();
        _slottedSpells = slots;
        _characterID = characterID;
        _characterName = characterName;
        _characterClass = characterClass;
        Debug.Log("CharacterClass: " + _characterClass);
        _characterLevel = characterLevel;
        _characterNameBytes = Encoding.UTF8.GetBytes(characterName);
        _statBytes = statBytes;
        _appearanceBytes = appearanceBytes;
        _idBytes = BitConverter.GetBytes(characterID);
        UpdateSkillsTable(skills);
    }
    public byte GetSkillLevel(SpellDiscipline discipline)
    {
        if (_skills.ContainsKey(discipline))
        {
            return _skills[discipline];
        }
        return 0;
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
    public byte[] StatBytes
    {
        get { return _statBytes; }
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
    public void UpdateSkillsTable(int skills)
    {
        bool[] skillArray = new bool[skills];
        int id = 0;
        while(skills != 0)
        {
            skillArray[id] = skills % 2 != 0;
            skills = skills / 2;
        }
        _skills.Clear();
        foreach(SpellDiscipline discipline in SharedFunctions.DisciplinesByClass((PlayerClass)_characterClass))
        {
            int skillIndex = ((byte)discipline) * 2;
            bool lsb = skillArray[skillIndex];
            bool msb = skillArray[skillIndex + 1];
            byte value;
            if(!msb && !lsb)    // 00
            {
                value = 0;
            }
            else if (!msb)      // 01
            {
                value = 1;
            }
            else if (!lsb)      // 10
            {
                value = 2;
            }
            else                // 11
            {
                value = 3;
            }
            _skills.Add(discipline, value);
        }
    }
}
