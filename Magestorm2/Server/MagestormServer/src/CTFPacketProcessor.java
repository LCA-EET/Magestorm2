public class CTFPacketProcessor extends InGamePacketProcessor{
    CaptureTheFlag _owningCTF;
    public CTFPacketProcessor(int port, CaptureTheFlag owningMatch){
        super(port, owningMatch);
        _owningCTF = owningMatch;
    }
    @Override
    protected void HandlePoolBias(){
        _owningCTF.GetPoolManager().BiasPool(_verified, _decrypted[2], _remote);
    }
    @Override
    protected void HandleFlagCapture(){
        _owningCTF.FlagCaptured(_verified, _decrypted[2]);
    }
    @Override
    protected void HandleFlagReturn(){
        _owningCTF.FlagReturnedByPlayer(_verified, _decrypted[2]);
    }
    @Override
    protected void HandleFlagTaken(){
        _owningCTF.FlagTaken( _verified,_decrypted[2],_senderID);
    }
    @Override
    protected void HandleFlagDrop(){
        _owningCTF.SeeIfFlagDropped(_verified, _senderID);
    }
}
