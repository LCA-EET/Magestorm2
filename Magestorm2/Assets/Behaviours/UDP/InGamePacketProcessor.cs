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
                            ProcessPlayerJoinedMatchPacket();
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
                            Match.ProcessShrineAdjustment(_decrypted[1], _decrypted[2], _decrypted[3]);
                            break;
                        case InGame_Receive.ShrineFailure:
                            ProcessShrineFailure();
                            break;
                        case InGame_Receive.HMLUpdate:
                            ProcessHMLUpdate();
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
                    }
                }
            }
        }
    }
    private void HandleFlagTaken()
    {
        Team flagTaken = (Team)_decrypted[1];
        string teamName = Teams.GetTeamName(flagTaken);
        byte takerID = _decrypted[2];
        if(takerID == MatchParams.IDinMatch)
        {

            ComponentRegister.MessageRecorder.MessageReceived(new MessageData(Language.BuildString(193, teamName), "Server")); //
        }
        else
        {
            Avatar flagTaker = null;
            if (Match.GetAvatar(takerID, ref flagTaker))
            {
                ComponentRegister.MessageRecorder.MessageReceived(new MessageData(Language.BuildString(192, teamName, flagTaker.Name), "Server")); //
            }
            else
            {
                ComponentRegister.MessageRecorder.MessageReceived(new MessageData(Language.BuildString(191, teamName), "Server")); //
            }
        }
    }
    private void HandleFlagReturn()
    {
        Team flagReturned = (Team)_decrypted[1];
        FlagManager.ReturnFlag(flagReturned);
        MessageData data = new MessageData(Language.BuildString(190, Teams.GetTeamName(flagReturned)), "Server"); //
        ComponentRegister.MessageRecorder.MessageReceived(data);
    }
    private void HandleFlagCapture()
    {
        Team capturingTeam = (Team)_decrypted[1];
        Team flagCaptured = (Team)_decrypted[2];
        byte capturedBy = _decrypted[3];
        byte scoreCapturer = _decrypted[4];
        byte scoreCaptured = _decrypted[5];

        ComponentRegister.CTFScorePanel.ChangeScore(capturingTeam, scoreCapturer);
        ComponentRegister.CTFScorePanel.ChangeScore(flagCaptured, scoreCaptured);
        FlagManager.ReturnFlag(flagCaptured);
        MessageData data = new MessageData(Language.BuildString(189, Teams.GetTeamName(flagCaptured), Teams.GetTeamName(capturingTeam)), "Server"); //
        ComponentRegister.MessageRecorder.MessageReceived(data);
    }
    private void HandleFlagDrop()
    {
        byte killedPlayerID = _decrypted[1];
        byte killerID = _decrypted[2];
        if(killerID > 0)
        {
            ProcessKilledPlayer();
        }
        Team flagTeam = (Team)_decrypted[3];
        Vector3 position = ByteUtils.BytesToVector3(_decrypted, 5);
        FlagManager.RepositionFlag(flagTeam, position);
        MessageData data = new MessageData(Language.BuildString(186, Teams.GetTeamName(flagTeam)), "Server"); //
        ComponentRegister.MessageRecorder.MessageReceived(data);
    }
    private void ProcessKilledPlayer()
    {
        byte killedPlayerID = _decrypted[1];
        byte killerID = _decrypted[2];
        MessageData data = null;
        if (killedPlayerID == MatchParams.IDinMatch) // player was killed
        {
            ComponentRegister.PC.HMLUpdate(0, 0);
            Avatar playerKiller = null;
            if(Match.GetAvatar(killerID, ref playerKiller))
            {
                data = new MessageData(Language.BuildString(186, playerKiller.Name), "Server"); //
            }
        }
        else
        {
            Avatar killedPlayer = null;
            if(Match.GetAvatar(killedPlayerID, ref killedPlayer))
            {
                if (killerID == MatchParams.IDinMatch) // player killed someone
                {
                    data = new MessageData(Language.BuildString(185, killedPlayer.Name), "Server"); //
                }
                else // someone else killed someone
                {
                    Avatar killer = null;
                    if(Match.GetAvatar(killerID, ref killer))
                    {
                        data = new MessageData(Language.BuildString(187, killedPlayer.Name), "Server"); //
                    }
                }
            }
            
        }
        ComponentRegister.MessageRecorder.MessageReceived(data);
    }
    private void ProcessHMLUpdate()
    {
        ComponentRegister.PC.HMLUpdate(_decrypted);
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
        Match.PoolBiased(biaserID, poolID, teamID, biasAmount);
    }
    private void ProcessInactivityWarning()
    {
        Game.SendInGameBytes(InGame_Packets.InactivityResponsePacket());
    }

    private void ProcessPlayerJoinedMatchPacket()
    {
        byte idInMatch = _decrypted[1];
        if(idInMatch != MatchParams.IDinMatch)
        {
            byte teamID = _decrypted[2];
            byte[] appearance = new byte[5];
            int index = 3;
            Array.Copy(_decrypted, index, appearance, 0, appearance.Length);
            index += 5;
            byte level = _decrypted[index];
            index++;
            byte characterClass = _decrypted[index];
            index++;
            byte nameLength = _decrypted[index];
            index++;
            string name = ByteUtils.BytesToUTF8(_decrypted, index, nameLength);
            MessageData md = new MessageData(name + " has joined the match.", "Server");
        }
        
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
        Match.ChangeObjectState(_decrypted[1], _decrypted[2]);
    }
}

