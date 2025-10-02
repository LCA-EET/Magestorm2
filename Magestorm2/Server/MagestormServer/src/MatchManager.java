import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class MatchManager{
    private static ConcurrentHashMap<Byte, Match> _activeMatches;
    private static byte _nextMatchID = 1;
    private static final byte _maxMatches = 10;
    private static MatchMonitor _monitor;
    public static boolean UpdatesNeeded;

    public static void init(){
        UpdatesNeeded = false;
        _activeMatches = new ConcurrentHashMap();
        _monitor = new MatchMonitor();
    }

    public static void SendMatchListToClient(RemoteClient rc){
        GameServer.EnqueueForSend(Packets.MatchDataPacket(_activeMatches.values()), rc);
    }

    public static void Subscribe(int accountID, boolean subscribe, int charID){
        Main.LogMessage("MatchManager.Subscribe: " + charID +", " + subscribe);
        RemoteClient rc = GameServer.GetClient(accountID);
        if(rc != null){
            if(subscribe){
                rc.SubscribeToMatches(charID);
                GameServer.EnqueueForSend(Packets.AcknowledgeSubscriptionPacket(), rc);
            }
            else{
                rc.UnsubscribeFromMatches();
            }

        }
        else{
            Main.LogMessage("MatchManager.Subscribe: rc is null for account id: " + accountID);
        }
    }
    public static void NotifySubscribers(){
        Iterable<RemoteClient> connectedClients = GameServer.ConnectedClients();
        ArrayList<RemoteClient> subscribedClients = new ArrayList<>();
        for (RemoteClient rc : connectedClients) {
            if (rc.IsSubscribedToMatches()) {
                subscribedClients.add(rc);
            }
        }
        GameServer.EnqueueForSend(Packets.MatchDataPacket(_activeMatches.values()), subscribedClients);
        UpdatesNeeded = false;
    }
    public static void RequestMatchCreation(int accountID, byte sceneID, byte duration, byte matchType){
        RemoteClient rc = GameServer.GetClient(accountID);
        if(rc != null){
            if(CheckOtherMatchesCreatedByAccount(accountID)){
                GameServer.EnqueueForSend(Packets.MatchAlreadyCreatedPacket(), rc);
            }
            else{
                if(_activeMatches.size() >= _maxMatches){
                    GameServer.EnqueueForSend(Packets.MatchLimitReachedPacket(), rc);
                }
                else{
                    byte matchID = NextMatchID();
                    Main.LogMessage("Attempting to create match " + matchID + "...");
                    Match newlyCreated = new Match(matchID, accountID, rc.GetActiveCharacter().GetNameBytes(),
                            sceneID, System.currentTimeMillis(), duration, matchType);
                    Main.LogMessage("Match " + matchID + " created by account " + accountID );
                    AddMatch(matchID, newlyCreated);
                }
            }
        }
    }
    public static void DeleteMatch(int accountID, RemoteClient rc){
        Match toDelete = null;
        for(Match match : _activeMatches.values()){
            if(match.CreatorAccountID() == accountID){
                if(match.NumPlayersInMatch() == 0){
                    toDelete = match;
                }
                else{
                    GameServer.EnqueueForSend(Packets.MatchStillHasPlayersPacket(), rc);
                }
            }
        }
        if(toDelete != null){
            _activeMatches.remove(toDelete.MatchID());
            UpdatesNeeded = true;
        }
    }
    private static void AddMatch(byte matchID, Match newlyCreated){
        _activeMatches.put(matchID, newlyCreated);
        UpdatesNeeded = true;
    }
    private static boolean CheckOtherMatchesCreatedByAccount(int accountID){
        for(Match match : _activeMatches.values()){
            if(match.CreatorAccountID() == accountID){
                return true;
            }
        }
        return false;
    }

    private static byte NextMatchID(){
        byte toReturn = _nextMatchID;
        _nextMatchID++;
        if(_nextMatchID > 100){
            _nextMatchID = 1;
        }
        return toReturn;
    }

    public static Match GetMatch(byte matchID){
        if(_activeMatches.containsKey(matchID)){
            return _activeMatches.get(matchID);
        }
        return null;
    }

    public static void RemoveMatch(byte matchID){
        _activeMatches.remove(matchID);
        UpdatesNeeded = true;
    }

    public static ArrayList<Match> GetMatches(){
        ArrayList<Match> toReturn = new ArrayList<>();
        for(Match match : _activeMatches.values()){
            toReturn.add(match);
        }
        return toReturn;
    }
}
