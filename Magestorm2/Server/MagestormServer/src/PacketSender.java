import java.util.ArrayList;
import java.util.concurrent.BlockingQueue;

public class PacketSender extends RegisteredThread{
    private final UDPClient _udp;
    private final UDPProcessor _processor;
    private BlockingQueue<OutgoingPacket> _outgoing;

    public PacketSender(UDPClient udp, UDPProcessor processor){
        _udp = udp;
        _processor = processor;
    }
    private void ProcessTypeA(){
        try {
            OutgoingPacket toProcess = _outgoing.take();
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

        } catch (InterruptedException e) {
            _terminated = true;
        }
    }
    private void ProcessTypeB(){
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
                            Main.LogError("Attempted to send to null RC associated with account ");
                        }
                    }
                }
            }
            Thread.sleep(ServerParams.TickInterval);
        } catch (InterruptedException e) {
            Main.LogStackTrace(e);
        }
    }
    public void run(){
        Register("PacketSender, port " + _udp.GetLocalPort());
        _outgoing = _processor.OutgoingQueue();
        while(!_terminated){
            try {
                OutgoingPacket toProcess = _outgoing.take();
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

            } catch (InterruptedException e) {
                _terminated = true;
            }
        }
        Main.LogMessage("No longer sending from port " + _processor._listeningPort);
        _udp.StopListening();
        Deregister();
    }
}
