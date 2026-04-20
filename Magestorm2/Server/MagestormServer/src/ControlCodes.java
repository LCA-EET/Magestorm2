public class ControlCodes {

    public static final byte CastPayloadStartIndex = 5;

    //region SkillLevels
        public static final byte SkillLevel_Basic = 1;
        public static final byte SkillLevel_Expert = 2;
        public static final byte SkillLevel_Master = 3;
    //

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
    //endregion

    //region Discipline
        public static final byte Discipline_FireLaw = 0;
        public static final byte Discipline_IceLaw = 1;
        public static final byte Discipline_EarthLaw = 2;
        public static final byte Discipline_Brilliance = 3;
        public static final byte Discipline_Displacement = 4;
        public static final byte Discipline_Psionics = 5;
        public static final byte Discipline_Supplication = 6;
        public static final byte Discipline_Healing = 7;
        public static final byte Discipline_Smiting = 8;
        public static final byte Discipline_ManaLaw = 9;
        public static final byte Discipline_VoidLaw = 10;
        public static final byte Discipline_Sigils = 11;
        public static final byte Discipline_SpiritLaw = 12;
        public static final byte Discipline_Barriers = 13;
        public static final byte Discipline_Shielding = 14;
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
