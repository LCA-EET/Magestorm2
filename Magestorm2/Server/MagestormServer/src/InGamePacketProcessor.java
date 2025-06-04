import java.net.DatagramPacket;
import java.util.concurrent.ConcurrentLinkedQueue;

public class InGamePacketProcessor extends UDPProcessor{

    public InGamePacketProcessor(int port){
        super(port);
    }

    public void ProcessPacket(DatagramPacket received){

    }
}
