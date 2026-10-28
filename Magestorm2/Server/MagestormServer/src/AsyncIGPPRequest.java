public class AsyncIGPPRequest {
    private final byte _opCode;
    private final byte[] _decrypted;
    private final MatchCharacter _verified;
    private final RemoteClient _sender;

    public AsyncIGPPRequest(byte[] decrypted, byte opCode, MatchCharacter verified, RemoteClient sender){
        _decrypted = decrypted;
        _verified = verified;
        _opCode = opCode;
        _sender = sender;
    }

    public byte GetOpCode(){
        return _opCode;
    }

    public byte[] GetBytes(){
        return _decrypted;
    }

    public MatchCharacter GetVerifiedCharacter(){
        return _verified;
    }

    public RemoteClient GetSender(){
        return _sender;
    }
}
