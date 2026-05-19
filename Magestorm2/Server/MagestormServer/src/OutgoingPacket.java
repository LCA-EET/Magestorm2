import java.util.Collections;

public class OutgoingPacket {
    private final byte[] _data;
    private final Iterable<RemoteClient> _recipients;
    
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
