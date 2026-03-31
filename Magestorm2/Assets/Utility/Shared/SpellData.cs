using UnityEngine;

public class SpellData
{
    private int _spellNameReference, _descReference;
    private byte _spellID, _cost, _rolls, _minLevel, _skillNeeded, _minDamagePerRoll0, _maxDamagePerRoll0, _minHealPerRoll, _maxHealPerRoll, _element0, _element1, _staminaCost, _effectRadius, _range, _projectileSpeed, _shakePrevention;
    private byte _discipline;
    private byte _spellType;

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
                case SpellAttributes.ELEMENT0:
                    _element0 = byte.Parse(fieldValue); 
                    break;
                case SpellAttributes.ELEMENT1:
                    _element1 = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.ID:
                    _spellID = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MIN_DAMAGE_PER_ROLL0:
                    _minDamagePerRoll0 = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.MAX_DAMAGE_PER_ROLL0:
                    _maxDamagePerRoll0 = byte.Parse(fieldValue);
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
                    _discipline = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.DESCRIPTION:
                    _descReference = int.Parse(fieldValue);
                    break;
                case SpellAttributes.SPELLTYPE:
                    _spellType = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.EFFECTRADIUS:
                    _effectRadius = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.RANGE:
                    _range = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.PROJECTILESPEED:
                    _projectileSpeed = byte.Parse(fieldValue);
                    break;
                case SpellAttributes.SHAKEPREVENTION:
                    _shakePrevention = byte.Parse(fieldValue);
                    break;
            }
        }
    }
    public float GetStaminaCost(byte characterLevel)
    {
        int difference = characterLevel - MinLevel;
        float staminaCost = 170 - (difference * 8.5f);
        if(staminaCost < 17)
        {
            return 17;
        }
        else
        {
            return staminaCost;
        }
    }
    public byte ShakePrevention
    {
        get { return _shakePrevention; }
    }
    public byte ProjectileSpeed
    {
        get { return _projectileSpeed; }
    }
    public byte Range
    {
        get { return _range; }  
    }
    public byte EffectRadius
    {
        get { return _effectRadius; }
    }
    public byte SpellType
    {
        get { return _spellType; }
    }
    public byte Discipline
    {
        get { return _discipline; }
    }
    public byte SpellCost
    {
        get { return _cost; }
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

    public bool IsFriendly
    {
        get { return _minDamagePerRoll0 == 0; }
    }
    public void CastSpell()
    {
        if (ValidCast())
        {
            Debug.Log("Valid cast");
            byte[] toSend = null;
            switch (SpellType)
            {
                
                case ControlCodes.SpellTypes_Projectile:
                    toSend = InGame_Packets.ProjectileCastPacket(SpellID);
                    break;
                case ControlCodes.SpellTypes_PBAoE:
                    toSend = InGame_Packets.GenericCastPacket(0, SpellID, ControlCodes.SpellTypes_PBAoE);
                    break;
                case ControlCodes.SpellTypes_Self:
                    toSend = InGame_Packets.GenericCastPacket(0, SpellID, ControlCodes.SpellTypes_Self);
                    break;
                case ControlCodes.SpellTypes_Summon:
                    toSend = ComponentRegister.PC.Summon(SpellID);
                    break;
                case ControlCodes.SpellTypes_Bolt:
                    SpellData spellReference = null;
                    if(SpellManager.GetSpell(SpellID, ref spellReference))
                    {
                        byte target = SharedFunctions.GetPlayerInSphereCast(Camera.main.transform.position, spellReference.Range, 3.0f, TeamSelectionCode.Enemy);
                        toSend = InGame_Packets.CastBoltPacket(SpellID, target);
                    }
                    break;
                    
            }
            if (toSend != null)
            {
                Debug.Log("Sending spell bytes.");
                Game.SendInGameBytes(toSend);
            }
            else
            {
                Debug.Log("Spell bytes is null");
            }
        }
        else
        {
            Debug.Log("Invalid cast");
        }
    }
    
    private bool ValidCast()
    {
        bool toReturn = false;
        PlayerCharacter caster = PlayerAccount.SelectedCharacter;
        if(caster.GetSkillLevel(Discipline) >= SkillNeeded)
        {
            PC pc = ComponentRegister.PC;
            byte casterLevel = caster.CharacterLevel;
            if(casterLevel >= MinLevel)
            {
                if(pc.CurrentMana >= SpellCost && 
                    pc.CurrentStamina >= GetStaminaCost(casterLevel))
                {
                    toReturn = true;
                }
            }
        } 
        return toReturn;
    }
}
public static class SpellAttributes
{
    public const string ID = "id";
    public const string NAME = "spellname";
    public const string MIN_DAMAGE_PER_ROLL0 = "mindamageperroll0";
    public const string MAX_DAMAGE_PER_ROLL0 = "maxdamageperroll0";
    public const string MIN_HEAL_PER_ROLL = "minhealperroll";
    public const string MAX_HEAL_PER_ROLL = "maxhealperroll";
    public const string ELEMENT = "element0";
    public const string COST = "cost";
    public const string SPELLTYPE = "spelltype";
    public const string DESCRIPTION = "description";
    public const string DISCIPLINE = "school";
    public const string SKILLNEEDED = "skillneeded";
    public const string ROLLS = "rolls";
    public const string SPELL_NAME_REFERENCE = "spellnameref";
    public const string MINLEVEL = "minlevel";
    public const string MIN_DAMAGE_PER_ROLL1 = "mindamageperroll1";
    public const string MAX_DAMAGE_PER_ROLL1 = "maxdamageperroll1";
    public const string ELEMENT0 = "element0";
    public const string ELEMENT1 = "element1";
    public const string EFFECTRADIUS = "effectradius";
    public const string RANGE = "range";
    public const string PROJECTILESPEED = "projectilespeed";
    public const string SHAKEPREVENTION = "shakeprevention";
}