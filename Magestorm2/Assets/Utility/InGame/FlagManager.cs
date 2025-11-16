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
        Debug.Log("FlagManager Init.");
        _flagTable = new Dictionary<Team, Flag>();
        _flagData = new Dictionary<byte, FlagData>();
        _scores = new Dictionary<Team, byte>();
        for (int i = 0; i < decrypted.Length; i++)
        {
            Debug.Log(i + ": " + decrypted[i]);
        }
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
    public static sbyte GetScore(Team team)
    {
        sbyte toReturn = 0;
        byte rawScore = _scores[team];
        if (rawScore > 127){
            toReturn = (sbyte)(rawScore - 256);
        }
        else
        {
            toReturn = (sbyte)rawScore;
        }
        return toReturn;
    }
    public static void SetScore(Team team, byte newScore)
    {
        _scores[team] = newScore;
    }
    public static void Register(Flag toRegister)
    {
        Debug.Log("Registering Flag " + toRegister.name);
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
        worldPosition.y += 1;
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
