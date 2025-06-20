import java.rmi.Remote;
import java.util.ArrayList;

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
    public OutgoingPacket(byte[] bytes, ArrayList<RemoteClient> recipients){
        _data = bytes;
        _recipients = (RemoteClient[])recipients.toArray();
    }
    public RemoteClient[] Recipients(){
        return _recipients;
    }
    public byte[] Bytes(){
        return _data;
    }
}
