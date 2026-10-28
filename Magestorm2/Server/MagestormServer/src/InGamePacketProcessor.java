import java.net.DatagramPacket;

public class InGamePacketProcessor extends UDPProcessor{
    private final Match _owningMatch;
    protected MatchCharacter _verified;
    protected byte _senderID;
    protected RegisteredThread _asyncThread;
    protected AsyncIGPP _asyncProcessor;
    public InGamePacketProcessor(int port, Match owningMatch){
        super(port, ServerParams.Encryption);
        _asyncProcessor = new AsyncIGPP(owningMatch, this);
        _owningMatch = owningMatch;
        Main.LogDebug("Key for match " + owningMatch._objectIDAsByte + " (Hex): " +
                _cryptographer.GetCipher().GetSecretSymmetricKeyBI().toString(16));
        _asyncThread = new RegisteredThread(_asyncProcessor);
        _asyncThread.start();
    }
    @Override
    public void TerminateProcessor(){
        super.TerminateProcessor();
        _asyncThread.interrupt();
    }
    @Override protected boolean PreProcess(DatagramPacket received)
    {
        super.PreProcess(received);
        _senderID = _decrypted[1];
        _verified = _owningMatch.GetCharacterAndReset(_senderID);
        return _verified != null;
    }
    private void EnqueueForAsyncProcessing(){
        _asyncProcessor.EnqueueRequest(new AsyncIGPPRequest(_decrypted, _opCode, _verified, _remote));
    }
    @Override
    protected boolean ProcessPacket(DatagramPacket received){
        if(PreProcess(received)){
            _remote = _owningMatch.GetVerifiedClient(_senderID);
            switch(_opCode){
                case InGame_Receive.LeaderboardRequest:
                case InGame_Receive.RequestWallsAndSigils:
                case InGame_Receive.TeamMessage:
                case InGame_Receive.DirectMessage:
                case InGame_Receive.BroadcastMessage:
                case InGame_Receive.AllPlayerData:
                    EnqueueForAsyncProcessing();
                    return true;
                case InGame_Receive.ChangedObjectState:
                    HandleObjectStateChange();
                    return true;
                case InGame_Receive.LeaveMatch:
                    _owningMatch.LeaveMatch(_verified, false);
                    return true;
                case InGame_Receive.InactivityCheckResponse:
                    _owningMatch.GetMatchCharacter(_senderID).ResetDuration();
                    return true;
                case InGame_Receive.QuitGame:
                    _owningMatch.LeaveMatch(_verified, true);
                    return true;
                case InGame_Receive.ObjectStatus:
                    _owningMatch.ProcessObjectStatusPacket(_verified);
                    return true;
                case InGame_Receive.PlayerMoved:
                    _owningMatch.UpdatePlayerLocation(_verified, _decrypted);
                    return true;
                case InGame_Receive.FetchPlayer:
                    _owningMatch.SendPlayerData( _verified, _decrypted[2]);
                    return true;
                case InGame_Receive.LeyUpdate:
                    _owningMatch.UpdatePlayerLey(_verified, _decrypted);
                    return true;
                case InGame_Receive.Tap:
                    _owningMatch.PlayerTapped(_verified);
                    return true;
                case InGame_Receive.PostureChange:
                    _owningMatch.SendToAll(_decrypted);
                    return true;
                case InGame_Receive.Cast:
                    HandleCast();
                    return true;
                case InGame_Receive.ReportHit:
                    _owningMatch.PlayerHit(_verified, ByteUtils.ExtractShort(_decrypted, 2), false);
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
                case InGame_Receive.UpdateSlotting:
                    Main.AsyncDBUpdater.AddToQueue(new AsyncDBUpdate(ControlCodes.AsyncDBUpdate_Slotting, _verified, _decrypted));
                    return true;
                case InGame_Receive.TriggeredSigil:
                    HandleSigilTrigger();
                    return true;
                case InGame_Receive.ReportHitByWall:
                    _owningMatch.PlayerHit(_verified, ByteUtils.ExtractShort(_decrypted, 2), true);
                    return true;
                case InGame_Receive.PotionTaken:
                    HandlePotionTaken();
                    return true;
                case InGame_Receive.UsePotion:
                    HandleUsePotion();
                    return true;
                case InGame_Receive.BiasPool:
                    HandlePoolBias();
                    return true;
                case InGame_Receive.FlagCaptured:
                    HandleFlagCapture();
                    return true;
                case InGame_Receive.FlagReturned:
                    HandleFlagReturn();
                    return true;
                case InGame_Receive.FlagTaken:
                    HandleFlagTaken();
                    return true;
                case InGame_Receive.DropFlag:
                    HandleFlagDrop();
                    return true;
                case InGame_Receive.FetchShrineHealth:
                    HandleShrineHealthRequest();
                    return true;
                case InGame_Receive.AdjustShrineHealth:
                    HandleShrineHealthAdjustment();
                    return true;
                case InGame_Receive.Ping:
                    EnqueueForSend(Packets.PingReply(), _remote);
                    return true;
            }
        }
        else if(_opCode == InGame_Receive.JoinedMatch){
            return HandleJoinMatchPacket(new RemoteClient(received));
        }
        return false;
    }
    protected void HandleShrineHealthRequest(){}
    protected void HandleShrineHealthAdjustment(){}
    protected void HandlePoolBias(){}
    protected void HandleFlagCapture(){}
    protected void HandleFlagReturn(){}
    protected void HandleFlagTaken(){}
    protected void HandleFlagDrop(){}
    private void HandleUsePotion(){
        if(_verified.IsAlive()){
            if(_verified.IsCarryingPotion()){
                _verified.UsePotion();
                _owningMatch.SendToPlayer(Packets.UsedPotionPacket(
                        _verified.GetCurrentHPPct(), _verified.GetCurrentManaPct()), _verified);
            }
        }
    }
    private void HandlePotionTaken(){
        if(_verified.IsAlive()){
            _owningMatch.PotionTaken(_verified);
        }
    }
    private void HandleSigilTrigger() {
        if(_verified.IsAlive()){
            Sigil triggered = _owningMatch.GetSigil(ByteUtils.ExtractShort(_decrypted,2));
            if(triggered != null){
                triggered.SigilTriggered(_verified);
            }
        }
    }
    private void HandleResistableHitPacket(){
        if(_verified.IsAlive()){
            ResistableSpell resistable =
                    (ResistableSpell) _owningMatch.GetCastSpell(ByteUtils.ExtractShort(_decrypted, 2));
            if(resistable != null){
                resistable.ProcessSpell(_verified);
            }
        }
    }

