import java.util.ArrayList;
import java.util.Collection;
import java.util.concurrent.ConcurrentHashMap;

public class MatchTeam {
    public static byte Neutral = 0;
    public static byte Chaos = 1;
    public static byte Balance = 2;
    public static byte Order = 3;
    public static byte[] TeamCodes = new byte[]{Neutral, Chaos, Balance, Order};
    public static byte[] TeamCodes_NonNeutral = new byte[]{Chaos, Balance, Order};
    private boolean _listChanged;
    private final byte _teamID;

    private byte[] _playerListBytes;
    private final ConcurrentHashMap<Byte, MatchCharacter> _teamPlayers;
    private final ConcurrentHashMap<Byte, RemoteClient> _clients;
    private final Match _owningMatch;
    private int _totalLevel;

    public MatchTeam(byte teamID, Match owningMatch)
    {
        _totalLevel = 0;
        _teamID = teamID;
        _listChanged = true;
        _teamPlayers = new ConcurrentHashMap<>();
        _owningMatch = owningMatch;
        _clients = new ConcurrentHashMap<>();
    }

    public boolean PlayerIDUsed(byte idToCheck){
        return _teamPlayers.containsKey(idToCheck);
    }

    public void AddPlayer(byte id, MatchCharacter toAdd){
        _teamPlayers.put(id, toAdd);
        _totalLevel += toAdd.GetLevel();
        _listChanged = true;
    }

    public void RemovePlayer(byte idToRemove){
        MatchCharacter mc = _teamPlayers.remove(idToRemove);
        _totalLevel -= mc.GetLevel();
        _clients.remove(idToRemove);
        _listChanged = true;
    }

    public Collection<MatchCharacter> GetPlayers(){
        return _teamPlayers.values();
    }

    public byte NumPlayers(){
        return (byte)_teamPlayers.size();
    }

    public byte[] GetPlayerBytes(){
        if(_listChanged){
            _playerListBytes = RefreshPlayerBytes();
            _listChanged = false;
        }
        return _playerListBytes;
    }

    private byte[] RefreshPlayerBytes(){
        int length = 1;
        ArrayList<byte[]> players = new ArrayList<>();
        for(MatchCharacter player : _teamPlayers.values()){
            byte[] playerBytes = player.GetINLCTABytes();
            players.add(playerBytes);
            length += playerBytes.length;
        }
        byte[] toReturn = ByteUtils.ArrayListToByteArray(players, length, 1);
        toReturn[0] = (byte)players.size();
        return toReturn;
    }
    public int GetTotalLevel(){
        return _totalLevel;
    }
    public Collection<RemoteClient> GetRemoteClients(){
        return _clients.values();
    }
    public void RegisterVerifiedClient(byte id, RemoteClient verified){
        _clients.put(id, verified);
    }
}
