using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Match
{
    private static Dictionary<byte, Avatar> _matchPlayers;
    private static Dictionary<byte, ActivateableObject> _objects;
    private static Dictionary<byte, ManaPool> _pools;
    private static Dictionary<byte, InitialPoolData> _initialPoolData;
    private static Level _level;

    public static bool Running;
    
    
    public static void Init()
    {
        _level = LevelData.GetLevel(MatchParams.SceneID);
        _matchPlayers = new Dictionary<byte, Avatar>();
        _objects = new Dictionary<byte, ActivateableObject>(); 
        _pools = new Dictionary<byte, ManaPool>();
        byte[] poolData = MatchParams.GetPoolData();
        _initialPoolData = new Dictionary<byte, InitialPoolData>();
        for (int i = 0; i < poolData.Length; i += 3)
        {
            _initialPoolData.Add(poolData[i], new InitialPoolData(poolData[i + 1], poolData[i + 2]));
            Debug.Log("Pool ID: " + poolData[i] + ", Team: " + poolData[i + 1] + ", Amount: " + poolData[i+2]);
        }
        
    }
    public static void PoolBiased(byte biaserID, byte poolID, byte teamID, byte biasAmount)
    {
        if (_pools.ContainsKey(poolID))
        {
            _pools[poolID].BiasPool(biasAmount, (Team)teamID, biaserID);
        }
    }
    public static byte RegisterPool(ManaPool toRegister)
    {
        _pools.Add(toRegister.PoolID, toRegister);
        InitialPoolData poolData = _initialPoolData[toRegister.PoolID];
        toRegister.SetBiasAmount(poolData.BiasAmount, poolData.BiasedToward);
        return _level.GetPoolPower(toRegister.PoolID);
    }
    public static void AddAvatar(Avatar avatar)
    {
        _matchPlayers.Add(avatar.PlayerID, avatar);
    }
    public static void RemoveAvatar(byte ID)
    {
        if (_matchPlayers.ContainsKey(ID))
        {
            Avatar toRemove = _matchPlayers[ID];
            MessageData md = new MessageData(toRemove.Name + " has left the match.", "Server");
            _matchPlayers.Remove(ID);
        }
        
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
        UDPBuilder.TerminateClient(MatchParams.ListeningPort);
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

    public static bool PlayerExists(byte playerID, ref Avatar avatar)
    {
        if (_matchPlayers.ContainsKey(playerID))
        {
            avatar = _matchPlayers[playerID];
            return true;
        }
        return false;
    }
}
