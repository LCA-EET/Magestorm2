import java.util.Collections;

public class OutgoingPacket {
    private byte[] _data;
    private Iterable _recipients;
    
    public OutgoingPacket(byte[] bytes, RemoteClient rc){
        _data = bytes;
        _recipients = Collections.singletonList(rc);
    }
    public OutgoingPacket(byte[] bytes, Iterable<RemoteClient> recipients){
        _data = bytes;
        _recipients = recipients;
    }
    public Iterable<RemoteClient> Recipients(){
        return _recipients;
    }
    public byte[] Bytes(){
        return _data;
    }
}
