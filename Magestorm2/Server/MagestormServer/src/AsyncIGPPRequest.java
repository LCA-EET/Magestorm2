public class AsyncIGPPRequest extends AsyncRequest{
    private final MatchCharacter _verified;

    public AsyncIGPPRequest(byte[] decrypted, byte opCode, MatchCharacter verified, RemoteClient sender){
        super(decrypted, opCode, sender);
        _verified = verified;
    }
    public MatchCharacter GetVerifiedCharacter(){
        return _verified;
    }

}
