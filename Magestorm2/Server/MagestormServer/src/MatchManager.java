import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class MatchManager extends Thread{
    private ConcurrentHashMap<Byte, Match> _activeMatches;
    private boolean _updatesNeeded;
    private byte _nextMatchID = 1;
    private final byte _maxMatches = 10;

    public MatchManager(){
        _updatesNeeded = false;
        _activeMatches = new ConcurrentHashMap<Byte, Match>();
        new Thread(this).start();
    }

    public void Subscribe(int accountID, boolean subscribe, String characterName){
        RemoteClient rc = GameServer.GetClient(accountID);
        if(rc != null){
            rc.SubscribeToMatches(subscribe, characterName);
            GameServer.EnqueueForSend(Packets.MatchDataPacket(_activeMatches.values()), rc);
        }
    }
    private void NotifySubscribers(){
        ArrayList<RemoteClient> connectedClients = GameServer.ConnectedClients();
        ArrayList<RemoteClient> subscribedClients = new ArrayList<>();
        for (RemoteClient rc : connectedClients){
            if(rc.IsSubscribedToMatches()){
                subscribedClients.add(rc);
            }
        }
        RemoteClient[] recipients = subscribedClients.toArray(new RemoteClient[]{});
        GameServer.EnqueueForSend(Packets.MatchDataPacket(_activeMatches.values()), recipients);
        _updatesNeeded = false;
    }
    public void RequestMatchCreation(RemoteClient rc, int accountID, byte sceneID){
        if(CheckOtherMatchesCreatedByAccount(accountID)){
            GameServer.EnqueueForSend(Packets.MatchAlreadyCreatedPacket(), rc);
        }
        else{
            if(_activeMatches.size() >= _maxMatches){
                GameServer.EnqueueForSend(Packets.MatchLimitReachedPacket(), rc);
            }
            else{
                byte matchID = NextMatchID();
                Match newlyCreated = new Match(matchID, accountID, rc.SelectedCharacterName(), sceneID, System.currentTimeMillis());
                AddMatch(matchID, newlyCreated);
            }
        }
    }
    private void AddMatch(byte matchID, Match newlyCreated){
        _activeMatches.put(matchID, newlyCreated);
        _updatesNeeded = true;
    }
    private boolean CheckOtherMatchesCreatedByAccount(int accountID){
        for(Match match : _activeMatches.values()){
            if(match.CreatorAccountID() == accountID){
                return true;
            }
        }
        return false;
    }
    public void run(){
        while(Main.Running){
            try {
                if(_updatesNeeded){
                    NotifySubscribers();
                }
                Thread.sleep(2000);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }
    private byte NextMatchID(){
        byte toReturn = _nextMatchID;
        _nextMatchID++;
        if(_nextMatchID > 100){
            _nextMatchID = 1;
        }
        return toReturn;
    }
}
