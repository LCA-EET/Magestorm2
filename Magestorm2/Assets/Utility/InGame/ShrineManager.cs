using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ShrineManager
{
    private static Dictionary<byte, Shrine> _shrines;
    private static Dictionary<Team, byte> _shrineData;
    public static void Init(byte[] decrypted, int index)
    {
        _shrines = new Dictionary<byte, Shrine>();
        _shrineData = new Dictionary<Team, byte>();
        _shrineData.Add(Team.Chaos, decrypted[index]);
        _shrineData.Add(Team.Balance, decrypted[index+1]);
        _shrineData.Add(Team.Order, decrypted[index+2]);
    }

    public static void RegisterShrine(Shrine toRegister)
    {
        _shrines.Add((byte)toRegister.Team, toRegister);
        toRegister.SetHealth(_shrineData[toRegister.Team]);
    }

    public static void ProcessShrineAdjustment(byte shrineID, byte newHealth, byte adjuster)
    {
        if (_shrines.ContainsKey(shrineID))
        {
            _shrines[shrineID].AdjustHealth(newHealth, adjuster);
        }

    }
}
