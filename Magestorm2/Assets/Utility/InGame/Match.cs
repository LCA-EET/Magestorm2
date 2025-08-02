using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Match
{
    private static Dictionary<byte, Avatar> _matchPlayers;
    private static Dictionary<byte, ActivateableObject> _objects;

    public static bool ChatMode, MenuMode;
    public static bool Running;
    
    public static bool GameMode
    {
        get
        {
            return !ChatMode && !MenuMode;
        }
    }
    public static void Init()
    {
        _matchPlayers = new Dictionary<byte, Avatar>();
        _objects = new Dictionary<byte, ActivateableObject>(); 
    }

    public static void AddAvatar(Avatar avatar)
    {
        _matchPlayers.Add(avatar.PlayerID, avatar);
    }
    public static void RemoveAvatar(byte ID)
    {
        _matchPlayers.Remove(ID);
    }
    public static void RegisterActivateableObject(ActivateableObject obj)
    {
        _objects.Add(obj.ObjectKey, obj);
    }
    public static bool GetAvatar(byte id, ref Avatar avatar)
    {
        bool toReturn = _matchPlayers.ContainsKey(id);
        avatar = toReturn ? _matchPlayers[id] : null;
        return toReturn;
    }
    public static void LeaveMatch()
    {
        MatchParams.ReturningFromMatch = true;
        SceneManager.LoadScene("Pregame");
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

    public static void ChangeObjectState(byte key, byte state)
    {
        if (_objects.ContainsKey(key))
        {
            _objects[key].StatusChanged(state);
            Debug.Log("Object state change: " + key + ", " + state);
        }
    }
    public static void ChangeShrineHealth(byte shrineID, byte health)
    {
        ComponentRegister.ShrinePanel.SetFill((Team)shrineID, health);
    }

    public static void Send(byte[] packetBytes)
    {
        ComponentRegister.InGamePacketProcessor.SendBytes(packetBytes);
    }
}
