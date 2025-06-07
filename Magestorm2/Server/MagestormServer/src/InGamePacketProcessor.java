import java.net.DatagramPacket;
import java.util.concurrent.ConcurrentLinkedQueue;

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
        }
    }
    public void HandlePlayerDataRequest(){
        if(IsVerified()){
            byte requestedPlayer = _decrypted[3];
            EnqueueForSend(Packets.PlayerDataPacket(_owningMatch.PlayerData(requestedPlayer)), _remote);
        }
    }
    public void HandleJoinMatchPacket(){
        if(CheckAccountAndCharacter()){
            byte idInMatch = _decrypted[9];
            byte teamID = _decrypted[10];
            if(_owningMatch.IsPlayerOnTeam(idInMatch, teamID)){
                _owningMatch.MarkPlayerVerified(idInMatch);
                EnqueueForSend(_owningMatch.PlayersInMatch(InGame_OpCode_Send.PlayersInMatch), _remote);
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
}
