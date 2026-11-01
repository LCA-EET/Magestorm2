import java.net.DatagramPacket;
import java.net.InetAddress;
import java.util.Arrays;

public class UDPTransmission {
    private final byte[] _data;
    private final int _originPort;
    private final InetAddress _origin;

    public UDPTransmission(DatagramPacket received){
        _data = Arrays.copyOf(
                received.getData(),
                received.getLength());
        _originPort = received.getPort();
        _origin = received.getAddress();
    }

    public InetAddress OriginAddress(){
        return _origin;
    }

    public int OriginPort(){
        return _originPort;
    }

    public byte[] GetData(){
        return _data;
    }
}
