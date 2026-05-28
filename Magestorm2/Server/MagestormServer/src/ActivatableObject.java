public class ActivatableObject extends TimedObject{
    private byte _status;
    public ActivatableObject(byte key, int numSeconds)
    {
        _objectID = key;
        _status = 0;
        SetDurationRemaining(numSeconds * 1000);
    }
    public void ChangeState(byte newState)
    {
        _status = newState;
        if(_status > 0){
            ResetDuration();
        }
    }
    public byte GetStatus(){
        return _status;
    }

}
