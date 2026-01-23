import java.util.ArrayList;

public class RemoteClientMonitor extends Thread{

    public RemoteClientMonitor(){
        new Thread(this).start();
    }
    public void run(){
        while(Main.Running){
            try {
                Iterable<RemoteClient> clients = GameServer.ConnectedClients();
                for(RemoteClient client : clients){
                    if(client.TimeOut()){
                        int accountID = client.AccountID();
                        GameServer.ClientLoggedOut(accountID);
                        Main.LogMessage("Client " + accountID + " disconnected for inactivity.");
                        GameServer.EnqueueForSend(Packets.InactivityDisconnectPacket(), client);
                    }
                }
                Thread.sleep(30000);
            } catch (Exception e) {
                Main.LogError(e.getMessage());
                Main.LogStackTrace(e);
            }
        }
    }
}
