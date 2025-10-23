public class ActivatableObject {
    private byte _key;
    private byte _status;
    private float _timeRemaining;

    public ActivatableObject(byte key, byte newStatus)
    {
        _key = key;
        _status = newStatus;
    }

    public ActivatableObject(byte key, byte newStatus, byte numSeconds)
    {
        this(key, newStatus);
        _timeRemaining = numSeconds * 1000;
    }

    public boolean IsTimed(){
        return _timeRemaining > 0;
    }

    public void ChangeState(byte newState)
    {
        _status = newState;
    }
    public float TimeRemaining(){
        return _timeRemaining;
    }
    public void SetTimeRemaining(float timeRemaining){
        _timeRemaining = timeRemaining;
    }
    public byte GetStatus(){
        return _status;
    }
}
