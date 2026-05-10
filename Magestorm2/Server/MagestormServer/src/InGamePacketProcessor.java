import javax.sound.sampled.Control;
import java.net.DatagramPacket;
import java.util.Arrays;

public class InGamePacketProcessor extends UDPProcessor{
    private final Match _owningMatch;

    public InGamePacketProcessor(int port, Match owningMatch){
        super(port);
        _owningMatch = owningMatch;
    }
    @Override
    protected boolean ProcessPacket(DatagramPacket received){
        PreProcess(received);
        if(IsVerified()){
            _remote = _owningMatch.GetVerifiedClient(_decrypted[1]);
            switch(_opCode){
                case InGame_Receive.ChangedObjectState:
                    HandleObjectStateChange();
                    return true;
                case InGame_Receive.DirectMessage:
                    HandleDirectMessage();
                    return true;
                case InGame_Receive.BroadcastMessage:
                    HandleBroadcastMessage();
                    return true;
                case InGame_Receive.LeaveMatch:
                    HandleLeaveMatch();
                    return true;
                case InGame_Receive.InactivityCheckResponse:
                    InactivityCheckResponse();
                    return true;
                case InGame_Receive.QuitGame:
                    HandleQuitGame();
                    return true;
                case InGame_Receive.ObjectStatus:
                    HandleObjectStatusRequest();
                    return true;
                case InGame_Receive.PlayerMoved:
                    HandlePlayerMoved();
                    return true;
                case InGame_Receive.FetchPlayer:
                    HandleFetchPlayer();
                    return true;
                case InGame_Receive.LeyUpdate:
                    HandleLeyUpdate();
                    return true;
                case InGame_Receive.Tap:
                    HandleTap();
                    return true;
                case InGame_Receive.PostureChange:
                    HandlePostureChange();
                    return true;
                case InGame_Receive.Cast:
                    HandleCast();
                    return true;
                case InGame_Receive.ReportHit:
                    HandleReportHitPacket();
                    return true;
                case InGame_Receive.ReportResistableHit:
                    HandleResistableHitPacket();
                    return true;
                case InGame_Receive.Devour:
                    _owningMatch.HandleBanish(_decrypted);
                    return true;
                case InGame_Receive.ReportSplash:
                    HandleSplashHit();
                    return true;
                case InGame_Receive.WallHit:
                    HandleWallHit();
                    return true;
                case InGame_Receive.RequestWallData:
                    HandleWallRequestPacket();
                    return true;
                case InGame_Receive.LeaderboardRequest:
                    HandleLeaderboardRequest();
                    return true;
                case InGame_Receive.UpdateSlotting:
                    HandleUpdateSlotting();
                    return true;
            }
        }
        else if(_opCode == InGame_Receive.JoinedMatch){
            _remote = new RemoteClient(received);
            return HandleJoinMatchPacket(_remote);
        }
        return false;
    }
    private void HandleResistableHitPacket(){
        MatchCharacter mc = _owningMatch.GetMatchCharacter(_decrypted[1]);
        if(mc != null){
            short castID = ByteUtils.ExtractShort(_decrypted, 2);

        }
    }
    private void HandleUpdateSlotting(){
        MatchCharacter mc = _owningMatch.GetMatchCharacter(_decrypted[1]);
        if(mc != null){
            Main.AsyncDBUpdater.AddToQueue(new AsyncDBUpdate(ControlCodes.AsyncDBUpdate_Slotting, mc, _decrypted));
        }
    }
    private void HandleLeaderboardRequest(){
        byte[] toEncrypt = MatchManager.GetScoreBytes(_owningMatch._matchID);
        _owningMatch.SendToPlayer(Packets.MatchScoresPacket(InGame_Send.MatchScores, toEncrypt),
                _owningMatch.GetMatchCharacter(_decrypted[1]));
    }
    private void HandleWallRequestPacket(){
        _owningMatch.RequestWallData(_owningMatch.GetMatchCharacter(_decrypted[1]));
    }
    private void HandleWallHit(){
        MatchCharacter shooter = _owningMatch.GetMatchCharacter(_decrypted[1]);
        if(shooter!= null){
            if(shooter.IsAlive()){
                short castID = ByteUtils.ExtractShort(_decrypted, 2);
                DamagingSpell projectile = (DamagingSpell)_owningMatch.GetCastSpell(castID);
                if(projectile != null){
                    short wallID = ByteUtils.ExtractShort(_decrypted, 4);
                    Wall wall = _owningMatch.GetWall(wallID);
                    if(wall != null){
                        wall.TakeDamage(projectile);
                    }
                }
            }
        }

    }
    private void HandleSplashHit(){
        MatchCharacter hit = _owningMatch.GetMatchCharacter(_decrypted[1]);
        if(hit != null){
            short castID = ByteUtils.ExtractShort(_decrypted, 2);
            hit.RegisterSplashHit(castID);
            _owningMatch.PlayerHit(hit, castID);
            hit.DeregisterSplashHit(castID);
        }
    }
    private void HandleCast(){
        byte casterID = _decrypted[1];
        byte spellID = _decrypted[3];
        if(_owningMatch.IsCharacterAlive(casterID) ){
            if(SpellManager.ContainsSpell(spellID)){
                Spell spellReference = SpellManager.GetSpell(spellID);
                MatchCharacter casterReference = _owningMatch.GetMatchCharacter(_decrypted[1]);
                short castID = casterReference.CastSpell(spellReference, _decrypted); // instantiation downstream
                if(castID != -1){
                    byte[] toSend = Packets.CastPacket(_decrypted, castID);
                    switch(spellReference.GetNotificationCode()){
                        case ControlCodes.SpellNotification_All:
                            _owningMatch.SendToAll(toSend);
                            break;
                        case ControlCodes.SpellNotification_TeamOnly:
                            MatchTeam recipientTeam = _owningMatch.GetMatchTeam(casterReference.GetTeamID());
                            _owningMatch.SendToCollection(toSend, recipientTeam.GetRemoteClients());
                            break;
                        case ControlCodes.SpellNotification_CasterOnly:
                            _owningMatch.SendToPlayer(toSend, casterReference);
                            break;
                        case ControlCodes.SpellNotification_Payload: // sends to the player(s) identified in the payload
                            _owningMatch.SendToPlayer(toSend, casterReference);
                            _owningMatch.SendToPlayer(toSend, _decrypted[ControlCodes.CastPayloadStartIndex]);
                            break;
                    }
                }
            }
            else{
                Main.LogError("IGPP.HandleCast: Invalid spell key. SpellID: " + spellID);
            }
        }
        else{
            Main.LogError("IGPP.HandleCast: Invalid caster. CasterID: " + casterID);
        }
    }
    private void HandleReportHitPacket(){
        _owningMatch.PlayerHit(_decrypted[1], ByteUtils.ExtractShort(_decrypted, 2));
    }

