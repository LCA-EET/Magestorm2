import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class UDPClient extends Thread{
    private DatagramSocket _udpSocket;
    private boolean _listening;
    private final int _localPort;
    private PacketProcessor _processor;

    public UDPClient(int localPort, PacketProcessor processor){
        _listening = true;
        _localPort = localPort;
        _processor = processor;
        try{
            _udpSocket = new DatagramSocket(localPort);
        }
        catch(Exception e){

        }
    }
    @Override
    public void run() {
        Main.LogMessage("Listening on port " + _localPort);
        byte[] receivedBuffer = new byte[256];
        while (_listening) {
            DatagramPacket receivedPacket = new DatagramPacket(receivedBuffer, receivedBuffer.length);
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
