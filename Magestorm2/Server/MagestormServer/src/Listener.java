import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class Listener extends Thread{
    private DatagramSocket _udpSocket;
    private boolean _listening;
    private byte[] _received;
    private final PacketProcessor _processor;
    private final int _localPort;

    public Listener(int localPort, PacketProcessor processor){
        _listening = true;
        _processor = processor;
        _localPort = localPort;
        _received = new byte[256];
        try{
            _udpSocket = new DatagramSocket(localPort);
        }
        catch(Exception e){

        }
    }
    @Override
    public void run() {
        while (_listening) {
            DatagramPacket receivedPacket = new DatagramPacket(_received, _received.length);
            try {
                _udpSocket.receive((receivedPacket));
                _processor.ProcessPacket(receivedPacket);
            } catch (Exception e) {

            }
        }
    }
    public void StopListening(){
        _listening = false;
    }
}
