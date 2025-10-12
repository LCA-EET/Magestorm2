public class Flag {
    private final byte _teamID;
    private byte _holderID;
    private final Vector3 _location;

    public Flag(byte teamID){
        _teamID = teamID;
        _holderID = -1;
        _location = new Vector3();
    }

    public boolean IsHeld(){
        return _holderID >= 0;
    }

    public byte[] GetLocation(){
        return _location.GetPosition();
    }

    public void FlagTaken(byte takerID){
        _holderID = takerID;
    }

    public void FlagCaptured(){
        _holderID = -1;
    }

    public void FlagDropped(byte[] positionBytes){
        _location.SetCoordinates(positionBytes, 0);
    }


}
