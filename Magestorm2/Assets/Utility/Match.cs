using System.Collections.Generic;
using UnityEngine;

public static class Match
{
    private static Dictionary<byte, Avatar> _matchPlayers;

    public static bool ChatMode;
    public static bool Running;

    public static void Init()
    {
        _matchPlayers = new Dictionary<byte, Avatar>();
    }

    public static void AddAvatar(Avatar avatar)
    {
        _matchPlayers.Add(avatar.PlayerID, avatar);
    }
    public static void RemoveAvatar(byte ID)
    {
        _matchPlayers.Remove(ID);
    }
    public static Dictionary<byte, Avatar> GetMatchPlayers()
    {
        return _matchPlayers;
    }
    public static List<Avatar> GetPlayersOfTeam(Team team)
    {
        List<Avatar> list = new List<Avatar>();
        foreach (Avatar avatar in _matchPlayers.Values)
        {
            if (avatar.PlayerTeam == team)
            {
                list.Add(avatar);
            }
        }
        return list;
    }
    public static List<Avatar> GetSortedPlayers()
    {
        List<Avatar> toReturn = new List<Avatar>();
        for (byte b = 0; b < 4; b++)
        {
            toReturn.AddRange(GetPlayersOfTeam((Team)b));
        }
        toReturn.Sort();
        return toReturn;
    }
}
