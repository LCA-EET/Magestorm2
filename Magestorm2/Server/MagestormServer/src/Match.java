import java.nio.charset.StandardCharsets;

public class Match {
    private byte _numPlayers;
    private byte _matchID;
    private int _creatorID;
    private byte _sceneID;
    private byte _minutesLeft;
    private byte _secondsLeft;
    private long _creationTime;
    private String _creator;
    private byte[] _matchBytes;

    public Match(byte matchID, int creatorID, String creator, byte sceneID, long creationTime){
        _matchID = matchID;
        _numPlayers = 0;
        _creatorID = creatorID;
        _sceneID = sceneID;
        _creator = creator;
        _creationTime = creationTime;
        byte[] creatorNameBytes = creator.getBytes(StandardCharsets.UTF_8);
        byte nameBytesLength = (byte)creatorNameBytes.length;
        _matchBytes = new byte[1 + 1 + 1 + nameBytesLength  + 1 + 1 + 1];
        _matchBytes[0] = matchID;
        _matchBytes[1] = sceneID;
        _matchBytes[2] = nameBytesLength;
        System.arraycopy(creatorNameBytes, 0, _matchBytes, 3, nameBytesLength);
    }
    public int CreatorAccountID(){
        return _creatorID;
    }
    public byte[] ToByteArray(){
        int length = _matchBytes.length;
        _matchBytes[length - 3] = _numPlayers;
        _matchBytes[length - 2] = _minutesLeft;
        _matchBytes[length - 1] = _secondsLeft;
        return _matchBytes;
    }
}
