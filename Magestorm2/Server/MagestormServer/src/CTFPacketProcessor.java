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
                        HandlePoolBias();
                        break;
                    case InGame_Receive.TeamMessage:
                        HandleTeamMessage();
                        break;
                    case InGame_Receive.FlagCaptured:
                        HandleFlagCapture();
                        break;
                    case InGame_Receive.FlagReturned:
                        HandleFlagReturn();
                        break;
                    case InGame_Receive.FlagTaken:
                        break;
                }
            }


        }
        return false;
    }
    private void HandleFlagReturn(){


    }

    private void HandleFlagCapture(){
        _owningCTF.FlagCaptured(_decrypted[1], _decrypted[2]);
    }

    private boolean HandlePoolBias(){
        if(IsVerified()){
            _owningCTF.GetPoolManager().BiasPool(_decrypted[1], _decrypted[2], _remote);
        }
        return true;
    }

    private boolean HandleTeamMessage(){
        if(IsVerified()){
            SharedHandlers.HandleTeamMessage(_decrypted, this, _owningCTF, _remote);
        }
        return true;
    }

}
