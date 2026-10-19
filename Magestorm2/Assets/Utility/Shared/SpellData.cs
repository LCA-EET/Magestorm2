using UnityEngine;
using System;
public class SpellData
{
    private byte[] _spellData;

    private static byte Index_SpellID = 0;
    private static byte Index_Cost = 1;
    private static byte Index_MinLevel = 2;
    private static byte Index_SkillNeeded = 3;
    private static byte Index_MinDamagePerRoll0 = 4;
    private static byte Index_Element0 = 5;
    private static byte Index_EffectRadius = 6;
    private static byte Index_Range = 7;
    private static byte Index_ProjectileSpeed = 8;
    private static byte Index_ShakePrevention = 9;
    private static byte Index_ForceDuration = 10;
    private static byte Index_AutoAim = 11;
    private static byte Index_Discipline = 12;
    private static byte Index_SpellType = 13;
    private static byte Index_SpellNameReference = 14;
    private static byte Index_DescReference = 16;
    
    public SpellData(byte[] spellData)
    {
        _spellData = spellData;
    }
    public byte GetElementalVFXCode()
    {
        switch (Element0)
        {
            case ControlCodes.Element_Fire:
                return ControlCodes.VFX_FireShield;
            case ControlCodes.Element_Ice:
                return ControlCodes.VFX_IceShield;
            case ControlCodes.Element_Earth:
                return ControlCodes.VFX_EarthShield;
            case ControlCodes.Element_Electric:
                return ControlCodes.VFX_ElectricShield;
        }
        return ControlCodes.VFX_None;
    }
    public byte Element0
    {
        get { return _spellData[Index_Element0]; }
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
    public byte ForceDuration
    {
        get { return _spellData[Index_ForceDuration]; }
    }
    public byte ShakePrevention
    {
        get { return _spellData[Index_ShakePrevention]; }
    }
    public byte ProjectileSpeed
    {
        get { return _spellData[Index_ProjectileSpeed]; }
    }
    public byte Range
    {
        get { return _spellData[Index_Range]; }
    }
    public byte EffectRadius
    {
        get { return _spellData[Index_EffectRadius]; }
    }
    public byte SpellType
    {
        get { return _spellData[Index_SpellType]; }
    }
    public byte Discipline
    {
        get { return _spellData[Index_Discipline]; }
    }
    public byte SpellCost
    {
        get { return _spellData[Index_Cost]; }
    }
    public byte MinLevel
    {
        get { return _spellData[Index_MinLevel]; }
    }
    public byte SkillNeeded
    {
        get { return _spellData[Index_SkillNeeded]; }
    }
    public byte SpellID
    {
        get { return _spellData[Index_SpellID]; }
    }
    public short SpellNameReference
    {
        get { return BitConverter.ToInt16(_spellData, Index_SpellNameReference); }
    }
    public short DescriptionReference
    {
        get { return BitConverter.ToInt16(_spellData, Index_DescReference); }
    }
    public byte AutoAim
    {
        get { return _spellData[Index_AutoAim]; }
    }

    public bool IsFriendly
    {
        get { return _spellData[Index_MinDamagePerRoll0] == 0; }
    }
    public void CastSpell()
    {
        if (ValidCast())
        {
            Debug.Log("Valid cast");
            byte[] toSend = null;
            switch (SpellType)
            {
                case ControlCodes.SpellTypes_HitScan:
                    Vector3 pointHit = Vector3.zero;
                    if(ComponentRegister.PC.HitScanCheck(this, ref pointHit)){
                        toSend = InGame_Packets.HitScanPacket(SpellID, pointHit);
                    }
                    break;
                case ControlCodes.SpellTypes_HarmfulSigil:
                case ControlCodes.SpellTypes_Sigil:
                    toSend = InGame_Packets.PlacedSigilPacket(SpellID, SpellType);
                    break;
                case ControlCodes.SpellTypes_Projectile:
                    toSend = InGame_Packets.ProjectileCastPacket(SpellID);
                    break;
                case ControlCodes.SpellTypes_PBAoE:
                case ControlCodes.SpellTypes_Self:
                case ControlCodes.SpellTypes_SelfHeal:
                case ControlCodes.SpellTypes_SelfResist:
                    toSend = InGame_Packets.GenericCastPacket(0, SpellID, SpellType);
                    break;
                case ControlCodes.SpellTypes_Summon:
                    toSend = ComponentRegister.PC.Summon(SpellID);
                    break;
                case ControlCodes.SpellTypes_SolidWall:
                case ControlCodes.SpellTypes_NonSolidWall:
                    toSend = InGame_Packets.WallCastPacket(SpellID, SpellType);
                    break;
                case ControlCodes.SpellTypes_Bolt:
                case ControlCodes.SpellTypes_Resistable:
                    SpellData spellReference = null;
                    if(SpellManager.GetSpell(SpellID, ref spellReference))
                    {

                        byte target = (byte)(AutoAim == 1 ? SharedFunctions.GetPlayerInSphereCast(Camera.main.transform.position, spellReference.Range, 2.0f, TeamSelectionCode.Enemy, SpellType==ControlCodes.SpellTypes_Resistable?LayerManager.ResistableObstructionMask:LayerManager.AoEObstructionMask) : 0);
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
    public const string RADIUS = "radius";
    public const string RANGE = "range";
    public const string PROJECTILESPEED = "projectilespeed";
    public const string SHAKEPREVENTION = "shakeprevention";
    public const string FORCEDURATION = "forceduration";
    public const string AUTOAIM = "autoaim";
}