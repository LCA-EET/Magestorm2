using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MatchParams
{
    public static Team MatchTeam;
    public static byte MatchTeamID;
    public static byte IDinMatch;
    public static byte SceneID;
    public static byte MatchType;
    public static int ListeningPort;
    public static long ExpirationTime;
    public static bool ReturningFromMatch;

    private static byte[] _decrypted;
    private static byte[] _poolData;
    private static byte[] _shrineData;
    private static bool _includePools;
    private static bool _includeShrines;
    private static bool _includeTeams;
    private static bool _includeFlags;

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
        MatchTeamID = _decrypted[4];
        ListeningPort = BitConverter.ToInt32(_decrypted, 5);
        MatchTeam = (Team)MatchTeamID;
        MatchTypes type = (MatchTypes)MatchType;
        switch (type)
        {
            case MatchTypes.Deathmatch:
                InitDM();
                break;
            case MatchTypes.FreeForAll:
                InitFFA();
                break;
            case MatchTypes.CaptureTheFlag:
                InitCTF();
                break;
        }
    }
    public static void InitDM()
    {
        UnityEngine.Debug.Log("InitDM");
        _shrineData = new byte[3];
        _shrineData[0] = _decrypted[9];
        _shrineData[1] = _decrypted[10];
        _shrineData[2] = _decrypted[11];
        byte numPools = _decrypted[12];
        _poolData = new byte[numPools * 3];
        Array.Copy(_decrypted, 13, _poolData, 0, _poolData.Length);
        IncludeShrines = true;
        IncludeFlags = false;
        IncludePools = true;
        ShrineManager.Init();
        PoolManager.Init();
    }

    public static void InitCTF()
    {
        UnityEngine.Debug.Log("InitCTF");
        IncludeShrines = false;
        IncludeFlags = true;
        IncludePools = true;
        byte[] scores = new byte[3];
        scores[0] = _decrypted[9];
        scores[1] = _decrypted[10];
        scores[2] = _decrypted[11];
        FlagManager.Init();
        PoolManager.Init();
    }

    public static void InitFFA()
    {
        UnityEngine.Debug.Log("InitFFA");
        ReturningFromMatch = false;
        MatchTeamID = 0;
        
        IncludeShrines = false;
        IncludeFlags = false;
        IncludePools = false;
    }

    public static byte[] GetPoolData()
    {
        return _poolData;
    }

    public static byte[] GetShrineData()
    {
        return _shrineData;
    }

    public static byte GetShrineHealth(byte teamID)
    {
        return _shrineData[teamID-1];
    }
}

