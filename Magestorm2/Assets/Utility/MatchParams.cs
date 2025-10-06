using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MatchParams
{
    public static Team MatchTeam;
    public static byte MatchTeamID;
    public static byte IDinMatch;
    public static byte SceneID;
    public static int ListeningPort;
    public static long ExpirationTime;
    public static bool ReturningFromMatch;

    private static byte[] _poolData;
    public static void Init(byte[] decrypted)
    {
        ReturningFromMatch = false;
        SceneID = decrypted[1];
        MatchTeamID = decrypted[2];
        IDinMatch = decrypted[3];
        ListeningPort = BitConverter.ToInt32(decrypted, 4);
        MatchTeam = (Team)MatchTeamID;
        byte numPools = decrypted[8];
        _poolData = new byte[numPools * 3];
        Array.Copy(decrypted, 9, _poolData, 0, _poolData.Length);
    }

    public static byte[] GetPoolData()
    {
        return _poolData;
    }
}

