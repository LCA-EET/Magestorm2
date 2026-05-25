public class TimedObject {
    protected long _priorDuration, _durationRemaining;
    protected Number _objectID;
    protected byte[] _bytes;
    protected boolean _timedOut;

    public TimedObject(){ }

    public boolean ReduceDuration(long msReduction){
        _durationRemaining -= msReduction;
        CheckTimeOut();
        return _durationRemaining <= 0;
    }

    public long DurationRemaining(){
        return _durationRemaining;
    }

    public void SetDurationRemaining(long durationRemaining){
        _durationRemaining = durationRemaining;
        _priorDuration = _durationRemaining;
        CheckTimeOut();
    }
    public void CheckTimeOut(){
        if(_durationRemaining <= 0){
            _timedOut = true;
        }
    }
    public void ResetDuration(){
        if(!_timedOut){
            _durationRemaining = _priorDuration;
        }
    }

    public boolean DurationExpired(){
        return _timedOut || _durationRemaining <= 0;
    }

    public Number ObjectID(){
        return _objectID;
    }

    public byte[] GetBytes(){
        return _bytes;
    }
}
