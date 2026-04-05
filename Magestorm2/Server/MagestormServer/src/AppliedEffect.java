public class AppliedEffect {
    private long _timeRemaining;
    private final byte _effectCode;
    private byte _degree;
    protected MatchCharacter _caster, _target;
    public AppliedEffect(MatchCharacter caster, MatchCharacter target, byte degree, byte effectCode, byte duration){
        Main.LogMessage("Applied " + effectCode + " effect to " + target.GetCharacterName());
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
}
