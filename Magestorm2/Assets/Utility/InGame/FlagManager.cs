using System.Collections.Generic;
using UnityEngine;

public static class FlagManager
{
    public static Team FlagHeldByPlayer = Team.Neutral;
    private static Dictionary<Team, Flag> _flagTable;
    public static void Init()
    {
        _flagTable = new Dictionary<Team, Flag>();
    }

    public static void Register(Flag toRegister)
    {
        _flagTable.Add(toRegister.Team, toRegister);
    }

    public static void RepositionFlag(Team toReposition, Vector3 worldPosition)
    {
        _flagTable[toReposition].Reposition(worldPosition);
    }

    public static void ReturnFlag(Team toReturn)
    {
        _flagTable[toReturn].FlagReturned();
    }
}
