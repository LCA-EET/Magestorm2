public class ActivatableObject implements ITimedObject{
    private final byte _key;
    private byte _status;
    private final Match _owningMatch;
    private final Duration _timeToHold, _timeRemaining;

    public ActivatableObject(Match owner, byte key, int numSeconds)
    {
        _owningMatch = owner;
        _key = key;
        _status = 0;
        _timeToHold = new Duration(numSeconds * 1000);
        _timeRemaining = new Duration(_timeToHold.DurationRemaining());
    }

    public void ChangeState(byte newState)
    {
        _status = newState;
        if(!_timeRemaining.DurationExpired()){ // the object was triggered prior to its normal expiration
            _timeRemaining.SetDurationRemaining(0);
        }
        else{
            if(!_timeToHold.DurationExpired() && newState > 0){
                _timeRemaining.SetDurationRemaining(_timeToHold.DurationRemaining());
            }
        }
    }
    public byte GetStatus(){
        return _status;
    }
    public boolean ReduceDuration(long msReduction) {
        boolean expired = _timeRemaining.ReduceDuration(msReduction);
        if(expired){
            ChangeState((byte)0); // revert to default state
            _owningMatch.SendToAll(Packets.ObjectStateChangePacket(_key, _status));
        }
        return expired;
    }

    public boolean IsExpired() {
        return _timeRemaining.DurationExpired();
    }

    public short TimedObjectID() {
        return _key;
    }
}
