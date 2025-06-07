import java.net.DatagramPacket;
import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentLinkedQueue;

public class UDPProcessor {

    protected RemoteClient _remote;
    protected final UDPClient _udpClient;
    protected final PacketSender _sender;
    protected final ConcurrentLinkedQueue<OutgoingPacket> _outgoingPackets;
    protected byte[] _decrypted;
    protected final int _listeningPort;
    protected byte _opCode;
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
    protected void PreProcess(DatagramPacket received){
        _remote = new RemoteClient(received, _listeningPort);
        _decrypted = Cryptographer.Decrypt(received.getData());
        _opCode = _decrypted[0];
    }
    protected int IsLoggedIn(){
        int accountID = ByteUtils.ExtractInt(_decrypted, 1);
        return GameServer.IsLoggedIn(accountID) ? accountID: 0;
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

    public void EnqueueForSend(byte[] data, RemoteClient rc){
        _outgoingPackets.add(new OutgoingPacket(data, rc));
    }
    public void EnqueueForSend(byte[] data, RemoteClient[] rc){
        _outgoingPackets.add(new OutgoingPacket(data, rc));
    }

    protected void ProcessPacket(DatagramPacket received){}
}
