using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Match
{
    private static Dictionary<byte, Avatar> _matchPlayers;
    private static Dictionary<byte, ActivateableObject> _objects;
    public static bool Running;
    
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
        if (_matchPlayers.ContainsKey(ID))
        {
            Avatar toRemove = _matchPlayers[ID];
            MessageData md = new MessageData(toRemove.Name + " has left the match.", "Server");
            _matchPlayers.Remove(ID);
        }
        
    }
    public static void RegisterActivateableObject(ActivateableObject obj)
    {
        Debug.Log("AO " + obj.ObjectKey + " registered.");
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
        //Debug.Log("AL Count: " +  toReturn.Count); 
        return toReturn;
    }
    public static void ProcessObjectStates(byte[] decrypted)
    {
        for(int i = 1; i < decrypted.Length; i+=2)
        {
            ChangeObjectState(decrypted[i], decrypted[i+1], true);
        }
    }
    public static void ProcessPlayerJoinedPacket(byte[] decrypted)
    {
        byte playerID = decrypted[1];
        byte teamID = decrypted[2];
        byte[] appearance = new byte[5];
        System.Array.Copy(decrypted, 3, appearance, 0, appearance.Length);
        byte level = decrypted[8];
        byte characterClass = decrypted[9];
        byte[] nameBytes = new byte[decrypted[10]];
        System.Array.Copy(decrypted, 11, nameBytes, 0, nameBytes.Length);
        string name = ByteUtils.BytesToUTF8(nameBytes, 0, nameBytes.Length);
        Avatar added = ComponentRegister.Spawner.SpawnAvatar();
        added.SetAttributes(playerID, name, level, characterClass, (Team)teamID, appearance);
        MessageData md = new MessageData(name + " has joined the match.", "Server");
        AddAvatar(added);
        if (playerID == MatchParams.IDinMatch)
        {
            ComponentRegister.PC.JoinedMatch = true;
        }
    }
    public static void UpdatePlayerLocation(byte[] decrypted)
    {
        byte playerID = decrypted[1];
        if(playerID != MatchParams.IDinMatch)
        {
            byte controlCode = decrypted[2];
            if (_matchPlayers.ContainsKey(playerID))
            {
                Avatar toUpdate = _matchPlayers[playerID];
                
                switch (controlCode)
                {
                    case 0: // position only
                        _matchPlayers[playerID].UpdatePosition(decrypted, false);
                        break;
                    case 1: // direction only
                        _matchPlayers[playerID].UpdateDirection(decrypted, 3, false);
                        break;
                    case 2: // position and direction
                        _matchPlayers[playerID].UpdatePosition(decrypted, false);
                        _matchPlayers[playerID].UpdateDirection(decrypted, 15, false);
                        break;
                }
            }
            else
            {
                Game.SendInGameBytes(InGame_Packets.FetchPlayerPacket(playerID));
            }
        }
    }
    public static void ChangeObjectState(byte key, byte state, bool force)
    {
        if (_objects.ContainsKey(key))
        {
            _objects[key].StatusChanged(state, force);
            Debug.Log("Object state change: " + key + ", " + state);
        }
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
