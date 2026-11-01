import java.util.ArrayList;
import java.util.concurrent.BlockingQueue;

public class PacketSender extends RegisteredThread{
    private final UDPClient _udp;
    private final UDPProcessor _processor;

    public PacketSender(UDPClient udp, UDPProcessor processor){
        _udp = udp;
        _processor = processor;
    }
    public void run(){
        Register("PacketSender, port " + _udp.GetLocalPort());
        BlockingQueue<OutgoingPacket> outgoing = _processor.OutgoingQueue();
        while(!_terminated){
            try {
                OutgoingPacket toProcess = outgoing.take();
                Iterable<RemoteClient> recipients = toProcess.Recipients();
                byte[] packetBytes = toProcess.Bytes();
                for(RemoteClient rc : recipients){
                    if(rc != null){
                        _udp.Send(packetBytes, rc);
                    }
                    else{
                        Main.LogError("Attempted to send to null RC associated with account ");
                    }
                }

            } catch (InterruptedException ie) {
                OnInterrupt();
            }
            catch (Exception e){
                Main.LogError("PacketSender.run(): " + e.getMessage());
            }
        }
        Main.LogMessage("No longer sending from port " + _processor._listeningPort);
        _udp.StopListening();
        Deregister();
    }
}
