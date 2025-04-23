import java.nio.charset.StandardCharsets;

public class Match {
    private byte _numPlayers;
    private byte _matchID;
    private int _creatorID;
    private byte _sceneID;
    private byte _minutesLeft;
    private byte _secondsLeft;
    private long _creationTime;
    private long _expirationTime;
    private String _creator;
    private byte[] _matchBytes;
    private byte _lastIndex;

    public Match(byte matchID, int creatorID, String creator, byte sceneID, long creationTime){

        _matchID = matchID;
        _numPlayers = 0;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _creator = creator;
        _creationTime = creationTime;
        _expirationTime = creationTime + 3600000; // one hour
        Main.LogMessage("Initializing match " + _matchID + " with expiration time: " + _expirationTime);
        byte[] creatorNameBytes = creator.getBytes(StandardCharsets.UTF_8);
        byte nameBytesLength = (byte)creatorNameBytes.length;
        _matchBytes = new byte[1 + 1 + 8 + 4 + 1 +  nameBytesLength + 1];
        _lastIndex = (byte)(_matchBytes.length-1);
        int index = 0;
        _matchBytes[index] = matchID;
        index++;
        _matchBytes[index] = sceneID;
        index++;
        byte[] expirationBytes = ByteUtils.LongToByteArray(_expirationTime);
        System.arraycopy(expirationBytes, 0, _matchBytes, index, 8);
        index+=8;
        byte[] accountIDBytes = ByteUtils.IntToByteArray(creatorID);
        System.arraycopy(accountIDBytes, 0, _matchBytes, index, 4);
        index+=4;
        _matchBytes[index] = nameBytesLength;
        index++;
        System.arraycopy(creatorNameBytes, 0, _matchBytes, index, nameBytesLength);
    }
    public byte MatchID(){
        return _matchID;
    }
    public int CreatorAccountID(){
        return _creatorID;
    }
    public byte NumPlayersInMatch(){
        return _numPlayers;
    }
    public byte[] ToByteArray(){
        _matchBytes[_lastIndex] = _numPlayers;
        return _matchBytes;
    }
}
