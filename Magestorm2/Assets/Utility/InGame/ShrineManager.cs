using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ShrineManager
{
    private static Dictionary<byte, Shrine> _shrines;

    public static void Init()
    {
        _shrines = new Dictionary<byte, Shrine>();
    }

    public static void RegisterShrine(Shrine toRegister)
    {
        _shrines.Add((byte)toRegister.Team, toRegister);
        toRegister.SetHealth(MatchParams.GetShrineHealth((byte)toRegister.Team));
    }

    public static void ProcessShrineAdjustment(byte shrineID, byte newHealth, byte adjuster)
    {
        if (_shrines.ContainsKey(shrineID))
        {
            _shrines[shrineID].AdjustHealth(newHealth, adjuster);
        }

    }
}
