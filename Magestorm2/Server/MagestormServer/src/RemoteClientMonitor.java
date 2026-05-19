import java.util.ArrayList;
import java.util.List;

public class RemoteClientMonitor extends RegisteredThread{

    public RemoteClientMonitor(){
        new RegisteredThread(this).start();
    }
    public void run(){
        Register("RemoteClientMonitor");
        int tick = 30000;
        while(!_terminated){
            try {
                Thread.sleep(tick);
                if(GameServer.LoggedInClients.CountdownObjects(tick)){
                    List<RemoteClient> expiredClients = GameServer.LoggedInClients.GetExpiredObjects();
                    for(RemoteClient expiredClient : expiredClients){
                        Main.LogMessage("Client " + expiredClient.ObjectID() + " disconnected for inactivity.");
                        //GameServer.ClientLoggedOut(expiredClient.AccountID());
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
