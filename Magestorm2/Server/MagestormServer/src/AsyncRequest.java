public class AsyncRequest {
    protected final byte[] _decrypted;
    protected final byte _opCode;
    protected final RemoteClient _sender;

    public AsyncRequest(byte[] decrypted, byte opCode, RemoteClient sender){
        _decrypted = decrypted;
        _opCode = opCode;
        _sender = sender;
    }

    public byte GetOpCode(){
        return _opCode;
    }

    public byte[] GetBytes(){
        return _decrypted;
    }

    public RemoteClient GetSender(){
        return _sender;
    }
}
