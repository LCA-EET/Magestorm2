using System.Collections.Generic;
using UnityEngine;
public static class ShrineManager
{
    private static Dictionary<byte, Shrine> _shrines;
    private static Dictionary<Team, byte> _shrineData;
    private static Team _winning;
    public static void Init(byte[] decrypted, int index)
    {
        _shrines = new Dictionary<byte, Shrine>();
        _shrineData = new Dictionary<Team, byte>();
        _shrineData.Add(Team.Chaos, decrypted[index]);
        _shrineData.Add(Team.Balance, decrypted[index+1]);
        _shrineData.Add(Team.Order, decrypted[index+2]);
        
    }

    public static void CheckVictoryCondition()
    {
        byte fullHealth = 0;
        byte destroyed = 0;
        foreach (Shrine shrine in _shrines.Values)
        {
            if (shrine.BiasAmount == 100)
            {
                fullHealth++;
                //Debug.Log("CVC: " + shrine.Team + " at full health.");
                _winning = shrine.Team;
            }
            else if (shrine.BiasAmount == 0)
            {
                //Debug.Log("CVC: " + shrine.Team + " is destroyed.");
                destroyed++;
            }
        }
        if (fullHealth == 1 && destroyed == _shrines.Count - 1)
        {
            ComponentRegister.ShrinePanel.ShowVictoryNotification(_winning);
        }
        else
        {
            ComponentRegister.ShrinePanel.HideVictoryNotification();
        }
    }
    public static void RegisterShrine(Shrine toRegister)
    {
        _shrines.Add((byte)toRegister.Team, toRegister);
        toRegister.SetHealth(_shrineData[toRegister.Team]);
        //Debug.Log("Registered Shrine.");
    }

    public static void ProcessShrineAdjustment(byte shrineID, byte newHealth, byte adjuster)
    {
        if (_shrines.ContainsKey(shrineID))
        {
            _shrines[shrineID].AdjustHealth(newHealth, adjuster);
        }

    }
    public static bool IsShrineAlive(Team team)
    {
        Shrine toCheck = GetShrine(team);
        if(toCheck != null)
        {
            return toCheck.BiasAmount > 0;
        }
        else
        {
            return false;
        }
    }
    public static Shrine GetShrine(Team team)
    {
        if (_shrineData.ContainsKey(team))
        {
            return _shrines[(byte)team];
        }
        return null;
    }
}
