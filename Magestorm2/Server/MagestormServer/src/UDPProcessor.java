import java.math.BigInteger;
import java.net.DatagramPacket;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

public class UDPProcessor extends RegisteredThread{

    protected RegisteredThread _processor, _sender, _listener;
    protected RemoteClient _remote;
    protected DatagramPacket _received;
    protected final UDPClient _udpClient;
    protected final BlockingQueue<OutgoingPacket> _outgoingPackets;
    protected final BlockingQueue<DatagramPacket> _toProcess;
    protected byte[] _decrypted;
    protected final int _listeningPort;
    protected byte _opCode;
    protected Cryptographer _cryptographer;

    public UDPProcessor(int listeningPort, byte encryptionType){
        _cryptographer = new Cryptographer(encryptionType);
        _listeningPort = listeningPort;
        _udpClient = new UDPClient(listeningPort, this);
        _outgoingPackets = new LinkedBlockingQueue<>();
        _toProcess = new LinkedBlockingQueue<>();
        _terminated = false;

        _listener = new RegisteredThread(_udpClient);
        _listener.start();
        _processor = new RegisteredThread(this);
        _processor.start();
        _sender = new RegisteredThread(new PacketSender(_udpClient, this));
        _sender.start();
    }
    public byte[] GetSymmetricKey(){
        return _cryptographer.GetCipher().GetSecretSymmetricKeyBytes();
    }
    public BigInteger GetSymmetricKeyBI(){
        return _cryptographer.GetCipher().GetSecretSymmetricKeyBI();
    }
    public void TerminateProcessor(){
        Main.LogMessage("Terminating UDPProcessor for port: " + _listeningPort);
        _terminated = true;
        _listener.interrupt();
        _processor.interrupt(); // interrupt the blocking take()
        _sender.interrupt();
    }
    protected boolean PreProcess(DatagramPacket received){
        _decrypted = _cryptographer.Decrypt(received.getData());
        _opCode = _decrypted[0];
        _received = received;
        return true;
    }

    protected RemoteClient LoggedInClient(){
        int accountID = ByteUtils.ExtractInt(_decrypted, 1);
        return RemoteClientManager.GetClient(accountID);
    }
    public BlockingQueue<OutgoingPacket> OutgoingQueue(){
        return _outgoingPackets;
    }
    public void EnqueueForSendWithChecksum(byte[] toEncrypt, RemoteClient rc){
        byte[] toEncryptWithChecksum = Cryptographer.ApplyChecksum(toEncrypt);
        EnqueueForSend(toEncryptWithChecksum, rc);
    }
    public void EnqueueForSend(byte[] toEncrypt, Iterable<RemoteClient> recipients){
        _outgoingPackets.add(new OutgoingPacket(_cryptographer.Encrypt(toEncrypt), recipients));
    }
    public void EnqueueForSend(byte[] toEncrypt, RemoteClient rc){
        _outgoingPackets.add(new OutgoingPacket(_cryptographer.Encrypt(toEncrypt), rc));
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
        Register("UDPProcessor, port " + _listeningPort);
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
