using UnityEngine;

public enum TeamSelectionCode
{
    All = 0,
    Friendly = 1,
    Enemy = 2
}

public class Postures
{
    public const byte Standing = 0, 
        Crouched = 1, 
        Airborne = 2, 
        Jump = 3;
}
public class AnimationKeys
{
    public const byte Idle_Standing = 0,
        Idle_Crouching = 1,
        Walk_Forward = 2,
        Walk_Backward = 3,
        Run = 4,
        Jump = 5,
        CrouchWalk_Forward = 6,
        CrouchWalk_Backward = 7,
        Airborne = 8,
        None = 99;
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
