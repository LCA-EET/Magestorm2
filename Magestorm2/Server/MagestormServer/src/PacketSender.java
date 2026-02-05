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
        while(!_processor.IsTerminated() || _processor.HasOutgoingPackets()){
            try {
                if(_processor.HasOutgoingPackets()){
                    ArrayList<OutgoingPacket> outgoing = _processor.OutgoingPackets();
                    for(OutgoingPacket packet : outgoing){
                        Iterable<RemoteClient> recipients = packet.Recipients();
                        byte[] packetBytes = packet.Bytes();
                        for(RemoteClient rc : recipients){
                            if(rc != null){
                                _udp.Send(packetBytes, rc);
                            }
                            else{
                                Main.LogError("Attempted to send to null RC associated with account " + rc.AccountID());
                            }
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
