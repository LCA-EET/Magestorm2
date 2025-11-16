public class Flag {
    private final byte[] _flagBytes;
    private final byte _idxTeam = 0;
    private final byte _idxHolder = 1;
    private final long _droppedTime = 30000; // 30K ms = 30 seconds
    private long _droppedTimeRemaining;
    public static final byte NOT_HELD = 0;
    public static final byte DROPPED = 101;

    public Flag(byte teamID){
        _flagBytes = new byte[14];
        _flagBytes[_idxTeam] = teamID;
    }

    public boolean IsHeld(){
        byte holder = _flagBytes[_idxHolder];
        return holder > 0 && holder < DROPPED;
    }

    public boolean IsDropped(){
        return _flagBytes[_idxHolder] == DROPPED;
    }

    public byte HeldBy(){
        return _flagBytes[_idxHolder];
    }

    public void FlagTaken(byte takerID){
        _flagBytes[_idxHolder] = takerID;
        _droppedTimeRemaining = _droppedTime;
    }

    public void FlagReturned(){
        _flagBytes[_idxHolder] = NOT_HELD;
        _droppedTimeRemaining = _droppedTime;
    }

    public void FlagDropped(byte[] position){
        _flagBytes[_idxHolder] = DROPPED;
        System.arraycopy(position,0, _flagBytes, 2, 12);
        _droppedTimeRemaining = _droppedTime;
        /*
        float x = ByteUtils.ExtractFloat(position, 0);
        float y = ByteUtils.ExtractFloat(position, 4);
        float z = ByteUtils.ExtractFloat(position, 8);

        Main.LogMessage("Flag dropped at position " + x + ", " + y + ", " + z);
        */
    }

    public boolean DropCountdown(long msElapsed){
        _droppedTimeRemaining -= msElapsed;
        return _droppedTimeRemaining <= 0;
    }

    public byte[] GetFlagBytes(){
        return _flagBytes;
    }

    public byte GetHolderID(){
        return _flagBytes[_idxHolder];
    }

    public byte GetTeamID(){
        return _flagBytes[_idxTeam];
    }
}
