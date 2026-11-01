public class AsyncPGPPRequest extends AsyncRequest{
    private final int _accountID;
    public AsyncPGPPRequest(byte[] decrypted, byte opCode, RemoteClient sender, int accountID){
        super(decrypted, opCode, sender);
        _accountID = accountID;
    }
    public int GetAccountID(){
        return _accountID;
    }
}
