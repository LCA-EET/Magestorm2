public class TimedObject {
    protected long _priorDuration, _durationRemaining;
    protected Number _objectID;

    public TimedObject(){ }

    public boolean ReduceDuration(long msReduction){
        _durationRemaining -= msReduction;
        Main.LogDebug("Duration remaining for object " + _objectID + ": " + _durationRemaining);
        return _durationRemaining <= 0;
    }

    public long DurationRemaining(){
        return _durationRemaining;
    }

    public void SetDurationRemaining(long durationRemaining){
        _durationRemaining = durationRemaining;
        _priorDuration = _durationRemaining;
    }

    public void ResetDuration(){
        _durationRemaining = _priorDuration;
    }

    public boolean DurationExpired(){
        return _durationRemaining <= 0;
    }

    public Number ObjectID(){
        return _objectID;
    }
}
