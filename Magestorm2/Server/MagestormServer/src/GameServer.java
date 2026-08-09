import java.sql.Connection;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentSkipListSet;

public class GameServer extends Thread {
    public static final boolean SymmetricEncryption = false;
    public static int MaxUDPPayload = 508;
    private static ConcurrentSkipListSet<Integer> _usedMatchPorts;
    private static ConcurrentHashMap<Byte, Byte> _maxPlayerData;
    private static ConcurrentHashMap<Integer, PlayerCharacter> _activeCharacters;
    private static ConcurrentHashMap<Byte, byte[]> _objectData;
    private static PregamePacketProcessor _pgProcessor;
    private static byte[] _levelData;

    public static void init(){
        ByteUtils.init();
        GameUtils.init();
        CharacterManager.init();
        _maxPlayerData = new ConcurrentHashMap<>();
        _activeCharacters = new ConcurrentHashMap<>();
        _objectData = new ConcurrentHashMap<>();
        try(Connection conn = Database.DBConnection()){
            CharacterClassManager.init(conn);
            DisciplineManager.init(conn);
            EffectManager.init(conn);
            SpellManager.init(conn);
        }
        catch(Exception ex){
            Main.LogError("GameServer.init(): " + ex.getMessage());
            Main.LogStackTrace(ex);
            TerminateServer();
            return;
        }
        RemoteClientManager.init();
        MatchManager.init(ServerParams.MaxMatches);
        _pgProcessor = new PregamePacketProcessor(ServerParams.ListeningPort);
        _levelData = Database.GetLevelsList((byte)1);
        _usedMatchPorts = new ConcurrentSkipListSet<>();
        if(ServerParams.QMEnabled == 1){
            MatchManager.AddQuickMatch();
        }
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
    public static void SetActivatables(byte sceneID, byte[] aoBytes){
        _objectData.put(sceneID, aoBytes);
    }
    public static byte[] GetActivatablesData(byte sceneID){
        return _objectData.get(sceneID);
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
    public static void RemoveUsedPort(int portNumber){
        _usedMatchPorts.remove(portNumber);
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
    public static void TerminateServer(){
        Main.Running = false;
        Main.LogMessage("Server is shutting down.");
        Main.ThreadMonitor.InterruptAllThreads();
        System.exit(0);
    }
}
