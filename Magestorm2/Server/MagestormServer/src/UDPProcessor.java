import java.net.DatagramPacket;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentLinkedQueue;

public class UDPProcessor {

    protected final UDPClient _udpClient;
    protected final PacketSender _sender;
    protected final ConcurrentLinkedQueue<OutgoingPacket> _outgoingPackets;
    protected final int _listeningPort;
    protected boolean _terminated;

    public UDPProcessor(int listeningPort){
        _listeningPort = listeningPort;
        _udpClient = new UDPClient(listeningPort, this);
        _outgoingPackets = new ConcurrentLinkedQueue<>();
        _terminated = false;
        _sender = new PacketSender(_udpClient, this);
    }
    public boolean IsTerminated(){
        return _terminated;
    }
    public void TerminateProcessor(){
        _terminated = true;
    }
    public ArrayList<OutgoingPacket> OutgoingPackets(){
        ArrayList<OutgoingPacket> toReturn = new ArrayList<>();
        while(!_outgoingPackets.isEmpty()){
            toReturn.add(_outgoingPackets.remove());
        }
        return toReturn;
    }

    public boolean HasOutgoingPackets(){
        return !_outgoingPackets.isEmpty();
    }

    protected void ProcessPacket(DatagramPacket received){}
}
