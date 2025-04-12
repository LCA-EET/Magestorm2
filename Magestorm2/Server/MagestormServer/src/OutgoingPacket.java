import java.rmi.Remote;

public class OutgoingPacket {
    private byte[] _data;
    private RemoteClient _rc;
    public OutgoingPacket(byte[] bytes, RemoteClient rc){
        _data = bytes;
        _rc = rc;
    }
    public RemoteClient Recipient(){
        return _rc;
    }
    public byte[] Bytes(){
        return _data;
    }
}
