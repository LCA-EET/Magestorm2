import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class GameServer extends Thread {
    public static final long TimeOut = 600000; // 10 minutes
    public static final long Tick = 10;
    private static ConcurrentHashMap<Integer, RemoteClient> _loggedInClients;
    private static RemoteClientMonitor _rcMonitor;
    private static PregamePacketProcessor _pgProcessor;

    public static void init(){
       _loggedInClients = new ConcurrentHashMap<Integer, RemoteClient>();
       _rcMonitor = new RemoteClientMonitor();
       _pgProcessor = new PregamePacketProcessor();
    }

    public static boolean IsLoggedIn(int accountID){
        return _loggedInClients.containsKey(accountID);
    }

    public static void ClientLoggedIn(RemoteClient rc)
    {
        _loggedInClients.put(rc.AccountID(), rc);
    }

    public static RemoteClient RemoveClient(int accountID){
        RemoteClient toReturn = null;
        if(_loggedInClients.containsKey(accountID)){
            toReturn = _loggedInClients.remove(accountID);
        }
        return toReturn;
    }

    public static ArrayList<RemoteClient> ConnectedClients(){
        return new ArrayList<>(_loggedInClients.values());
    }
    private void CheckClientStatus(){

    }
    public void ClientTimeOut(RemoteClient rc){

    }
}
