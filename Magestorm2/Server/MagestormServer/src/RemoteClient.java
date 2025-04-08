import java.net.DatagramPacket;
import java.net.InetAddress;

public class RemoteClient {

    private int _emanatingPort, _receivingPort;
    private InetAddress _address;

    public RemoteClient(DatagramPacket received, int receivingPort){
        _emanatingPort = received.getPort();
        _receivingPort = receivingPort;
        _address = received.getAddress();
        Main.LogMessage("Remote client IP: " + _address.getHostAddress() + ":" + _emanatingPort);
    }

    public InetAddress IPAddress(){
        return _address;
    }

    public int EmanatingPort(){
        return _emanatingPort;
    }

    public int ReceivingPort(){
        return _receivingPort;
    }
}
