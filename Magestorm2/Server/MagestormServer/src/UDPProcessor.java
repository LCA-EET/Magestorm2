import java.net.DatagramPacket;
import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentLinkedQueue;

public class UDPProcessor extends Thread{

    protected RemoteClient _remote;
    protected DatagramPacket _received;
    protected final UDPClient _udpClient;
    protected final PacketSender _sender;
    protected final ConcurrentLinkedQueue<OutgoingPacket> _outgoingPackets;
    protected final ConcurrentLinkedQueue<DatagramPacket> _toProcess;
    protected byte[] _decrypted;
    protected final int _listeningPort;
    protected byte _opCode;
    protected boolean _terminated;

    public UDPProcessor(int listeningPort){
        _listeningPort = listeningPort;
        _udpClient = new UDPClient(listeningPort, this);
        _outgoingPackets = new ConcurrentLinkedQueue<>();
        _toProcess = new ConcurrentLinkedQueue<>();
        _terminated = false;
        _sender = new PacketSender(_udpClient, this);
        new Thread(this).start();
    }
    public boolean IsTerminated(){
        return _terminated;
    }
    public void TerminateProcessor(){
        _terminated = true;
    }
    protected void PreProcess(DatagramPacket received){
        _decrypted = Cryptographer.Decrypt(received.getData());
        _opCode = _decrypted[0];
        _received = received;
    }

    protected RemoteClient LoggedInClient(){
        int accountID = ByteUtils.ExtractInt(_decrypted, 1);
        return GameServer.GetClient(accountID);
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

    public void EnqueueForSend(byte[] encrypted, Iterable<RemoteClient> recipients){
        _outgoingPackets.add(new OutgoingPacket(encrypted, recipients));
    }
    public void EnqueueForSend(byte[] encrypted, RemoteClient rc){
        _outgoingPackets.add(new OutgoingPacket(encrypted, rc));
    }

    protected boolean ProcessPacket(DatagramPacket received){
        Main.LogError("Unimplemented packet handler.");
        return true;
    }

    public void EnqueuePacket(DatagramPacket received){
        _toProcess.add(received);
    }

    @Override
    public void run(){
        while(!_terminated){
            try{
                if(!_toProcess.isEmpty()){
                    ProcessPacket(_toProcess.poll());
                }
            }
            catch(Exception ex){
                Main.LogError(ex.getMessage());
                Main.LogStackTrace(ex);
            }
        }
        Main.LogMessage("UDPProcessor on port " + _listeningPort + " has terminated.");
    }
}
