using UnityEngine;
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


