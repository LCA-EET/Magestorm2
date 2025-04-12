import java.util.ArrayList;

public class PacketSender extends Thread{
    private UDPClient _udp;
    private PacketProcessor _processor;

    public PacketSender(UDPClient udp, PacketProcessor processor){
        _udp = udp;
        _processor = processor;
        new Thread(this).start();
    }

    public void run(){
        while(Main.Running){
            try {
                if(_processor.HasOutgoingPackets()){
                    ArrayList<OutgoingPacket> outgoing = _processor.OutgoingPackets();
                    for(OutgoingPacket packet : outgoing){
                        _udp.Send(packet.Bytes(), packet.Recipient());
                    }
                }
                Thread.sleep(GameServer.Tick);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }
}
