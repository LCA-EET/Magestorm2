import java.util.ArrayList;

public class PacketSender extends Thread{
    private UDPClient _udp;
    private UDPProcessor _processor;

    public PacketSender(UDPClient udp, UDPProcessor processor){
        _udp = udp;
        _processor = processor;
        new Thread(this).start();
    }

    public void run(){
        while(!_processor.IsTerminated()){
            try {
                if(_processor.HasOutgoingPackets()){
                    ArrayList<OutgoingPacket> outgoing = _processor.OutgoingPackets();
                    for(OutgoingPacket packet : outgoing){
                        Iterable<RemoteClient> recipients = packet.Recipients();
                        for(RemoteClient rc : recipients){
                            _udp.Send(packet.Bytes(), rc);
                        }
                    }
                }
                Thread.sleep(GameServer.Tick);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }
}
