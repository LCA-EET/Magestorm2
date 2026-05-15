import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.SocketException;
import java.nio.channels.ClosedByInterruptException;

public class UDPClient extends RegisteredThread{
    private DatagramSocket _udpSocket;
    private final int _localPort;
    private final UDPProcessor _processor;

    public UDPClient(int localPort, UDPProcessor processor){
        _localPort = localPort;
        _processor = processor;
    }
    @Override
    public void run() {
        Register("UDPClient, port " + _localPort);
        try{
            _udpSocket = new DatagramSocket(_localPort);
        }
        catch(Exception e){
            Main.LogError("Could not open datagram socket on port: " + _localPort + ", " + e.getMessage());
        }
        final int bufferSize = 256;
        byte[] receivedBuffer = new byte[bufferSize];
        while (!_terminated) {
            DatagramPacket receivedPacket = new DatagramPacket(receivedBuffer, receivedBuffer.length);
            try {
                _udpSocket.receive((receivedPacket));
                _processor.EnqueuePacket(receivedPacket);
            }
            catch(ClosedByInterruptException | SocketException ie){
                _terminated = true;
            }
            catch (Exception e){ }
            receivedBuffer = new byte[bufferSize];
        }
        GameServer.RemoveUsedPort(_localPort);
        Deregister();
    }
    public void StopListening() {
        _terminated = true;
        _udpSocket.close(); // closing the socket causes the blocking receive method to throw a SocketException
    }
    public int GetLocalPort(){
        return _localPort;
    }
    public void Send(byte[] encryptedPayload, RemoteClient rc){
        try{
            _udpSocket.send(new DatagramPacket(encryptedPayload, encryptedPayload.length, rc.IPAddress(), rc.GetRemotePort()));
        }catch(Exception e){
            Main.LogStackTrace(e);
        }
    }
}
