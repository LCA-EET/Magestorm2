using System;
using System.Numerics;
using UnityEngine;
public static class MatchParams
{
    public static Team MatchTeam;
    public static byte MatchTeamID;
    public static byte MatchID;
    public static byte IDinMatch;
    public static byte SceneID;
    public static byte MatchType;
    public static int RemotePort;
    public static long ExpirationTime;
    public static bool ReturningFromMatch;

    private static byte[] _decrypted;
    private static bool _includePools;
    private static bool _includeShrines;
    private static bool _includeTeams;
    private static bool _includeFlags;
    private static float _maxHP;
    private static float _maxMana;
    private static byte _maxStamina;
    public static bool JoinedMatch;
    public static byte WinningTeamID;
    public static byte[] MatchKey;

    public static bool IncludeFlags{
        get { return _includeFlags;}
        set { _includeFlags = value; }
    }
    public static bool IncludePools
    {
        get { return _includePools; }
        set { _includePools = value; }
    }
    public static bool IncludeTeams
    {
        get { return _includeTeams; }
        set { _includeTeams = value; }
    }
    public static bool IncludeShrines
    {
        get { return _includeShrines; }
        set { _includeShrines = value; }
    }
    public static bool Init(byte[] decrypted)
    {
        JoinedMatch = false;
        _decrypted = decrypted;
        ReturningFromMatch = false;
        MatchType = decrypted[1];
        SceneID = decrypted[2];
        IDinMatch = decrypted[3];
        Debug.Log("ID IN MATCH: " + IDinMatch);
        MatchTeamID = _decrypted[4];
        MatchID = _decrypted[5];
        RemotePort = BitConverter.ToInt32(_decrypted, 6);
        MatchKey = new byte[16];
        Array.Copy(decrypted, 10, MatchKey, 0, 16);
        BigInteger matchKeyBI = new BigInteger(MatchKey, false, true);
        Debug.Log("Match Key (Hex): " + matchKeyBI.ToString("X"));
        _maxHP = BitConverter.ToSingle(_decrypted, 26);
        _maxMana = BitConverter.ToSingle(_decrypted, 30);
        _maxStamina = _decrypted[34];
        MatchParams.ExpirationTime = BitConverter.ToInt64(_decrypted, 35);
        MatchTeam = (Team)MatchTeamID;
        byte type = MatchType;
        int index = 43;
        switch (type)
        {
            case ControlCodes.MatchTypes_DeathMatch:
                InitDM(ref index);
                break;
            case ControlCodes.MatchTypes_FreeForAll:
                InitFFA();
                break;
            case ControlCodes.MatchTypes_CaptureTheFlag:
                InitCTF(ref index);
                break;
        }
        return Cryptographer.VerifyChecksum(decrypted, index);
    }
    public static void InitDM(ref int index)
    {
        UnityEngine.Debug.Log("InitDM");
        ShrineManager.Init(_decrypted, ref index);
        PoolManager.Init(_decrypted, ref index);
        TorchManager.Init();
        TriggerManager.Init();
        IncludeShrines = true;
        IncludeFlags = false;
        IncludePools = true;
        IncludeTeams = true;
    }

    public static void InitCTF(ref int index)
    {
        UnityEngine.Debug.Log("InitCTF");
        IncludeShrines = false;
        IncludeFlags = true;
        IncludePools = true;
        IncludeTeams = true;
        FlagManager.Init(_decrypted, ref index);
        PoolManager.Init(_decrypted, ref index);
    }

    public static void InitFFA()
    {
        UnityEngine.Debug.Log("InitFFA");
        ReturningFromMatch = false;
        MatchTeamID = 0;
        
        IncludeShrines = false;
        IncludeFlags = false;
        IncludePools = false;
        IncludeTeams = false;
    }

    public static float MaxHP
    {
        get
        {
            return _maxHP;
        }
    }

    public static float MaxMana
    {
        get
        {
            return _maxMana;
        }
    }

    public static float MaxStamina
    {
        get
        {
            return _maxStamina;
        }
    }

    public static bool IsCTF
    {
        get
        {
            return MatchType == ControlCodes.MatchTypes_CaptureTheFlag;
        }
    }

    public static bool IsFFA
    {
        get
        {
            return MatchType == ControlCodes.MatchTypes_FreeForAll;
        }
    }

    public static bool IsDM
    {
        get
        {
            return MatchType == ControlCodes.MatchTypes_DeathMatch;
        }
    }
}

