public class ActivatableObject extends TimedObject{
    private byte _status;
    private final long _duration;
    public ActivatableObject(byte key, int numSeconds)
    {
        _objectID = key;
        _status = 0;
        _duration = numSeconds * 1000;
    }
    public void ChangeState(byte newState)
    {
        _status = newState;
        if(_status > 0){
            SetDurationRemaining(_duration);
        }
    }
    public byte GetStatus(){
        return _status;
    }

}
