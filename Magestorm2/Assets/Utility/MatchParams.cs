using System;
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
    public static void Init(byte[] decrypted)
    {
        _decrypted = decrypted;
        ReturningFromMatch = false;
        MatchType = decrypted[1];
        SceneID = decrypted[2];
        IDinMatch = decrypted[3];
        Debug.Log("ID IN MATCH: " + IDinMatch);
        MatchTeamID = _decrypted[4];
        MatchID = _decrypted[5];
        RemotePort = BitConverter.ToInt32(_decrypted, 6);
        _maxHP = BitConverter.ToSingle(_decrypted, 10);
        _maxMana = BitConverter.ToSingle(_decrypted, 14);
        _maxStamina = _decrypted[18];
        MatchTeam = (Team)MatchTeamID;
        byte type = MatchType;
        switch (type)
        {
            case ControlCodes.MatchTypes_DeathMatch:
                InitDM();
                break;
            case ControlCodes.MatchTypes_FreeForAll:
                InitFFA();
                break;
            case ControlCodes.MatchTypes_CaptureTheFlag:
                InitCTF();
                break;
        }
    }
    public static void InitDM()
    {
        UnityEngine.Debug.Log("InitDM");
        ShrineManager.Init(_decrypted, 19);
        PoolManager.Init(_decrypted, 22);
        TorchManager.Init();
        TriggerManager.Init();
        IncludeShrines = true;
        IncludeFlags = false;
        IncludePools = true;
        IncludeTeams = true;
    }

    public static void InitCTF()
    {
        UnityEngine.Debug.Log("InitCTF");
        IncludeShrines = false;
        IncludeFlags = true;
        IncludePools = true;
        IncludeTeams = true;
        byte flagByteLength = _decrypted[22];
        int index = 23;
        FlagManager.Init(_decrypted, index);
        PoolManager.Init(_decrypted, index + flagByteLength);
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
}

