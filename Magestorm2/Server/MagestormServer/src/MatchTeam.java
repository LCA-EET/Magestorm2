import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class MatchTeam {
    public static byte Neutral = 0;
    public static byte Balance = 1;
    public static byte Chaos = 2;
    public static byte Order = 3;
    public static byte[] TeamCodes = new byte[]{Neutral, Balance, Chaos, Order};


    private boolean _listChanged;
    private byte _teamID;
    private byte[] _playerListBytes;
    private final ConcurrentHashMap<Byte, MatchCharacter> _teamPlayers;

    public MatchTeam(byte teamID)
    {
        _teamID = teamID;
        _listChanged = true;
        _teamPlayers = new ConcurrentHashMap<>();
    }

    public boolean PlayerIDUsed(byte idToCheck){
        return _teamPlayers.containsKey(idToCheck);
    }

    public void AddPlayer(byte id, MatchCharacter toAdd){
        _teamPlayers.put(id, toAdd);
        _listChanged = true;
    }

    public void RemovePlayer(byte idToRemove){
        _teamPlayers.remove(idToRemove);
        _listChanged = true;
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
}
