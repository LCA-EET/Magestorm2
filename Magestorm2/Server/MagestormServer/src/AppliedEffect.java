public class AppliedEffect {
    private long _timeRemaining;
    protected byte _degree;
    protected MatchCharacter _caster, _target;
    protected Effect _baseReference;

    public AppliedEffect(MatchCharacter caster, MatchCharacter target, Effect baseReference, byte degree){
        _baseReference = baseReference;
        _timeRemaining = baseReference.GetDuration() * 1000;
        _target = target;
        _caster = caster;
        _degree = degree;
    }
    public boolean Tick(long deltaTime){
        _timeRemaining -= deltaTime;
        return IsExpired();
    }
    public boolean IsExpired(){
        return _timeRemaining <= 0;
    }
    public byte GetEffectCode(){
        return _baseReference.GetEffectCode();
    }
    public boolean IsPreventingEffect(byte effectCode){
        return _baseReference.IsEffectPrevented(effectCode);
    }
    public void ReverseEffect(){

    }
}
