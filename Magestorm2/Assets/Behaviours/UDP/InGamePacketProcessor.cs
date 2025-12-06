using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGamePacketProcessor : UDPProcessor
{
    private void Awake()
    {
        ComponentRegister.InGamePacketProcessor = this;
        Init(MatchParams.ListeningPort);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponentRegister.UIPrefabManager.ClearStack();
    }

    // Update is called once per frame
    void Update()
    {
        if (_listeningPort > 0)
        {
            if (_udp.HasPacketsPending)
            {
                Debug.Log("IGPP received, opcode " + _opCode);
                List<byte[]> toProcess = _udp.PacketsReceived();
                foreach (byte[] decryptedPayload in toProcess)
                {
                    PreProcess(decryptedPayload);
                    switch (_opCode)
                    {
                        case InGame_Receive.ObjectData:
                            Match.ProcessObjectStates(_decrypted);
                            break;
                        case InGame_Receive.ObjectStateChange:
                            ProcessObjectChangePacket();
                            break;
                            /*
                        case InGame_Receive.ShrineHealth:
                            ProcessShrineHealthPacket();
                            break;
                        case InGame_Receive.AllShrineHealth:
                            ProcessAllShrineHealthPacket();
                            break;
                            */
                        case InGame_Receive.BroadcastMessage:
                            ProcessBroadcastMessagePacket();
                            break;
                        case InGame_Receive.MatchEnded:
                            Match.LeaveMatch();
                            break;
                        case InGame_Receive.PlayerLeftMatch:
                            ProcessPlayerLeftMatchPacket();
                            break;
                        case InGame_Receive.RemovedFromMatch:
                            Match.LeaveMatch();
                            break;
                        case InGame_Receive.PlayerJoinedMatch:
                            Match.ProcessPlayerJoinedPacket(_decrypted);
                            break;
                        case InGame_Receive.InactivityWarning:
                            ProcessInactivityWarning();
                            break;
                        case InGame_Receive.PoolBiased:
                            ProcessPoolBias();                    
                            break;
                        case InGame_Receive.PoolBiasFailure:
                            ProcessPoolBiasFailure();
                            break;
                        case InGame_Receive.ShrineAdjusted:
                            ShrineManager.ProcessShrineAdjustment(_decrypted[1], _decrypted[2], _decrypted[3]);
                            break;
                        case InGame_Receive.ShrineFailure:
                            ProcessShrineFailure();
                            break;
                        case InGame_Receive.PlayerKilled:
                            ProcessKilledPlayer();
                            break;
                        case InGame_Receive.FlagReturned:
                            HandleFlagReturn();
                            break;
                        case InGame_Receive.FlagCaptured:
                            HandleFlagCapture();
                            break;
                        case InGame_Receive.FlagDropped:
                            HandleFlagDrop();
                            break;
                        case InGame_Receive.FlagTaken:
                            HandleFlagTaken();
                            break;
                        case InGame_Receive.PlayerMoved:
                            Match.UpdatePlayerLocation(_decrypted);
                            break;
                        case InGame_Receive.PlayerData:
                            Match.ProcessPlayerJoinedPacket(_decrypted);
                            break;
                        case InGame_Receive.HPandManaUpdate:
                            ComponentRegister.PC.HPandManaUpdate(_decrypted);
                            break;
                        case InGame_Receive.HPUpdate:
                        case InGame_Receive.ManaUpdate:
                        case InGame_Receive.LeyUpdate:
                            ComponentRegister.PC.HPorManaorLeyUpdate(_decrypted);
                            break;
                        case InGame_Receive.TeamMessage:
                            HandleTeamMessage();
                            break;
                        case InGame_Receive.PlayerRevived:
                            HandleRevive();
                            break;
                        case InGame_Receive.PlayerTapped:
                            HandleTap();
                            break;
                        case InGame_Receive.PostureChange:
                            HandlePostureChange();
                            break;
                    }
                }
            }
        }
    }
    private void HandlePostureChange()
    {
        byte avatarID = _decrypted[1];
        Avatar avatar = null;
        if(Match.GetAvatar(avatarID, ref avatar))
        {
            avatar.Posture = _decrypted[2];
            Debug.Log("Posture Change, Avatar: " + avatarID + ", Posture: " + Game.PCAvatar.Posture);
        }
    }
    private void HandleTap()
    {
        byte tapperID = _decrypted[1];

        Avatar tapper = null;
        if (Match.GetAvatar(tapperID, ref tapper))
        {
            if(tapperID == MatchParams.IDinMatch)
            {
                ComponentRegister.Valhalla.EnterValhalla();
                ComponentRegister.PC.UpdateHP(MatchParams.MaxHP);
                new MessageData(Language.BuildString(213, Teams.GetTeamName((Team)MatchParams.MatchTeamID)), "Server");
            }
            else
            {
                new MessageData(Language.BuildString(214, tapper.Name, Teams.GetTeamName((Team)MatchParams.MatchTeamID)), "Server");
            }
            tapper.SetAlive(true);
        }
    }
    private void HandleRevive()
    {
        byte revivedID = _decrypted[1];
        byte reviverID = _decrypted[2];
        Avatar revived = null;
        if(Match.GetAvatar(revivedID, ref revived))
        {
            Avatar reviver = null;
            Match.GetAvatar(reviverID, ref reviver);
            if(revivedID == MatchParams.IDinMatch)
            {
                float hp = BitConverter.ToSingle(_decrypted, 3);
                ComponentRegister.PC.UpdateHP(hp);
                new MessageData(reviver == null ? Language.GetBaseString(210) : Language.BuildString(208, reviver.Name), "Server");
            }
            else
            {
                new MessageData(reviver == null ? Language.BuildString(211, revived.Name) : Language.BuildString(212, revived.Name, reviver.Name), "Server");
            }
            revived.SetAlive(true);
        }
    }
    private void HandleTeamMessage()
    {

        byte senderID = _decrypted[1];
        Team recipientTeam = (Team)_decrypted[2];
        int messageLength = BitConverter.ToInt32(_decrypted, 3);
        byte[] messageBytes = new byte[messageLength];
        Array.Copy(_decrypted, 7, messageBytes, 0, messageLength);
        string message = ByteUtils.BytesToUTF8(messageBytes, 0, messageLength);
        Avatar sender = null;
        string senderName = "Server";
        if(senderID == MatchParams.IDinMatch)
        {
            senderName = Language.GetBaseString(206) + " " + Teams.GetTeamName(recipientTeam);
        }
        else if(Match.GetAvatar(senderID, ref sender))
        {
            senderName = sender.Name;
        }
        MessageData md = new MessageData(message, senderName, Teams.GetTeamColor(recipientTeam));
    }
    private void HandleFlagTaken()
    {
        Team flagTaken = (Team)_decrypted[1];
        string teamName = Teams.GetTeamName(flagTaken);
        byte takerID = _decrypted[2];
        FlagManager.FlagTaken(flagTaken);
        if (takerID == MatchParams.IDinMatch)
        {
            new MessageData(Language.BuildString(193, teamName), "Server"); //
            FlagManager.FlagHeldByPlayer = flagTaken;
        }
        else
        {
            Avatar flagTaker = null;
            if (Match.GetAvatar(takerID, ref flagTaker))
            {
                new MessageData(Language.BuildString(192, teamName, flagTaker.Name), "Server"); //
            }
            else
            {
                new MessageData(Language.BuildString(191, teamName), "Server"); //
            }
        }
    }
    private void HandleFlagReturn()
    {
        Team flagReturned = (Team)_decrypted[1];
        FlagManager.ReturnFlag(flagReturned);
        new MessageData(Language.BuildString(190, Teams.GetTeamName(flagReturned)), "Server"); //
    }
    private void HandleFlagCapture()
    {
        Team capturingTeam = (Team)_decrypted[1];
        Team flagCaptured = (Team)_decrypted[2];
        byte capturedBy = _decrypted[3];
        byte scoreCapturer = _decrypted[4];
        byte scoreCaptured = _decrypted[5];

        FlagManager.SetScore(capturingTeam, scoreCapturer);
        FlagManager.SetScore(flagCaptured, scoreCaptured);
        ComponentRegister.CTFScorePanel.RefreshScores();
        FlagManager.ReturnFlag(flagCaptured);
        new MessageData(Language.BuildString(189, Teams.GetTeamName(flagCaptured), Teams.GetTeamName(capturingTeam)), "Server"); //
    }
    private void HandleFlagDrop()
    {
        byte killedPlayerID = _decrypted[1];
        byte killerID = _decrypted[2];
        if (killedPlayerID == MatchParams.IDinMatch)
        {
            FlagManager.FlagHeldByPlayer = Team.Neutral;
            if(killedPlayerID == killerID)
            {
                // voluntary drop
                FlagManager.FlagJustDropped = true;
            }
        }
        if (killerID > 0 && killedPlayerID != killerID)
        {
            ProcessKilledPlayer();
        }
        
        Team flagTeam = (Team)_decrypted[3];
        Vector3 position = ByteUtils.BytesToVector3(_decrypted, 5);
        FlagManager.RepositionFlag(flagTeam, position);
        new MessageData(Language.BuildString(188, Teams.GetTeamName(flagTeam)), "Server"); //
    }
    private void ProcessKilledPlayer()
    {
        byte killedPlayerID = _decrypted[1];
        byte killerID = _decrypted[2];
        Avatar killedPlayer = null;
        if(Match.GetAvatar(killedPlayerID, ref killedPlayer))
        {
            if(killedPlayerID == MatchParams.IDinMatch)
            {
                ComponentRegister.PlayerMovement.DeathResetCameraAndController();
                Avatar playerKiller = null;
                ComponentRegister.PC.UpdateHP(0.0f);
                if (Match.GetAvatar(killerID, ref playerKiller))
                {
                    new MessageData(Language.BuildString(186, playerKiller.Name), "Server"); //
                }
            }
            else
            {
                if (killerID == MatchParams.IDinMatch) // player killed someone
                {
                    new MessageData(Language.BuildString(185, killedPlayer.Name), "Server"); //
                }
                else // someone else killed someone
                {
                    Avatar killer = null;
                    if (Match.GetAvatar(killerID, ref killer))
                    {
                        new MessageData(Language.BuildString(187, killedPlayer.Name), "Server"); //
                    }
                }
            }
            killedPlayer.SetAlive(false);
        }
    }
    private void ProcessShrineFailure()
    {
        byte shrineID = _decrypted[1];
        string notificationText = "";
        if(shrineID == MatchParams.MatchTeamID)
        {
            notificationText = Language.BuildString(182, Language.GetBaseString(183), Teams.GetTeamName((Team)shrineID)); //
        }
        else
        {
            notificationText = Language.BuildString(182, Language.GetBaseString(184), Teams.GetTeamName((Team)shrineID)); //
        }
        ComponentRegister.Notifier.DisplayNotification(notificationText);
    }
    private void ProcessPoolBiasFailure()
    {
        ComponentRegister.Notifier.DisplayNotification(Language.GetBaseString(170)); //
    }
    private void ProcessPoolBias()
    {
        byte poolID = _decrypted[1];
        byte biasAmount = _decrypted[2];
        byte teamID = _decrypted[3];
        byte biaserID = _decrypted[4];
        PoolManager.PoolBiased(biaserID, poolID, teamID, biasAmount);
    }
    private void ProcessInactivityWarning()
    {
        Game.SendInGameBytes(InGame_Packets.InactivityResponsePacket());
    }

    private void ProcessPlayerLeftMatchPacket()
    {
        byte numDeparted = _decrypted[1];
        int index = 2;
        for (int i = 0; i < numDeparted; i++)
        {
            byte playerID = _decrypted[index];
            Match.RemoveAvatar(playerID);
            index++;
        }
    }
    private void ProcessBroadcastMessagePacket()
    {
        int messageLength = BitConverter.ToInt32(_decrypted, 2);
        Avatar sender = null;
        byte playerID = _decrypted[1];
        byte[] messageBytes = new byte[messageLength];
        Array.Copy(_decrypted, 6, messageBytes, 0, messageLength);
        string message = Encoding.UTF8.GetString(messageBytes);
        string name;
        if(playerID == MatchParams.IDinMatch)
        {
            name = "You";
        }
        else
        {
            if (Match.GetAvatar(playerID, ref sender))
            {
                name = sender.Name;
            }
            else
            {
                name = "Player " + playerID;
            }
        }
        MessageData md = new MessageData(message, name);
        
    }
    /*
    private void ProcessAllShrineHealthPacket()
    {
        Match.ChangeShrineHealth((byte)Team.Chaos, _decrypted[1]);
        Match.ChangeShrineHealth((byte)Team.Balance, _decrypted[2]);
        Match.ChangeShrineHealth((byte)Team.Order, _decrypted[3]);
    }
    private void ProcessShrineHealthPacket()
    {
        Match.ChangeShrineHealth(_decrypted[1], _decrypted[2]);
    }
    */
    private void ProcessObjectChangePacket()
    {
        Match.ChangeObjectState(_decrypted[1], _decrypted[2], false);
    }
}