    private void HandleWallHit(){
        DamagingSpell projectile = (DamagingSpell)_owningMatch.GetCastSpell(ByteUtils.ExtractShort(_decrypted, 2));
        if(projectile != null){
            Wall wall = _owningMatch.GetWall(ByteUtils.ExtractShort(_decrypted, 4));
            if(wall != null){
                wall.TakeDamage(projectile);
            }
        }
    }
    private void HandleSplashHit(){
        short castID = ByteUtils.ExtractShort(_decrypted, 2);
        _verified.RegisterSplashHit(castID);
        _owningMatch.PlayerHit(_verified, castID, false);
        _verified.DeregisterSplashHit(castID);
    }
    private void HandleCast(){
        if(_verified.IsAlive()){
            byte spellID = _decrypted[3];
            if(SpellManager.ContainsSpell(spellID)){
                Spell spellReference = SpellManager.GetSpell(spellID);
                short castID = _verified.CastSpell(spellReference, _decrypted); // instantiation downstream
                if(castID != -1){
                    byte[] toSend = Packets.CastPacket(_decrypted, castID);
                    switch(spellReference.GetNotificationCode()){
                        case ControlCodes.SpellNotification_All:
                            _owningMatch.SendToAll(toSend);
                            break;
                        case ControlCodes.SpellNotification_TeamOnly:
                            _owningMatch.SendToCollection(toSend, _owningMatch.GetMatchTeam(_verified.GetTeamID())
                                    .GetRemoteClients());
                            break;
                        case ControlCodes.SpellNotification_CasterOnly:
                            _owningMatch.SendToPlayer(toSend, _verified);
                            break;
                        case ControlCodes.SpellNotification_Payload: // sends to the player(s) identified in the payload
                            _owningMatch.SendToPlayer(toSend, _verified);
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
            Main.LogError("IGPP.HandleCast: Invalid caster. CasterID: " + _verified.GetIDinMatch());
        }
    }
    private void HandleObjectStateChange(){
        _owningMatch.ChangeObjectState(_decrypted[2], _decrypted[3], _verified, _decrypted[4]);
    }
    protected boolean HandleJoinMatchPacket(RemoteClient remote){
        int accountID = CheckAccountAndCharacter();
        if(accountID >= 0){
            byte idInMatch = _decrypted[9];
            byte teamID = _decrypted[10];
            Main.LogMessage("Verifying player " + idInMatch + " for match " + _owningMatch.ObjectID() + ", team " + teamID);
            if(_owningMatch.IsAwaitingVerification(accountID)){
                RemoteClient pregameClient = RemoteClientManager.GetClient(accountID);
                pregameClient.MarkInGame();
                _owningMatch.MarkPlayerVerified(idInMatch, teamID, accountID, remote);
                remote.SetNameAndID(pregameClient.GetUserName(), accountID);
                SendPlayerDataForJoinee(_owningMatch.GetMatchCharacter(idInMatch));
                _owningMatch.ProcessObjectStatusPacket(_owningMatch.GetMatchCharacter(_decrypted[9]));
                return true;
            }
            else if (_owningMatch.IsPlayerVerified(idInMatch)){
                SendPlayerDataForJoinee(_owningMatch.GetMatchCharacter(idInMatch));
                return true;
            }
            else{
                Main.LogMessage("Player " + idInMatch + " NOT verified for match " + _owningMatch.ObjectID() + ", team " + teamID);
            }
        }
        return false;
    }
    private void SendPlayerDataForJoinee(MatchCharacter joinee){
        _owningMatch.SendToAll(Packets.PlayerDataPacket(joinee.GetPlayerData(), joinee.IsNewToMatch()));
    }
    private int CheckAccountAndCharacter(){
        RemoteClient remote = LoggedInClient();
        if(remote != null){
            int accountID = (int)remote.ObjectID();
            if(ByteUtils.ExtractInt(_decrypted, 5) == GameServer.GetActiveCharacter(accountID).GetCharacterID()){
                Main.LogMessage("Account check passed: " + accountID + ", match " + _owningMatch.ObjectID());
                return accountID;
            }
        }
        return -1;
    }
    public boolean IsVerified(byte playerID){
        return _owningMatch.IsPlayerVerified(playerID);
    }
}
