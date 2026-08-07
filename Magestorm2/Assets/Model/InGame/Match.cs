using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Match
{
    private static Dictionary<byte, Avatar> _matchPlayers;
    private static Dictionary<byte, ActivateableObject> _objects;
    private static Dictionary<byte, Avatar> _deadAvatarsOnPCTeam;
    private static Dictionary<short, Wall> _walls;
    private static Dictionary<short, Sigil> _sigils;
    private static Dictionary<short, StoredVector> _storedVectors;
    
    public static void Init()
    {
        _matchPlayers = new Dictionary<byte, Avatar>();
        _storedVectors = new Dictionary<short, StoredVector>();
        _deadAvatarsOnPCTeam = new Dictionary<byte, Avatar>();
        _objects = new Dictionary<byte, ActivateableObject>();
        _walls = new Dictionary<short, Wall>();
        _sigils = new Dictionary<short, Sigil>();
    }

    public static void Reinitialize()
    {
        _matchPlayers.Clear();
        _storedVectors.Clear();
        _deadAvatarsOnPCTeam.Clear();
        _objects.Clear();
        _walls.Clear();
        _sigils.Clear();
    }
    
    public static void AddStoredVector(short castID,  Vector3 vector)
    {
        _storedVectors.Add(castID, new StoredVector(vector));
    }

    public static bool GetStoredVector(short castID, ref Vector3 stored)
    {
        if (_storedVectors.ContainsKey(castID))
        {
            stored = _storedVectors[castID].Vector;
            _storedVectors.Remove(castID);
            return true;
        }
        return false;
    }
    
    public static void CountdownVectors()
    {
        if(_storedVectors.Count > 0)
        {
            long currentTime = DateTime.Now.Ticks;
            List<short> expired = new List<short>();
            foreach (short castID in _storedVectors.Keys)
            {
                if (_storedVectors[castID].IsExpired(currentTime))
                {
                    expired.Add(castID);
                }
            }
            foreach(short castID in expired)
            {
                _storedVectors.Remove(castID);
            }
        }
        
    }

    public static void AddAvatar(Avatar avatar)
    {
        _matchPlayers.Add(avatar.PlayerID, avatar);
    }
    public static Avatar RemoveAvatar(byte ID)
    {
        if (_matchPlayers.ContainsKey(ID))
        {
            Avatar toRemove = _matchPlayers[ID];
            string message = Language.BuildString(299, toRemove.Name);
            MessageData md = new MessageData(message, "Server");
            _matchPlayers.Remove(ID);
            return toRemove;
        }
        return null;
    }
    public static void AddDeadAvatar(Avatar deadPlayer)
    {
        if (!_deadAvatarsOnPCTeam.ContainsKey(deadPlayer.PlayerID))
        {
            _deadAvatarsOnPCTeam.Add(deadPlayer.PlayerID, deadPlayer);
        }
    }
    public static void RemoveDeadAvatar(byte id)
    {
        if (_deadAvatarsOnPCTeam.ContainsKey(id))
        {
            _deadAvatarsOnPCTeam.Remove(id);
        }
    }
    public static List<Avatar> DeadAvatars
    {
        get
        {
            return _deadAvatarsOnPCTeam.Values.ToList();
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
        if (!toReturn)
        {
            Game.SendInGameBytes(InGame_Packets.FetchPlayerPacket(id));
        }
        return toReturn;
    }
    public static void LeaveMatch()
    {
        Game.UDP.StopListening();
        MatchParams.ReturningFromMatch = true;
        SceneManager.LoadScene("Pregame");
    }
    public static void AddSigil(short castID, Sigil sigil)
    {
        _sigils.Add(castID, sigil);
    }
    public static void AddWall(short castID, Wall wall)
    {
        _walls.Add(castID, wall);
    }
    public static void RemoveSigil(short castID)
    {
        if (_sigils.ContainsKey(castID))
        {
            Sigil toRemove = _sigils[castID];
            _sigils.Remove(castID);
            toRemove.DestroySigil();
        }
    }
    public static void RemoveWall(short castID)
    {
        if (_walls.ContainsKey(castID))
        {
            Wall toRemove = _walls[castID];
            _walls.Remove(castID);
            toRemove.DestroyWall();
        }
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
    public static Avatar CreateAvatar(byte[] decrypted, ref int index)
    {
        byte playerID = decrypted[index];
        index++;
        byte teamID = decrypted[index];
        index++;
        byte[] appearance = new byte[5];
        System.Array.Copy(decrypted, index, appearance, 0, appearance.Length);
        index += appearance.Length;
        byte level = decrypted[index];
        index++;
        byte characterClass = decrypted[index];
        index++;
        byte[] nameBytes = new byte[decrypted[index]];
        index++;
        System.Array.Copy(decrypted, index, nameBytes, 0, nameBytes.Length);
        index += nameBytes.Length;
        string name = ByteUtils.BytesToUTF8(nameBytes, 0, nameBytes.Length);
        byte[] positionBytes = new byte[12];
        byte[] directionBytes = new byte[4];
        Array.Copy(decrypted, index, positionBytes, 0, 12);
        index += 12;
        Array.Copy(decrypted, index, directionBytes, 0, 4);
        index += 4;
        byte alive = decrypted[index];
        Debug.Log("ALIVE BYTE VALUE: " + alive);
        index++;
        Avatar added = ComponentRegister.Spawner.SpawnAvatar();
        added.SetAttributes(playerID, name, level, characterClass, (Team)teamID, appearance, alive == 1);
        if(playerID != MatchParams.IDinMatch)
        {
            added.UpdatePosition(positionBytes, 0, true);
            added.UpdateDirection(directionBytes, 0, true);
        }
        return added;
    }
    public static void ProcessPlayerJoinedPacket(byte[] decrypted)
    {
        int index = 1;
        Avatar added = CreateAvatar(decrypted, ref index);
        if (added != null)
        {
            byte newToMatch = decrypted[index];
            MessageData md = new MessageData(added.Name + " has joined the match.", "Server");
            AddAvatar(added);
            if (added.PlayerID == MatchParams.IDinMatch)
            {
                if (added.IsAlive)
                {
                    ComponentRegister.PC.RestoreHPandMana();
                    if (newToMatch == 0)
                    {
                        ComponentRegister.Valhalla.EnterValhalla();
                    }
                }
                if (!ComponentRegister.PC.InValhalla)
                {
                    ComponentRegister.Scene.AssignEntryPoint(ComponentRegister.PC);
                }
                MatchParams.JoinedMatch = true;
                Debug.Log("MaxHP: " + MatchParams.MaxHP);
                Debug.Log("MaxMana: " + MatchParams.MaxMana);
                ComponentRegister.PlayerStatusPanel.SetExperience(PlayerAccount.SelectedCharacter.GetExperience());
                Game.SendInGameBytes(InGame_Packets.AllPlayerData());
                Game.SendInGameBytes(InGame_Packets.WallAndSigilRequest());
            }
            else
            {
                Debug.Log("Non PC entry: " + added.PlayerID);
            }
        }
        else
        {
            Debug.Log("Added is null!");
        }
    }
    public static void UpdatePlayerLocation(byte[] decrypted)
    {
        byte playerID = decrypted[1];
        if (_matchPlayers.ContainsKey(playerID))
        {
            byte pmd = decrypted[6];
            Avatar toUpdate = _matchPlayers[playerID];
            if(toUpdate.PMD.ToByte() != pmd)
            {
                toUpdate.PMD.SetPMD(pmd);
                toUpdate.UpdateModelRotation();
            }
            if(playerID != MatchParams.IDinMatch)
            {
                int packetID = BitConverter.ToInt32(decrypted, 2);
                if (packetID > toUpdate.LastPRPacketID)
                {
                    byte controlCode = decrypted[7];
                    toUpdate.LastPRPacketID = packetID;
                    switch (controlCode)
                    {
                        case 0: // position only
                            toUpdate.UpdatePosition(decrypted, 8, false);
                            break;
                        case 1: // direction only
                            toUpdate.UpdateDirection(decrypted, 8, false);
                            break;
                        case 2: // position and direction
                            toUpdate.UpdatePosition(decrypted, 8, false);
                            toUpdate.UpdateDirection(decrypted, 20, false);
                            break;
                    }
                }
            }
        }
        else
        {
            Game.SendInGameBytes(InGame_Packets.FetchPlayerPacket(playerID));
        }
    }
    public static void ChangeObjectState(byte key, byte state, bool force)
    {
        if (_objects.ContainsKey(key))
        {
            _objects[key].StatusChanged(state, force);
            //Debug.Log("Object state change: " + key + ", " + state);
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
