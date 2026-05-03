using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerCharacter
{
    private int _characterID, _experience;
    private string _characterName;
    private byte _characterClass;
    private byte _characterLevel;
    private byte[] _characterNameBytes;
    private byte[] _appearanceBytes;
    private byte[] _statBytes;
    private byte[] _idBytes;
    private byte[] _slottedSpells;
    private Dictionary<byte, byte> _skills;
    public PlayerCharacter(int characterID, string characterName, byte characterClass, byte characterLevel, byte[] statBytes, byte[] appearanceBytes, byte[] slots, int skills, int experience) {
        _skills = new Dictionary<byte, byte>();
        _slottedSpells = slots;
        _characterID = characterID;
        _characterName = characterName;
        _characterClass = characterClass;
        _characterLevel = characterLevel;
        _characterNameBytes = Encoding.UTF8.GetBytes(characterName);
        _statBytes = statBytes;
        _appearanceBytes = appearanceBytes;
        _idBytes = BitConverter.GetBytes(characterID);
        _experience = experience;
        UpdateSkillsTable(skills);
    }
    public byte[] SlottedSpells
    {
        get
        {
            return _slottedSpells;
        }
    }
    public void SetLevel(byte level)
    {
        _characterLevel = level;
    }
    public void SetExperience(int exp)
    {
        _experience = exp;
    }
    public int GetExperience()
    {
        return _experience;
    }
    public void UpdateSlottedSpells(byte[] decrypted, int offset)
    {
        for (int i = 0; i < _slottedSpells.Length; i++)
        {
            _slottedSpells[i] = decrypted[offset + i];
        }
    }
    public byte GetSkillLevel(byte discipline)
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
    public byte GetStat(byte stat)
    {
        return _statBytes[stat];
    }
    public float GetMaxHP()
    {
        float multiplier = HPMultiplier();
        float toReturn = (CharacterLevel * (GetStat(ControlCodes.PlayerStats_Constitution) / 20.0f) * multiplier * 1.579f) + 10;
        return Mathf.Round(toReturn);
    }
    public float GetMaxMana()
    {
        byte statToUse = (PlayerClass)CharacterClass == PlayerClass.Cleric ? GetStat(ControlCodes.PlayerStats_Charisma) : GetStat(ControlCodes.PlayerStats_Intellect);
        float manaMultiplier = 1 + ((statToUse - 10) * 0.05f);
        return ((_characterLevel * 4) + 10) * manaMultiplier;
    }
    public void UpdateSkillsTable(int skills)
    {
        bool[] skillArray = new bool[32];
        int id = 0;
        string binary = "";
        //Debug.Log("Skills base 10: " + skills);
        while(skills != 0)
        {
            bool result = skills % 2 != 0;
            skillArray[id] = result;
            skills = skills / 2;
            binary = (result ? "1" : "0") + binary;
            id++;
        }
        //Debug.Log("Skills base 2: " + binary +", length = " + binary.Length);
        _skills.Clear();
        foreach(byte discipline in SharedFunctions.DisciplinesByClass((PlayerClass)_characterClass))
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
            //Debug.Log("Adding skill " + (byte)discipline + ": " + value);
            _skills.Add(discipline, value);
        }
    }
}
