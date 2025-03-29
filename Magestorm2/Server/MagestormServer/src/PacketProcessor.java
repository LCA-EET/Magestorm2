import java.net.DatagramPacket;

public interface PacketProcessor {
    public void ProcessPacket(DatagramPacket received);
}
