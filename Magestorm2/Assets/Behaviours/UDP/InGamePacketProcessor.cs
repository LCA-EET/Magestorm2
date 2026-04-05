using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InGamePacketProcessor : UDPProcessor
{
    private void Awake()
    {
        ComponentRegister.InGamePacketProcessor = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(MatchParams.RemotePort);
        ComponentRegister.UIPrefabManager.ClearStack();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.UDP.HasPacketsPending)
        {
            //Debug.Log("IGPP received, opcode " + _opCode);
            List<byte[]> toProcess = Game.UDP.PacketsReceived();
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
                    case InGame_Receive.ApplyEffect:
                        HandleEffect();
                        break;
                    case InGame_Receive.Cast:
                        HandleCast();
                        break;
                    case InGame_Receive.InactivityDisconnect:
                        HandleInactivityDisconnect();
                        break;
                    case InGame_Receive.HitNotification:
                        HandleHitNotification();
                        break;
                    case InGame_Receive.SendToValhalla:
                        new MessageData(Language.GetBaseString(312), Language.GetBaseString(209));
                        ComponentRegister.Valhalla.EnterValhalla();
                        break;
                }
            }
        }
    }
    private void HandleCast()
    {
        byte casterID = _decrypted[1];
        byte spellType = _decrypted[2];
        byte spellID = _decrypted[3];
        byte payloadLength = _decrypted[4];
        Debug.Log("HandleCast()");
        Avatar caster = null;
        if(Match.GetAvatar(casterID, ref caster))
        {
            SpellSpawner spawner = null;
            if (ComponentRegister.Spawner.SpawnSpellPrefab(spellID, ref spawner))
            {
                byte[] payload = new byte[payloadLength];
                Array.Copy(_decrypted, ControlCodes.CastPayloadStartIndex, payload, 0, payloadLength);
                short castID = BitConverter.ToInt16(_decrypted, ControlCodes.CastPayloadStartIndex + payloadLength);
                Debug.Log("Payload length: " + payload.Length);
                spawner.Initialize(caster, spellType, castID, payload);
            }
        }
    }

    private void HandleHitNotification()
    {
        byte hitByID = _decrypted[2];
        Avatar hitBy = null;
        if(Match.GetAvatar(hitByID, ref hitBy))
        {
            float angle = SharedFunctions.AngleBetween(Camera.main.transform, hitBy.transform);
            ComponentRegister.DDIPanel.InstantiateDDI(angle);
        }
    }
    private void HandleInactivityDisconnect()
    {
        Game.Disconnected = true;
        ComponentRegister.UIPrefabManager.InstantiateMessageBox_Function(Language.GetBaseString(300), Game.Quit);
    }

    private void HandleEffect()
    {
        byte appliedToID = _decrypted[1];
        Avatar appliedTo = null;
        if(Match.GetAvatar(appliedToID, ref appliedTo))
        {
            byte applierID = _decrypted[2];
            byte effectCode = _decrypted[3];
            byte duration = _decrypted[4];
            byte degree = _decrypted[5];
            AppliedEffect ae = null;
            if (applierID != appliedToID)
            {
                Avatar applier = null;
                if(Match.GetAvatar(applierID, ref applier)){
                    if(ComponentRegister.Spawner.SpawnAppliedEffect(effectCode, ref ae))
                    {
                        ae.Initialize(applier, duration, degree);
                        appliedTo.AddEffect(ae);
                    }
                }
            }
            else
            {
                if(ComponentRegister.Spawner.SpawnAppliedEffect(effectCode, ref ae))
                {
                    ae.Initialize(appliedTo, duration, degree);
                    appliedTo.AddEffect(ae);
                }
            }
        }
    }
    private void HandlePostureChange()
    {
        byte avatarID = _decrypted[1];
        if(avatarID != MatchParams.IDinMatch)
        {
            Avatar avatar = null;
            if (Match.GetAvatar(avatarID, ref avatar))
            {
                avatar.PMD.SetPMD(_decrypted[2]);
            }
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
                Game.PlayerPMDByte.SetLocalPosture(Postures.Standing);
                ComponentRegister.Valhalla.EnterValhalla();
                ComponentRegister.PC.UpdateHP(MatchParams.MaxHP);
                new MessageData(Language.BuildString(213, Teams.GetTeamName((Team)MatchParams.MatchTeamID)), Language.GetBaseString(304));
            }
            else
            {
                new MessageData(Language.BuildString(214, tapper.Name, Teams.GetTeamName((Team)MatchParams.MatchTeamID)), Language.GetBaseString(304));
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
                new MessageData(reviver == null ? Language.GetBaseString(210) : Language.BuildString(208, reviver.Name), Language.GetBaseString(304));
            }
            else
            {
                new MessageData(reviver == null ? Language.BuildString(211, revived.Name) : Language.BuildString(212, revived.Name, reviver.Name), Language.GetBaseString(304));
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
        string senderName = Language.GetBaseString(304);
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
            new MessageData(Language.BuildString(193, teamName), Language.GetBaseString(304)); //
            FlagManager.FlagHeldByPlayer = flagTaken;
        }
        else
        {
            Avatar flagTaker = null;
            if (Match.GetAvatar(takerID, ref flagTaker))
            {
                new MessageData(Language.BuildString(192, teamName, flagTaker.Name), Language.GetBaseString(304)); //
            }
            else
            {
                new MessageData(Language.BuildString(191, teamName), Language.GetBaseString(304)); //
            }
        }
    }
    private void HandleFlagReturn()
    {
        Team flagReturned = (Team)_decrypted[1];
        FlagManager.ReturnFlag(flagReturned);
        new MessageData(Language.BuildString(190, Teams.GetTeamName(flagReturned)), Language.GetBaseString(304)); //
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
        new MessageData(Language.BuildString(189, Teams.GetTeamName(flagCaptured), Teams.GetTeamName(capturingTeam)), Language.GetBaseString(304)); //
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
        new MessageData(Language.BuildString(188, Teams.GetTeamName(flagTeam)), Language.GetBaseString(304)); //
    }
    private void ProcessKilledPlayer()
    {
        byte killedPlayerID = _decrypted[1];
        byte killerID = _decrypted[2];
        Avatar killedPlayer = null;
        
        if (Match.GetAvatar(killedPlayerID, ref killedPlayer))
        {
            if(killedPlayerID == MatchParams.IDinMatch)
            {
                ComponentRegister.PlayerMovement.DeathResetCameraAndController();
                Avatar playerKiller = null;
                ComponentRegister.PC.UpdateHP(0.0f);
                if (Match.GetAvatar(killerID, ref playerKiller))
                {
                    new MessageData(Language.BuildString(186, playerKiller.Name), Language.GetBaseString(304)); //
                }
            }
            else
            {
                if (killerID == MatchParams.IDinMatch) // player killed someone
                {
                    new MessageData(Language.BuildString(185, killedPlayer.Name), Language.GetBaseString(304)); //
                }
                else // someone else killed someone
                {
                    Avatar killer = null;
                    if (Match.GetAvatar(killerID, ref killer))
                    {

                        new MessageData(Language.BuildString(187, killedPlayer.Name, killer.Name), Language.GetBaseString(304)); //
                    }
                }
            }
            killedPlayer.CreateDeadBody();
            killedPlayer.SetAlive(false);
        }
    }
    private void ProcessShrineFailure()
    {
        byte shrineID = _decrypted[1];
        string notificationText = "";
        notificationText = Language.BuildString(182, Language.GetBaseString(shrineID == MatchParams.MatchTeamID ? 183 : 184), Teams.GetTeamName((Team)shrineID));
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
        new MessageData(Language.GetBaseString(301), Language.GetBaseString(304));
    }

    private void ProcessPlayerLeftMatchPacket()
    {
        byte numDeparted = _decrypted[1];
        int index = 2;
        for (int i = 0; i < numDeparted; i++)
        {
            byte playerID = _decrypted[index];
            Avatar toDestroy = Match.RemoveAvatar(playerID);
            if (toDestroy != null)
            {
                Destroy(toDestroy.gameObject);
            }
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
            name = Language.GetBaseString(302); // You
        }
        else
        {
            if (Match.GetAvatar(playerID, ref sender))
            {
                name = sender.Name;
            }
            else
            {
                name = Language.GetBaseString(303) + playerID;
            }
        }
        MessageData md = new MessageData(message, name);
        
    }
    private void ProcessObjectChangePacket()
    {
        Match.ChangeObjectState(_decrypted[1], _decrypted[2], false);
    }
}

