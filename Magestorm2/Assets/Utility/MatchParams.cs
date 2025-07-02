using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MatchParams
{
    public static byte MatchTeamID;
    public static byte IDinMatch;
    public static byte SceneID;
    public static int ListeningPort;
    public static long ExpirationTime;
    public static void Init(byte[] decrypted)
    {
        SceneID = decrypted[1];
        MatchTeamID = decrypted[2];
        IDinMatch = decrypted[3];
        ListeningPort = BitConverter.ToInt32(decrypted, 4);
        
    }
}

