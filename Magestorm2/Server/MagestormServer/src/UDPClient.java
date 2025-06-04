import java.net.DatagramPacket;
import java.net.DatagramSocket;

public class UDPClient extends Thread{
    private DatagramSocket _udpSocket;
    private boolean _listening;
    private final int _localPort;
    private final int _bufferSize = 256;
    private UDPProcessor _processor;

    public UDPClient(int localPort, UDPProcessor processor){
        _listening = true;
        _localPort = localPort;
        _processor = processor;
        try{
            _udpSocket = new DatagramSocket(_localPort);
            new Thread(this).start();
        }
        catch(Exception e){
            Main.LogError("Could not open datagram socket: " + e.getMessage());
        }
    }
    @Override
    public void run() {
        Main.LogMessage("Listening on port " + _localPort);
        byte[] receivedBuffer = new byte[_bufferSize];
        boolean dataReceived = false;
        while (_listening) {
            DatagramPacket receivedPacket = new DatagramPacket(receivedBuffer, receivedBuffer.length);
            try {
                _udpSocket.receive((receivedPacket));
                dataReceived = true;
                _processor.ProcessPacket(receivedPacket);
                dataReceived = false;
            } catch (Exception e)
            {
                if(dataReceived){
                    Main.LogError("UDPClient.run():" + e.getMessage());
                }
            }
            receivedBuffer = new byte[_bufferSize];
        }
        Main.LogMessage("UDP client on port " + _localPort + " is no longer listening.");
    }
    public void StopListening()
    {
        _listening = false;
    }
    public void Send(byte[] encryptedPayload, RemoteClient rc){
        Main.LogMessage("Sending packet to " + rc.IPAddress().toString() + ", " + rc.ReceivingPort());
        DatagramPacket toSend = new DatagramPacket(encryptedPayload, encryptedPayload.length, rc.IPAddress(), rc.ReceivingPort());
        try{
            _udpSocket.send(toSend);
        }catch(Exception e){
            Main.LogError("UDPClient.Send(): " + e.getMessage());
        }
    }
}
