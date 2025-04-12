import java.net.DatagramPacket;
import java.util.ArrayList;

public interface PacketProcessor {
    void ProcessPacket(DatagramPacket received);
    ArrayList<OutgoingPacket> OutgoingPackets();
    boolean HasOutgoingPackets();
}
