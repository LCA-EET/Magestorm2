import java.rmi.Remote;

public class OutgoingPacket {
    private byte[] _data;
    private RemoteClient[] _recipients;
    public OutgoingPacket(byte[] bytes, RemoteClient rc){
        _data = bytes;
        _recipients = new RemoteClient[]{rc};
    }
    public OutgoingPacket(byte[] bytes, RemoteClient[] rc){
        _data = bytes;
        _recipients = rc;
    }
    public RemoteClient[] Recipients(){
        return _recipients;
    }
    public byte[] Bytes(){
        return _data;
    }
}
