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
            SharedHandlers.HandleTeamMessage(_decrypted, this, _owningDM, _remote);
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
