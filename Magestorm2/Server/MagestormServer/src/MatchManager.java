import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class MatchManager{
    private static ConcurrentHashMap<Byte, Match> _activeMatches;
    private static byte _nextMatchID = 1;
    private static byte _maxMatches;
    private static ConcurrentHashMap<Byte, byte[]> _scores;
    public static boolean UpdatesNeeded;

    public static void init(byte maxMatches){
        UpdatesNeeded = false;
        _scores = new ConcurrentHashMap<>();
        _maxMatches = maxMatches;
        _activeMatches = new ConcurrentHashMap<>();
        new MatchMonitor();
    }
    public static void UpdateScore(byte matchID, byte[] scoreBytes){
        _scores.put(matchID, scoreBytes);
    }
    public static byte[] GetScoreBytes(byte matchID){
        return _scores.get(matchID);
    }

    public static void SendMatchListToClient(RemoteClient rc){
        GameServer.EnqueueForSend(Packets.MatchDataPacket(_activeMatches.values()), rc);
        int cid = rc.GetDepartedCharacterID();
        if(cid > 0){
            PlayerCharacter pc = CharacterManager.GetCharacter(cid);
            if(pc != null){
                GameServer.EnqueueForSend(Packets.ExpLevelUpdatePacket(pc), rc);
            }
        }
    }

    public static void Subscribe(int accountID, boolean subscribe, int charID, RemoteClient remote){
        Main.LogMessage("MatchManager.Subscribe: " + charID +", " + subscribe);
        RemoteClient rc = GameServer.GetClient(accountID);

        if(rc != null){
            if(subscribe){
                if(rc.PortSwitchPending()){
                    Main.LogMessage("RemoteClient port switch from " + rc.GetRemotePort() + " to " + remote.GetRemotePort() + ", for account: " + remote.AccountID());
                    remote.SetNameAndID(rc.GetUserName(), rc.AccountID());
                    rc = remote;
                    GameServer.ClientLoggedIn(remote);
                }
                rc.SubscribeToMatches();
                GameServer.AddActiveCharacter(accountID, CharacterManager.GetCharacter(charID));
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
    public static void RequestMatchCreation(int accountID, byte sceneID, byte duration, byte matchType, byte matchOptions){
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
                    Main.LogMessage("Attempting to create match " + matchID + ", scene " + sceneID + "...");
                    Match newlyCreated = null;
                    PlayerCharacter activeCharacter = GameServer.GetActiveCharacter(accountID);
                    switch(matchType){
                        case MatchType.DeathMatch:
                            newlyCreated = new DeathMatch(matchID, accountID, activeCharacter.GetNameBytes(),
                                    sceneID, System.currentTimeMillis(), duration, matchOptions);
                            break;
                        case MatchType.FreeForAll:
                            newlyCreated = new FreeForAll(matchID, accountID, activeCharacter.GetNameBytes(),
                                    sceneID, System.currentTimeMillis(), duration, matchOptions);
                            break;
                        case MatchType.CaptureTheFlag:
                            newlyCreated = new CaptureTheFlag(matchID, accountID, activeCharacter.GetNameBytes(),
                                    sceneID, System.currentTimeMillis(), duration, matchOptions);
                            break;
                    }
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
        Match removed = _activeMatches.remove(matchID);
        GameServer.RemoveUsedPort(removed.GetMatchPort());
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
