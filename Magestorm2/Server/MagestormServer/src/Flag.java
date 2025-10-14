public class Flag {
    private final byte[] _flagBytes;
    private final byte _idxTeam = 0;
    private final byte _idxHolder = 1;
    public static final byte NOT_HELD = 0;
    public static final byte DROPPED = 101;

    public Flag(byte teamID){
        _flagBytes = new byte[14];
        _flagBytes[_idxTeam] = teamID;
    }

    public boolean IsHeld(){
        return _flagBytes[_idxHolder] > 0;
    }

    public void FlagTaken(byte takerID){
        _flagBytes[_idxHolder] = takerID;
    }

    public void FlagReturned(){
        _flagBytes[_idxHolder] = NOT_HELD;
    }

    public void FlagDropped(byte[] positionBytes){
        byte formerHolder = _flagBytes[_idxHolder];
        _flagBytes[_idxHolder] = DROPPED;
        System.arraycopy(positionBytes,0, _flagBytes, 2, 12);
    }

    public byte[] GetFlagBytes(){
        return _flagBytes;
    }

}
