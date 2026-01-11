using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpellData
{
    private int _spellNameReference, _descReference;
    private byte _spellID, _cost, _rolls, _minLevel, _skillNeeded, _minDamagePerRoll, _maxDamagePerRoll, _minHealPerRoll, _maxHealPerRoll;
    private SpellDiscipline _discipline;
    private SpellType _spellType;

    public SpellData(string[] fields, string contents)
    {
        string[] split = contents.Split("<br>");
        string fieldID, fieldValue;
        for (int i = 0; i < fields.Length; i++)
        {
            fieldID = fields[i];
            fieldValue = split[i + 1];
            switch (fieldID)
            {
                case SpellAttributes.ID:
                    _spellID = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MIN_DAMAGE_PER_ROLL:
                    _minDamagePerRoll = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MAX_DAMAGE_PER_ROLL:
                    _maxDamagePerRoll = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MIN_HEAL_PER_ROLL:
                    _minHealPerRoll = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MAX_HEAL_PER_ROLL:
                    _maxHealPerRoll = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.SPELL_NAME_REFERENCE:
                    _spellNameReference = int.Parse(fieldValue);
                    break;
                case SpellAttributes.COST:
                    _cost = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.ROLLS:
                    _rolls = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MINLEVEL:
                    _minLevel = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.SKILLNEEDED:
                    _skillNeeded = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.DISCIPLINE:
                    _discipline = (SpellDiscipline)byte.Parse(fieldValue);
                    break;
                case SpellAttributes.DESCRIPTION:
                    _descReference = int.Parse(fieldValue);
                    break;
                case SpellAttributes.SPELLTYPE:
                    _spellType = (SpellType)byte.Parse(fieldValue);
                    break;
            }
        }
    }
    public SpellType SpellType
    {
        get { return _spellType; }
    }
    public SpellDiscipline Discipline
    {
        get { return _discipline; }
    }
    public byte MinLevel
    {
        get { return _minLevel; }
    }
    public byte SkillNeeded
    {
        get { return _skillNeeded; }
    }
    public byte SpellID
    {
        get { return _spellID; }
    }
    public int SpellNameReference
    {
        get { return _spellNameReference; }
    }
    public int DescriptionReference
    {
        get { return _descReference; }
    }
    public void CastSpell()
    {
        byte[] toSend = null;
        switch (SpellType)
        {
            case SpellType.Projectile:
                toSend = InGame_Packets.ProjectileCastPacket(SpellID);
                break;
            case SpellType.SelfHeal:
                break;
        }
        if(toSend != null)
        {
            Game.SendInGameBytes(toSend);
        }
}
public static class SpellAttributes
{
    public const string ID = "id";
    public const string NAME = "spellname";
    public const string MIN_DAMAGE_PER_ROLL = "mindamageperroll";
    public const string MAX_DAMAGE_PER_ROLL = "maxdamageperroll";
    public const string MIN_HEAL_PER_ROLL = "minhealperroll";
    public const string MAX_HEAL_PER_ROLL = "maxhealperroll";
    public const string ELEMENT = "element";
    public const string COST = "cost";
    public const string SPELLTYPE = "spelltype";
    public const string DESCRIPTION = "description";
    public const string DISCIPLINE = "school";
    public const string SKILLNEEDED = "skillneeded";
    public const string ROLLS = "rolls";
    public const string SPELL_NAME_REFERENCE = "spellnameref";
    public const string MINLEVEL = "minlevel";
}