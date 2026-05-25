import java.util.Collection;

public class RemoteClientManager {
    private static TimedObjectCollection<Integer, RemoteClient> _pregameClients;

    public static void init(){
        _pregameClients = new TimedObjectCollection<>(1000);
        new RemoteClientMonitor();
    }
    public static boolean IsLoggedIn(int accountID){
        return _pregameClients.containsKey(accountID);
    }

    public static Collection<RemoteClient> GetConnectedClients(){
        return _pregameClients.values();
    }

    public static TimedObjectCollection<Integer, RemoteClient> PregameClients(){
        return _pregameClients;
    }

    public static void ClientLoggedIn(RemoteClient rc)
    {
        int accountID = (int)rc.ObjectID();
        Main.LogMessage("Client logged in: " + accountID + rc.IPAddress().toString() + ":" + rc.GetRemotePort());
        _pregameClients.put(accountID, rc);
    }

    public static RemoteClient GetClient(int accountID){
        RemoteClient toReturn = null;
        if(_pregameClients.containsKey(accountID)){
            toReturn = _pregameClients.get(accountID);
        }
        else{
            Main.LogMessage("RemoteClient is null for account " + accountID);
        }
        return toReturn;
    }

    public static RemoteClient ClientLoggedOut(int accountID){
        Main.LogMessage("Client logged out: " + accountID);
        RemoteClient toRemove = _pregameClients.get(accountID);
        if(toRemove != null){
            toRemove.SetDurationRemaining(0);
        }
        return toRemove;
    }

}
