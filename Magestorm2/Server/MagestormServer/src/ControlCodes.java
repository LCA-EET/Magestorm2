public class ControlCodes {

    public static final byte CastPayloadStartIndex = 5;

    //region EffectTarget
        public static final byte EffectTarget_SpellTarget = 0;
        public static final byte EffectTarget_Caster = 1;
    //endregion

    //region AsyncDBUpdate
        public static final byte AsyncDBUpdate_Experience = 0;
        public static final byte AsyncDBUpdate_Slotting = 1;
    //endregion

    //region MatchOptions
        public static final byte MatchOptions_FastRegen = 0;
        public static final byte MatchOptions_NoSolidWalls = 1;
        public static final byte MatchOptions_AntiStack = 2;
        public static final byte MatchOptions_NoResurrection = 3;
        public static final byte MatchOptions_NoHealOther = 4;
    //endregion

    //region LogIDs
        public static final byte LogID_Main = 1;
        public static final byte LogID_Error = 2;
        public static final byte LogID_Debug = 3;
        public static final byte LogID_Chat = 4;
    //endregion

    //region SkillLevels
        public static final byte SkillLevel_Basic = 1;
        public static final byte SkillLevel_Expert = 2;
        public static final byte SkillLevel_Master = 3;
    //

    //region EffectNotification
        public static final byte EffectNotification_All = 0;
        public static final byte EffectNotification_EffectTarget = 1;
        public static final byte EffectNotification_EffectCaster = 2;
    //endregion

    //region SpellNotification
        public static final byte SpellNotification_All = 0;
        public static final byte SpellNotification_TeamOnly = 1;
        public static final byte SpellNotification_CasterOnly = 2;
        public static final byte SpellNotification_Payload = 3;
    //

    //region SpellTypes
        public static final byte SpellTypes_Projectile = 1;
        public static final byte SpellTypes_Self = 2;
        public static final byte SpellTypes_Summon = 3;
        public static final byte SpellTypes_Bolt = 4;
        public static final byte SpellTypes_PBAoE = 5;
        public static final byte SpellTypes_SelfHeal = 6;
        public static final byte SpellTypes_SelfResist = 7;
        public static final byte SpellTypes_NonSolidWall = 8;
        public static final byte SpellTypes_SolidWall = 9;
        public static final byte SpellTypes_Resistable = 10;
    //endregion

    //region Discipline
        public static final byte Discipline_Necromancy = 12;
    //endregion

    //region Character Status
        public static final byte CharacterStatus_Deactivated = 0;
        public static final byte CharacterStatus_Activated = 1;
    //endregion

    //region EffectCodes

        public static final byte EffectCode_Slow = 1;
        public static final byte EffectCode_Freeze = 2;
        public static final byte EffectCode_Burn = 3;
        public static final byte EffectCode_Shock = 4;
        public static final byte EffectCode_Entangle = 5;
        public static final byte EffectCode_FireShield = 6;
        public static final byte EffectCode_IceShield = 7;
        public static final byte EffectCode_ElectricShield = 8;
        public static final byte EffectCode_EarthShield = 9;
        public static final byte EffectCode_Bleed = 10;
        public static final byte EffectCode_Prayer = 11;
        public static final byte EffectCode_Haste = 12;
        public static final byte EffectCode_ViewTeam = 13;
        public static final byte EffectCode_ViewAll = 14;
        public static final byte EffectCode_Fly = 15;
    //endregion

    //region EffectTypes
        public static final byte EffectType_Unset = 0;
        public static final byte EffectType_Shield = 1;
        public static final byte EffectType_DamageOverTime = 2;
        public static final byte EffectType_HealOverTime = 3;
    //endregion
    //region ElementCodes
        public static final byte Element_None = 0;
        public static final byte Element_Fire = 1;
        public static final byte Element_Ice = 2;
        public static final byte Element_Earth = 3;
        public static final byte Element_Electric = 4;
        public static final byte Element_Light = 5;
        public static final byte Element_Dark = 6;
        public static final byte Element_Physical = 7;
        public static final byte Element_Mana = 8;
        public static final byte Element_Void = 9;
    //endregion

    //region Statistics
        public static final byte Statistic_Strength = 0;
        public static final byte Statistic_Dexterity  = 1;
        public static final byte Statistic_Constitution  = 2;
        public static final byte Statistic_Intellect  = 3;
        public static final byte Statistic_Charisma  = 4;
        public static final byte Statistic_Wisdom  = 5;
    //
}
