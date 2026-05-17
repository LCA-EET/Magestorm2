public class Duration {
    private long _durationRemaining;

    public Duration(long durationRemaining){
        SetDurationRemaining(durationRemaining);
    }

    public boolean ReduceDuration(long msReduction){
        _durationRemaining -= msReduction;
        return _durationRemaining < 0;
    }

    public long DurationRemaining(){
        return _durationRemaining;
    }

    public void SetDurationRemaining(long durationRemaining){
        _durationRemaining = durationRemaining;
    }

    public boolean DurationExpired(){
        return _durationRemaining <= 0;
    }
}
