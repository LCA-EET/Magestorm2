using UnityEngine;
public enum SpellType : byte
{
    Projectile = 1,
    Bolt = 2,
    SelfHeal = 3,
    HealOther = 4
}
public enum SpellDiscipline: byte
{
    FireLaw = 0,
    IceLaw = 1,
    EarthLaw = 2,
    Brilliance = 3,
    Displacement = 4,
    Psionics = 5,
    Supplication = 6,
    Healing = 7,
    Smiting = 8,
    ManaLaw = 9,
    VoidLaw = 10,
    Sigils = 11,
    SpiritLaw = 12,
    Barriers = 13,
    Shielding = 14
}
public enum TriggerType : byte
{
    ManaPool = 0,
    Shrine = 1,
    LeyInfluencer = 2
}
public enum PlayerStats : byte
{
    Strength = 0,
    Dexterity = 1,
    Constitution = 2,
    Intellect = 3,
    Charisma = 4,
    Wisdom = 5
}
public enum PlayerClass : byte
{
    Arcanist = 0,
    Cleric = 1,
    Magician = 2,
    Mentalist = 3
}
public enum PlayerIndicator : byte
{
    Health = 0,
    Mana = 1,
    Ley = 2,
    Stamina = 3
}
public enum Team : byte
{
    Neutral = 0,
    Chaos = 1,
    Balance = 2,
    Order = 3
}

public enum FormResult : byte
{
    Pending = 0,
    Yes = 1,
    No = 2
}

public enum MatchTypes : byte
{
    Deathmatch = 0,
    FreeForAll = 1,
    CaptureTheFlag=2
}

public enum EffectCode : byte
{
    Haste = 0,
    Slow = 1,
    Freezing = 2,
    Burning = 3,
    Shocked = 4,
    Entangle = 5,
    FireShield = 6,
    IceShield = 7,
    ElectricShield = 8,
    EarthShield = 9,
    Bleeding = 10,
    Prayer = 11
}
