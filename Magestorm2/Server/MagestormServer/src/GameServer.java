import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class GameServer extends Thread {
    public static final long TimeOut = 600000; // 10 minutes
    public static final long Tick = 10;
    private static ConcurrentHashMap<Integer, RemoteClient> _loggedInClients;
    private static RemoteClientMonitor _rcMonitor;
    private static PregamePacketProcessor _pgProcessor;
    private static byte[] _levelData;
    public static void init(){
       ByteUtils.init();
       GameUtils.init();
       CharacterManager.init();
        _loggedInClients = new ConcurrentHashMap<Integer, RemoteClient>();
       MatchManager.init();
       _rcMonitor = new RemoteClientMonitor();
       _pgProcessor = new PregamePacketProcessor();
       _levelData = Database.GetLevelsList((byte)1);
    }

    public static boolean IsLoggedIn(int accountID){
        return _loggedInClients.containsKey(accountID);
    }

    public static boolean IsLoggedIn(byte[] decrypted){
        return _loggedInClients.containsKey(ByteUtils.ExtractInt(decrypted,0));
    }

    public static void ClientLoggedIn(RemoteClient rc)
    {
        int accountID = rc.AccountID();
        Main.LogMessage("Client logged in: " + accountID);
        _loggedInClients.put(accountID, rc);
    }

    public static RemoteClient RemoveClient(int accountID){
        RemoteClient toReturn = null;
        if(_loggedInClients.containsKey(accountID)){
            toReturn = _loggedInClients.remove(accountID);
        }
        return toReturn;
    }

    public static RemoteClient GetClient(int accountID){
        RemoteClient toReturn = null;
        if(_loggedInClients.containsKey(accountID)){
            toReturn = _loggedInClients.get(accountID);
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

    public static void ClientLoggedOut(int accountID){
        Main.LogMessage("Client logged out: " + accountID);
        _loggedInClients.remove(accountID);
    }
    public static void EnqueueForSend(byte[] encrypted, RemoteClient recipient){
        _pgProcessor.EnqueueForSend(encrypted, recipient);
    }
    public static void EnqueueForSend(byte[] encrypted, RemoteClient[] recipients){
        _pgProcessor.EnqueueForSend(encrypted, recipients);
    }

    public static byte[] LevelList(){
        return _levelData;
    }
}
