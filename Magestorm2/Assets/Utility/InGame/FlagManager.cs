using System;
using System.Collections.Generic;
using UnityEngine;

public static class FlagManager
{
    public static Team FlagHeldByPlayer = Team.Neutral;
    private static Dictionary<byte, FlagData> _flagData;
    private static Dictionary<Team, Flag> _flagTable;
    private static Dictionary<Team, byte> _scores;
    public static void Init(byte[] decrypted, int index)
    {
        _flagTable = new Dictionary<Team, Flag>();
        _flagData = new Dictionary<byte, FlagData>();
        _scores = new Dictionary<Team, byte>();
        _scores.Add(Team.Chaos, decrypted[index-4]);
        _scores.Add(Team.Balance, decrypted[index - 3]);
        _scores.Add(Team.Order, decrypted[index - 2]);
        while (_flagData.Count < 3)
        {
            Debug.Log("Index: " + index);
            FlagData toAdd = new FlagData(decrypted, index);
            _flagData.Add(toAdd.Team, toAdd);
            index = toAdd.EndIndex;
        }
    }
    public static byte GetScore(Team team)
    {
        return _scores[team];
    }
    public static void SetScore(Team team, byte newScore)
    {
        _scores[team] = newScore;
    }
    public static void Register(Flag toRegister)
    {
        _flagTable.Add(toRegister.Team, toRegister);
        FlagData data = _flagData[(byte)toRegister.Team];
        if(data.HolderID == FlagData.DROPPED)
        {
            RepositionFlag(toRegister.Team, data.Position);
        }
        else if(data.HolderID != FlagData.NOT_HELD)
        {
            toRegister.FlagTaken();
        }
    }

    public static void RepositionFlag(Team toReposition, Vector3 worldPosition)
    {
        _flagTable[toReposition].Reposition(worldPosition);
    }

    public static void ReturnFlag(Team toReturn)
    {
        _flagTable[toReturn].FlagReturned();
    }

    public static void FlagTaken(Team taken)
    {
        _flagTable[taken].FlagTaken();
    }
}
