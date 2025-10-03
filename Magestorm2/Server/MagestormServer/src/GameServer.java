import java.rmi.Remote;
import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;
import java.util.concurrent.ConcurrentSkipListSet;

public class GameServer extends Thread {
    public static final long TimeOut = 600000; // 10 minutes
    public static final long Tick = 10;
    private static int _nextMatchPort;
    private static ConcurrentSkipListSet<Integer> _usedMatchPorts;
    public static ConcurrentHashMap<Integer, RemoteClient> _loggedInClients;
    private static ConcurrentHashMap<Byte, Byte> _maxPlayerData;
    private static ConcurrentHashMap<Integer, PlayerCharacter> _activeCharacters;
    private static ConcurrentHashMap<Byte, String> _poolData;
    private static RemoteClientMonitor _rcMonitor;
    private static PregamePacketProcessor _pgProcessor;
    private static byte[] _levelData;
    public static void init(){
       ByteUtils.init();
       GameUtils.init();
       CharacterManager.init();
        _loggedInClients = new ConcurrentHashMap<>();
        _maxPlayerData = new ConcurrentHashMap<>();
        _activeCharacters = new ConcurrentHashMap<>();
        _poolData = new ConcurrentHashMap<>();
       MatchManager.init();
       _rcMonitor = new RemoteClientMonitor();
       _pgProcessor = new PregamePacketProcessor(ServerParams.ListeningPort);
       _levelData = Database.GetLevelsList((byte)1);
       _usedMatchPorts = new ConcurrentSkipListSet<>();
    }
    
    public static void AddActiveCharacter(int accountID, PlayerCharacter active){
        _activeCharacters.put(accountID, active);
    }
    public static PlayerCharacter GetActiveCharacter(int accountID){
        return _activeCharacters.get(accountID);
    }
    public static PlayerCharacter RemoveActiveCharacter(int accountID){
        return _activeCharacters.remove(accountID);
    }
    public static void SetPoolData(byte sceneID, String poolString){
        _poolData.put(sceneID, poolString);
    }
    public static String GetPoolData(byte sceneID){
        return _poolData.get(sceneID);
    }

    public static boolean IsLoggedIn(int accountID){
        return _loggedInClients.containsKey(accountID);
    }

    public static int GetNextMatchPort()
    {
        int nextAvailablePort = ServerParams.ListeningPort + 1;
        while(_usedMatchPorts.contains(nextAvailablePort)){
            nextAvailablePort++;
        }
        _usedMatchPorts.add(nextAvailablePort);
        return nextAvailablePort;
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
            Main.LogMessage("RemoteClient returned for account " + accountID);
        }
        else{
            Main.LogMessage("RemoteClient is null for account " + accountID);
        }
        return toReturn;
    }

    public static Iterable<RemoteClient> ConnectedClients(){
        return _loggedInClients.values();
    }
    private void CheckClientStatus(){

    }
    public void ClientTimeOut(RemoteClient rc){

    }

    public static RemoteClient ClientLoggedOut(int accountID){
        Main.LogMessage("Client logged out: " + accountID);
        RemoteClient removed = _loggedInClients.remove(accountID);
        PlayerCharacter removedCharacter = _activeCharacters.remove(accountID);
        if(removedCharacter != null){
            byte idInMatch = removedCharacter.GetIDinMatch();
            byte matchID = removedCharacter.GetMatchID();
            byte teamID = removedCharacter.GetCurrentTeam();
            Match match = MatchManager.GetMatch(matchID);
            if(match != null){
                if(match.IsPlayerOnTeam(idInMatch, teamID)){
                    match.LeaveMatch(idInMatch, teamID, true);

                }
            }
        }
        return removed;
    }
    public static void EnqueueForSend(byte[] encrypted, RemoteClient recipient){
        _pgProcessor.EnqueueForSend(encrypted, recipient);
    }
    public static void EnqueueForSend(byte[] encrypted, Iterable<RemoteClient> recipients){
        _pgProcessor.EnqueueForSend(encrypted, recipients);
    }

    public static byte[] LevelList(){
        return _levelData;
    }
    public static void RecordMaxPlayerData(byte sceneID, byte maxPlayers){
        _maxPlayerData.put(sceneID, maxPlayers);
    }
    public static byte RetrieveMaxPlayerData(byte sceneID){
        return _maxPlayerData.get(sceneID);
    }
}
