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
        switch(_opCode){
            case InGame_Receive.JoinedMatch:
                HandleJoinMatchPacket();
                return true;
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
            case InGame_Receive.HitPlayer:
                HandleHitPlayer();
                return true;
            case InGame_Receive.CastSpell:
                HandleSpellCast();
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
        }
        return false;
    }
    private void HandleFetchPlayer(){
        if(IsVerified()){
            _owningMatch.SendPlayerData( _decrypted[1], _decrypted[2]);
        }
    }
    private void HandlePlayerMoved(){
        if(IsVerified()){
            _owningMatch.UpdatePlayerLocation(_decrypted);
        }
    }
    private void HandleObjectStatusRequest(){
        if(IsVerified()){
            _owningMatch.ProcessObjectStatusPacket(_decrypted[1]);
        }
    }
    private void HandleSpellCast(){
        if(IsVerified()){
            _owningMatch.ProcessSpellCast(_decrypted);
        }
    }
    private void HandleHitPlayer(){
        if(IsVerified()){
            _owningMatch.PlayerHit(_decrypted[1], _decrypted[2], ByteUtils.ExtractInt(_decrypted, 3));
        }
    }
    private void InactivityCheckResponse(){
        if(IsVerified()){
            _owningMatch.GetMatchCharacter(_decrypted[1]).MarkPacketReceived();
        }
    }
    private void HandleQuitGame(){
        if(IsVerified()){
            _owningMatch.LeaveMatch(_decrypted[1], _decrypted[2], true);
            int accountID = ByteUtils.ExtractInt(_decrypted, 3);
            GameServer.ClientLoggedOut(accountID);
        }
    }
    private void HandleLeaveMatch(){
        if(IsVerified()){
            _owningMatch.LeaveMatch(_decrypted[1], _decrypted[2], true);
        }
    }

    private void HandleBroadcastMessage(){
        if(IsVerified()){
            int messageLength = ByteUtils.ExtractInt(_decrypted,2);
            String messageString = ByteUtils.BytesToUTF8(_decrypted, 6, messageLength);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                        _remote);
            }
            else{
                EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_Send.BroadcastMessage),
                        _owningMatch.GetVerifiedClients());
            }
        }
    }
    private void HandleDirectMessage(){
        if(IsVerified()){
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
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_Send.DirectMessage), recipients);
                }
            }
        }
    }
    private void HandleObjectStateChange(){
        if(IsVerified()) {
            byte objectID = _decrypted[2];
            byte state = _decrypted[3];
            _owningMatch.ChangeObjectState(objectID, state, _decrypted[1]);
        }
    }
    protected boolean HandleJoinMatchPacket(){
        int accountID = CheckAccountAndCharacter();
        if(accountID >= 0){
            byte idInMatch = _decrypted[9];
            byte teamID = _decrypted[10];
            Main.LogMessage("Verifying player " + idInMatch + " for match " + _owningMatch.MatchID() + ", team " + teamID);
            if(_owningMatch.IsAwaitingVerification(accountID)){
                _owningMatch.MarkPlayerVerified(idInMatch, teamID, accountID);
                _owningMatch.SendToAll(Packets.PlayerDataPacket(_owningMatch.GetMatchCharacter(idInMatch).GetINLCTABytes()));
                _owningMatch.ProcessObjectStatusPacket(_decrypted[9]);
                Main.LogMessage("Player " + idInMatch + " verified for match " + _owningMatch.MatchID() + ", team " + teamID);
                return true;
            }
            else{
                Main.LogMessage("Player " + idInMatch + " NOT verified for match " + _owningMatch.MatchID() + ", team " + teamID);
            }
        }
        return false;
    }
    private int CheckAccountAndCharacter(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            if(ByteUtils.ExtractInt(_decrypted, 5) == GameServer.GetClient(accountID).GetActiveCharacter().GetCharacterID()){
                Main.LogMessage("Account check passed: " + accountID + ", match " + _owningMatch.MatchID());
                return accountID;
            }
        }
        Main.LogMessage("Account check failure: " + accountID + ", match " + _owningMatch.MatchID());
        return -1;
    }
    protected boolean IsVerified(){
        return _owningMatch.IsPlayerVerified(_decrypted[1]);
    }
    private boolean IsVerified(byte playerID){
        return _owningMatch.IsPlayerVerified(playerID);
    }
}
