public class ActivatableObject {
    private byte _objectID, _status;
    public ActivatableObject(byte key)
    {
        _objectID = key;
        _status = 0;
    }
    public void ChangeState(byte newState)
    {
        _status = newState;
    }
    public byte GetStatus(){
        return _status;
    }

}