    private void HandlePostureChange(){
        _owningMatch.SendToAll(Packets.PostureChangePacket(_decrypted));
    }
    private void HandleTap(){
        MatchCharacter tapped = _owningMatch.GetMatchCharacter(_decrypted[1]);
        tapped.MultiplyExperience(0.95f);
        _owningMatch.PlayerTapped(tapped.GetIDinMatch());

    }
    private void HandleLeyUpdate(){
        _owningMatch.UpdatePlayerLey(_decrypted);
    }
    private void HandleFetchPlayer(){
        _owningMatch.SendPlayerData( _decrypted[1], _decrypted[2]);
    }
    private void HandlePlayerMoved(){
        _owningMatch.UpdatePlayerLocation(_decrypted);
    }
    private void HandleObjectStatusRequest(){
        _owningMatch.ProcessObjectStatusPacket(_decrypted[1]);
    }

    private void InactivityCheckResponse(){
        _owningMatch.GetMatchCharacter(_decrypted[1]).MarkPacketReceived();
    }
    private void HandleQuitGame(){
        _owningMatch.LeaveMatch(_decrypted[1], true, true);
        int accountID = ByteUtils.ExtractInt(_decrypted, 3);
        GameServer.ClientLoggedOut(accountID);
    }
    private void HandleLeaveMatch(){
        _owningMatch.LeaveMatch(_decrypted[1], true, false);
    }

