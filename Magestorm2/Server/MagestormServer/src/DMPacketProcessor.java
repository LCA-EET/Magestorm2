import java.net.DatagramPacket;

public class DMPacketProcessor extends InGamePacketProcessor{
    private final DeathMatch _owningDM;
    public DMPacketProcessor(int port, DeathMatch owningMatch){
        super(port, owningMatch);
        _owningDM = owningMatch;
    }

    @Override
    protected boolean HandleJoinMatchPacket(RemoteClient remote){
        if(super.HandleJoinMatchPacket(remote)){
            HandleShrineHealthRequest();
        }
        return true;
    }
    @Override
    protected void HandleShrineHealthRequest(){
        byte[] health = _owningDM.ReportAllShrineHealth();
        EnqueueForSend(Packets.AllShrineHealthPacket(health[0], health[1], health[2]), _remote);
    }
    @Override
    protected void HandleShrineHealthAdjustment(){
        _owningDM.AdjustShrineHealth(_verified, _decrypted[2]);
    }
    @Override
    protected void HandlePoolBias(){
        _owningDM.GetPoolManager().BiasPool(_verified, _decrypted[2], _remote);
    }
}
