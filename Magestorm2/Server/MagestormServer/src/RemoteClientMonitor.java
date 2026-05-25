import java.util.List;

public class RemoteClientMonitor extends RegisteredThread{

    public RemoteClientMonitor(){
        new RegisteredThread(this).start();
    }
    public void run(){
        Register("RemoteClientMonitor");
        TimedObjectCollection<Integer, RemoteClient> activeClients = RemoteClientManager.PregameClients();
        int tick = 1000;
        while(!_terminated){
            try {
                Thread.sleep(tick);
                if(activeClients.CountdownObjects(tick)){
                    List<RemoteClient> expiredClients = activeClients.GetExpiredObjects();
                    for(RemoteClient expiredClient : expiredClients){
                        Main.LogMessage("Client " + expiredClient.ObjectID() + " disconnected for inactivity.");
                        GameServer.EnqueueForSend(Packets.PGInactivityDisconnectPacket(), expiredClient);
                    }
                }
            } catch (Exception e) {
                Main.LogError(e.getMessage());
                Main.LogStackTrace(e);
            }
        }
        Deregister();
    }
}