    private void HandleBroadcastMessage(){
        int messageLength = ByteUtils.ExtractInt(_decrypted,2);
        String messageString = ByteUtils.BytesToUTF8(_decrypted, 6, messageLength);
        if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
            EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                    _remote);
        }
        else{
            Main.LogMessage("MessageString: " + messageString);
            if(messageString.startsWith("/")){
                String[] split = messageString.split(" ");
                _owningMatch.ParseCommand(split[0].toLowerCase().substring(1), split, _decrypted[1]);
            }
            else{
                EnqueueForSend(Packets.MessagePacket(_decrypted, 6 + messageLength),
                        _owningMatch.GetVerifiedClients());
            }
        }
    }

    private void HandleDirectMessage(){
        byte recipientID = _decrypted[2];
        if(IsVerified(recipientID)){
            int messageLength = ByteUtils.ExtractInt(_decrypted, 3);
            byte[] messageBytes = new byte[messageLength];
            System.arraycopy(_decrypted, 7, messageBytes, 0, messageLength);
            String messageString = ByteUtils.BytesToUTF8(messageBytes);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage), _remote);
            }
            else{
                RemoteClient messageRecipient = _owningMatch.GetMatchCharacter(recipientID).GetRemoteClient();
                Iterable<RemoteClient> recipients = Arrays.asList(_remote, messageRecipient);
                EnqueueForSend(Packets.MessagePacket(_decrypted, messageLength + 7), recipients);
            }
        }
    }
    private void HandleObjectStateChange(){
        byte objectID = _decrypted[2];
        byte state = _decrypted[3];
        byte selfReset = _decrypted[4];
        _owningMatch.ChangeObjectState(objectID, state, _decrypted[1], selfReset);
    }
    protected boolean HandleJoinMatchPacket(RemoteClient remote){
        int accountID = CheckAccountAndCharacter();
        if(accountID >= 0){
            byte idInMatch = _decrypted[9];
            byte teamID = _decrypted[10];
            Main.LogMessage("Verifying player " + idInMatch + " for match " + _owningMatch.MatchID() + ", team " + teamID);
            if(_owningMatch.IsAwaitingVerification(accountID)){
                GameServer.GetClient(accountID).MarkInGame();
                _owningMatch.MarkPlayerVerified(idInMatch, teamID, accountID, remote);
                SendPlayerDataForJoinee(_owningMatch.GetMatchCharacter(idInMatch));
                _owningMatch.ProcessObjectStatusPacket(_decrypted[9]);
                return true;
            }
            else if (_owningMatch.IsPlayerVerified(idInMatch)){
                SendPlayerDataForJoinee(_owningMatch.GetMatchCharacter(idInMatch));
                return true;
            }
            else{
                Main.LogMessage("Player " + idInMatch + " NOT verified for match " + _owningMatch.MatchID() + ", team " + teamID);
            }
        }
        return false;
    }
    private void SendPlayerDataForJoinee(MatchCharacter joinee){
        byte alive = joinee.IsAlive()?(byte)1:(byte)0;
        _owningMatch.SendToAll(Packets.PlayerDataPacket(joinee.GetPlayerData(), alive, joinee.IsNewToMatch()));
    }
    private int CheckAccountAndCharacter(){
        RemoteClient remote = LoggedInClient();
        if(remote != null){
            int accountID = remote.AccountID();
            if(ByteUtils.ExtractInt(_decrypted, 5) == GameServer.GetActiveCharacter(accountID).GetCharacterID()){
                Main.LogMessage("Account check passed: " + accountID + ", match " + _owningMatch.MatchID());
                return accountID;
            }
        }
        return -1;
    }
    protected boolean IsVerified(){
        return _owningMatch.IsPlayerVerified(_decrypted[1]);
    }
    private boolean IsVerified(byte playerID){
        return _owningMatch.IsPlayerVerified(playerID);
    }
}
