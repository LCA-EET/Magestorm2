import java.util.ArrayList;

public class RemoteClientMonitor extends Thread{

    public RemoteClientMonitor(){
        new Thread(this).start();
    }
    public void run(){
        while(Main.Running){
            try {
                ArrayList<RemoteClient> clients = GameServer.ConnectedClients();
                for(RemoteClient client : clients){
                    if(client.TimeOut()){

                    }
                }
                Thread.sleep(60000);
            } catch (Exception e) {
                throw new RuntimeException(e);
            }
        }
    }
}
