import java.net.DatagramPacket;

public class DMPacketProcessor extends InGamePacketProcessor{
    private final DeathMatch _owningDM;
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
        _owningDM.AdjustShrineHealth(_decrypted[1], _decrypted[2]);
        return true;
    }
    private boolean HandlePoolBias(){
        _owningDM.GetPoolManager().BiasPool(_decrypted[1], _decrypted[2], _remote);
        return true;
    }
    private boolean HandleShrineHealthRequest(){
        SendShrineHealthPacket();
        return true;
    }
    private void SendShrineHealthPacket(){
        byte[] health = _owningDM.ReportAllShrineHealth();
        EnqueueForSend(Packets.AllShrineHealthPacket(health[0], health[1], health[2]), _remote);
    }
    private boolean HandleTeamMessage(){
        SharedFunctions.HandleTeamMessage(_decrypted, this, _owningDM, _remote);
        return true;
    }

    @Override
    protected boolean HandleJoinMatchPacket(RemoteClient remote){
        if(super.HandleJoinMatchPacket(remote)){
            SendShrineHealthPacket();
        }
        return true;
    }

}
