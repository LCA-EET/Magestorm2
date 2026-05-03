import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class UDPClient extends RegisteredThread{
    private DatagramSocket _udpSocket;
    private boolean _listening;
    private final int _localPort;
    private final UDPProcessor _processor;

    public UDPClient(int localPort, UDPProcessor processor){
        _listening = true;
        _localPort = localPort;
        _processor = processor;
        try{
            _udpSocket = new DatagramSocket(_localPort);
            new RegisteredThread(this).start();
        }
        catch(Exception e){
            Main.LogError("Could not open datagram socket on port: " + _localPort + ", " + e.getMessage());
        }
    }
    @Override
    public void run() {
        Main.LogMessage("UDPClient.run(): Listening on port " + _localPort);
        final int bufferSize = 256;
        byte[] receivedBuffer = new byte[bufferSize];
        while (_listening) {
            DatagramPacket receivedPacket = new DatagramPacket(receivedBuffer, receivedBuffer.length);
            try {
                _udpSocket.receive((receivedPacket));
                _processor.EnqueuePacket(receivedPacket);
            }catch (Exception e){ }
            receivedBuffer = new byte[bufferSize];
        }
        Main.LogMessage("UDP client on port " + _localPort + " is no longer listening.");
        Deregister();
    }
    public void StopListening() {
        _listening = false;
        _udpSocket.close();
        Main.LogMessage("UDP socket on port " + _localPort + " is closed.");
    }
    public void Send(byte[] encryptedPayload, RemoteClient rc){
        try{
            DatagramPacket toSend = new DatagramPacket(encryptedPayload, encryptedPayload.length, rc.IPAddress(), rc.GetRemotePort());
            _udpSocket.send(toSend);
        }catch(Exception e){
            Main.LogStackTrace(e);
        }
    }
}
