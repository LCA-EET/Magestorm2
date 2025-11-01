public class ActivatableObject {
    private final byte _key;
    private byte _status;
    private final float _timeToHold;
    private float _timeRemaining;
    private final Match _owningMatch;

    public ActivatableObject(Match owner, byte key, int numSeconds)
    {
        _owningMatch = owner;
        _key = key;
        _status = 0;
        _timeToHold = numSeconds * 1000;
    }

    public void ChangeState(byte newState)
    {
        _status = newState;
        if(_timeRemaining > 0){ // the object was triggered prior to its normal expiration
            _timeRemaining = 0;
        }
        else{
            if(_timeToHold > 0 && newState > 0){
                _timeRemaining = _timeToHold;
            }
        }
    }
    public float TimeRemaining(){
        return _timeRemaining;
    }
    public byte GetStatus(){
        return _status;
    }
    public void Tick(float elapsed){
        _timeRemaining -= elapsed;
        if(_timeRemaining <= 0){
            ChangeState((byte)0); // revert to default state
            _owningMatch.SendToAll(Packets.ObjectStateChangePacket(_key, _status));
        }
    }
}
