using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static bool ChatMode;
    private static Dictionary<byte, Avatar> _matchPlayers;
    public static void Init()
    {
        ChatMode = false;
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
}
