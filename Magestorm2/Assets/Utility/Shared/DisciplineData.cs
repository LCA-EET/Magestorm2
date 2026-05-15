using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DisciplineData
{
    private byte _id;
    private short _nameRef;
    private byte _availabilityMask;
    private byte _statCode;

    private List<SpellData> _spellsOfDiscipline;

    public DisciplineData(string[] fields, string contents)
    {
        _spellsOfDiscipline = new List<SpellData>();
        string[] split = contents.Split("<br>");
        string fieldID, fieldValue;
        for (int i = 0; i < fields.Length; i++)
        {
            fieldID = fields[i];
            fieldValue = split[i + 1];
            switch (fieldID)
            {
                case DisciplineAttributes.ID:
                    _id = byte.Parse(fieldValue);
                    break;
                case DisciplineAttributes.NAMEREFERENCE:
                    _nameRef = short.Parse(fieldValue);
                    break;
                case DisciplineAttributes.AVAILABILITYMASK:
                    _availabilityMask = byte.Parse(fieldValue);
                    ParseMask();
                    break;
            }
        }
    }
    private void ParseMask()
    {
        BitArray ba = ByteUtils.ByteToBoolArray(_availabilityMask);
        List<byte> classCodes = CharacterClassManager.CharacterClassCodes;
        foreach(byte classCode in classCodes)
        {
            if (ba[classCode])
            {
                CharacterClassManager.AssociateDisciplineToClass(classCode, this);
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

public static class DisciplineAttributes
{
    public const string ID = "id";
    public const string NAME = "name";
    public const string AVAILABILITYMASK = "availabilitymask";
    public const string STATCODE = "statcode";
    public const string NAMEREFERENCE = "nameref";
}