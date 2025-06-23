import java.net.DatagramPacket;
import java.util.ArrayList;
import java.util.Arrays; 

public class InGamePacketProcessor extends UDPProcessor{
    private Match _owningMatch;

    public InGamePacketProcessor(int port, Match owningMatch){
        super(port);
        _owningMatch = owningMatch;
    }

    public void ProcessPacket(DatagramPacket received){
        PreProcess(received);
        switch(_opCode){
            case InGame_OpCode_Receive.JoinedMatch:
                HandleJoinMatchPacket();
                break;
            case InGame_OpCode_Receive.RequestPlayerData:
                HandlePlayerDataRequest();
                break;
            case InGame_OpCode_Receive.ChangedObjectState:
                HandleObjectStateChange();
                break;
            case InGame_OpCode_Receive.FetchShrineHealth:
                HandleShrineHealthRequest();
                break;
            case InGame_OpCode_Receive.DirectMessage:
                HandleDirectMessage();
                break;
            case InGame_OpCode_Receive.BroadcastMessage:
                HandleBroadcastMessage();
                break;
            case InGame_OpCode_Receive.TeamMessage:
                HandleTeamMessage();
                break;
        }
    }
    private void HandleTeamMessage(){
        if(IsVerified()){
            byte teamID = _decrypted[2];
            int messageLength = ByteUtils.ExtractInt(_decrypted, 3);
            String messageString = ByteUtils.BytesToUTF8(_decrypted, 7, messageLength);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_OpCode_Send.ProhibitedLanguage),
                        _remote);
            }
            else{
                if(_owningMatch.GetMatchCharacter(_decrypted[1]).GetTeamID() == teamID){
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_OpCode_Send.TeamMessage),
                            _owningMatch.GetMatchTeam(teamID).GetRemoteClients());
                }
                else{
                    ArrayList<RemoteClient> recipients = new ArrayList<RemoteClient>();
                    recipients.add(_remote);
                    recipients.addAll(_owningMatch.GetMatchTeam(teamID).GetRemoteClients());
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_OpCode_Send.TeamMessage),
                            recipients);
                }
            }
        }
    }
    private void HandleBroadcastMessage(){
        if(IsVerified()){
            int messageLength = ByteUtils.ExtractInt(_decrypted,2);
            String messageString = ByteUtils.BytesToUTF8(_decrypted, 6, messageLength);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_OpCode_Send.ProhibitedLanguage),
                        _remote);
            }
            else{
                EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_OpCode_Send.BroadcastMessage),
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
                    EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_OpCode_Send.ProhibitedLanguage), _remote);
                }
                else{
                    RemoteClient messageRecipient = _owningMatch.GetMatchCharacter(recipientID).GetRemoteClient();
                    Iterable<RemoteClient> recipients = Arrays.asList(_remote, messageRecipient);
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_OpCode_Send.DirectMessage), recipients);
                }
            }
        }
    }
    private void HandleShrineHealthRequest(){
        if(IsVerified()){
            SendShrineHealthPacket();
        }
    }

    private void HandleObjectStateChange(){
        if(IsVerified()) {
            byte objectID = _decrypted[2];
            byte state = _decrypted[3];
            _owningMatch.ChangeObjectState(objectID, state);
            _owningMatch.SendToAll(Packets.ObjectStateChangePacket(objectID, state));
        }
    }
    private void HandlePlayerDataRequest(){
        if(IsVerified()){
            byte requestedPlayer = _decrypted[3];
            EnqueueForSend(Packets.PlayerDataPacket(_owningMatch.PlayerData(requestedPlayer)), _remote);
        }
    }
    private void HandleJoinMatchPacket(){
        if(CheckAccountAndCharacter()){
            byte idInMatch = _decrypted[9];
            byte teamID = _decrypted[10];
            if(_owningMatch.IsPlayerOnTeam(idInMatch, teamID)){
                _owningMatch.MarkPlayerVerified(idInMatch, teamID);
                SendShrineHealthPacket();
            }
        }
    }

    private boolean CheckAccountAndCharacter(){
        int accountID = IsLoggedIn();
        if(accountID > 0){
            if(ByteUtils.ExtractInt(_decrypted, 5) == GameServer.GetClient(accountID).GetActiveCharacter().GetCharacterID()){
                return true;
            }
        }
        return false;
    }

    private boolean IsVerified(){
        return _owningMatch.IsPlayerVerified(_decrypted[1]);
    }
    private boolean IsVerified(byte playerID){
        return _owningMatch.IsPlayerVerified(playerID);
    }

    private void SendShrineHealthPacket(){
        byte[] health = _owningMatch.ReportAllShrineHealth();
        EnqueueForSend(Packets.AllShrineHealthPacket(health[0], health[1], health[2]), _remote);
    }

    
}
