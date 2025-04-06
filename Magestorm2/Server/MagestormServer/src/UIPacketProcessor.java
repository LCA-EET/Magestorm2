import java.net.DatagramPacket;

public class UIPacketProcessor implements PacketProcessor
{
    private UDPClient _udpClient;

    public UIPacketProcessor(){
        _udpClient = new UDPClient(6000, this);
        new Thread(_udpClient).start();
    }

    @Override
    public void ProcessPacket(DatagramPacket received) {
        byte[] receivedBytes = received.getData();
        Main.LogMessage("Received packet of length " + receivedBytes.length);
        byte[] decrypted = Cryptographer.Decrypt(receivedBytes);
        for(int i = 0; i < decrypted.length; i++){
            System.out.println(decrypted[i]);
        }
    }
}
