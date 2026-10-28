using System;
using System.Collections;
using System.Collections.Generic;

public class DisciplineData
{
    private byte _id;
    private short _nameRef;
    private byte _availabilityMask;

    private List<SpellData> _spellsOfDiscipline;
    public DisciplineData(byte[] data)
    {
        _id = data[0];
        _availabilityMask = data[1];
        _nameRef = BitConverter.ToInt16(data, 2);
        _spellsOfDiscipline = new List<SpellData>();
        ParseMask();
    }
    
    private void ParseMask()
    {
        BitArray ba = ByteUtils.ByteToBoolArray(_availabilityMask);
        List<byte> classCodes = Game.ActiveCharacterClassManager.CharacterClassCodes;
        foreach(byte classCode in classCodes)
        {
            if (ba[classCode])
            {
                Game.ActiveCharacterClassManager.AssociateDisciplineToClass(classCode, this);
            }
        }
    }
    public void AddSpellToDiscipline(SpellData spell)
    {
        _spellsOfDiscipline.Add(spell);
    }
    public List<SpellData> SpellsOfDiscipline
    {
        get { return _spellsOfDiscipline; }
    }
    public byte DisciplineID
    {
        get { return _id; }
    }
    public string DisciplineName
    {
        get
        {
            return Language.GetBaseString(_nameRef);
        }
    }
}

