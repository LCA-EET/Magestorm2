public class AppliedEffect {
    private long _timeRemaining;
    private final byte _effectCode;
    protected byte _degree;
    protected MatchCharacter _caster, _target;
    protected Spell _baseReference;

    public AppliedEffect(MatchCharacter caster, MatchCharacter target, Spell baseReference, byte degree, byte effectCode, byte duration){
        Main.LogMessage("Applied " + effectCode + " effect to " + target.GetCharacterName());
        _baseReference = baseReference;
        _timeRemaining = duration * 1000;
        _target = target;
        _caster = caster;
        _degree = degree;
        _effectCode = effectCode;
    }
    public boolean Tick(long deltaTime){
        _timeRemaining -= deltaTime;
        return IsExpired();
    }
    public boolean IsExpired(){
        return _timeRemaining <= 0;
    }
    public byte GetEffectCode(){
        return _effectCode;
    }
    public boolean IsPreventingEffect(byte effectCode){
        return _baseReference.IsEffectPrevented(effectCode);
    }
    public void ReverseEffect(){

    }
}
