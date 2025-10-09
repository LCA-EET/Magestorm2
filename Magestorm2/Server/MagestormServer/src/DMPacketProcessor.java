import java.net.DatagramPacket;
import java.util.ArrayList;

public class DMPacketProcessor extends InGamePacketProcessor{
    DeathMatch _owningDM;
    public DMPacketProcessor(int port, DeathMatch owningMatch){
        super(port, owningMatch);
        _owningDM = owningMatch;
    }
    @Override
    protected boolean ProcessPacket(DatagramPacket received) {
        if(!super.ProcessPacket(received)){
            switch(_opCode){
                case InGame_Receive.BiasPool:
                    return HandlePoolBias();
                case InGame_Receive.FetchShrineHealth:
                    return HandleShrineHealthRequest();
                case InGame_Receive.TeamMessage:
                    return HandleTeamMessage();
                case InGame_Receive.AdjustShrineHealth:
                    return HandleShrineAdjustment();
            }

        }
        return false;
    }
    private boolean HandleShrineAdjustment(){
        if(IsVerified()){
            _owningDM.AdjustShrineHealth(_decrypted[1], _decrypted[2]);
        }
        return true;
    }
    private boolean HandlePoolBias(){
        if(IsVerified()){
            _owningDM.GetPoolManager().BiasPool(_decrypted[1], _decrypted[2], _remote);
        }
        return true;
    }
    private boolean HandleShrineHealthRequest(){
        if(IsVerified()){
            SendShrineHealthPacket();
        }
        return true;
    }
    private void SendShrineHealthPacket(){
        byte[] health = _owningDM.ReportAllShrineHealth();
        EnqueueForSend(Packets.AllShrineHealthPacket(health[0], health[1], health[2]), _remote);
    }
    private boolean HandleTeamMessage(){
        if(IsVerified()){
            byte teamID = _decrypted[2];
            int messageLength = ByteUtils.ExtractInt(_decrypted, 3);
            String messageString = ByteUtils.BytesToUTF8(_decrypted, 7, messageLength);
            if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
                EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                        _remote);
            }
            else{
                if(_owningDM.GetMatchCharacter(_decrypted[1]).GetTeamID() == teamID){
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_Send.TeamMessage),
                            _owningDM.GetMatchTeam(teamID).GetRemoteClients());
                }
                else {
                    ArrayList<RemoteClient> recipients = new ArrayList<RemoteClient>();
                    recipients.add(_remote);
                    recipients.addAll(_owningDM.GetMatchTeam(teamID).GetRemoteClients());
                    EnqueueForSend(Packets.MessagePacket(_decrypted, InGame_Send.TeamMessage),
                            recipients);
                }
            }
        }
        return true;
    }

    @Override
    protected boolean HandleJoinMatchPacket(){
        if(super.HandleJoinMatchPacket()){
            SendShrineHealthPacket();
        }
        return true;
    }

}
