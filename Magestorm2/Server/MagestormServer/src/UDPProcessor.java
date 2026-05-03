import java.net.DatagramPacket;
import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.LinkedBlockingQueue;

public class UDPProcessor extends RegisteredThread{

    protected RemoteClient _remote;
    protected DatagramPacket _received;
    protected final UDPClient _udpClient;
    protected final PacketSender _sender;
    protected final ConcurrentLinkedQueue<OutgoingPacket> _outgoingPackets;
    protected final BlockingQueue<DatagramPacket> _toProcess;
    protected byte[] _decrypted;
    protected final int _listeningPort;
    protected byte _opCode;

    public UDPProcessor(int listeningPort){
        _listeningPort = listeningPort;
        _udpClient = new UDPClient(listeningPort, this);
        _outgoingPackets = new ConcurrentLinkedQueue<>();
        _toProcess = new LinkedBlockingQueue<>();
        _terminated = false;
        _sender = new PacketSender(_udpClient, this);
        new RegisteredThread(this).start();
    }
    public void TerminateProcessor(){
        _terminated = true;
        this.interrupt(); // interrupt the blocking take()
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
                ProcessPacket(_toProcess.take());
            }
            catch(InterruptedException ie){
                Main.LogMessage("Terminating UDPProcessor on port " + _listeningPort);
            }
            catch(Exception ex){
                Main.LogError(ex.getMessage());
                Main.LogStackTrace(ex);
            }
        }
        Main.LogMessage("UDPProcessor on port " + _listeningPort + " has terminated.");
        Deregister();
    }
}
