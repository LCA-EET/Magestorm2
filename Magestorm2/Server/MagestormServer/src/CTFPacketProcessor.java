import java.net.DatagramPacket;

public class CTFPacketProcessor extends InGamePacketProcessor{
    CaptureTheFlag _owningCTF;
    public CTFPacketProcessor(int port, CaptureTheFlag owningMatch){
        super(port, owningMatch);
        _owningCTF = owningMatch;
    }

    @Override
    protected boolean ProcessPacket(DatagramPacket received) {
        if(!super.ProcessPacket(received)){
            if(IsVerified()){
                switch(_opCode){
                    case InGame_Receive.BiasPool:
                        _owningCTF.GetPoolManager().BiasPool(_decrypted[1], _decrypted[2], _remote);
                        break;
                    case InGame_Receive.TeamMessage:
                        SharedHandlers.HandleTeamMessage(_decrypted, this, _owningCTF, _remote);
                        break;
                    case InGame_Receive.FlagCaptured:
                        _owningCTF.FlagCaptured(_decrypted[1], _decrypted[2]);
                        break;
                    case InGame_Receive.FlagReturned:
                        _owningCTF.FlagReturned(_decrypted[1], _decrypted[2]);
                        break;
                    case InGame_Receive.FlagTaken:
                        break;
                }
            }


        }
        return false;
    }

}
