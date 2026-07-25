public class AppliedEffect extends TimedObject {
    protected byte _degree;
    protected MatchCharacter _caster, _target;
    protected Effect _baseReference;

    public AppliedEffect(MatchCharacter caster, MatchCharacter target, Effect baseReference, byte degree, short effectID){
        _baseReference = baseReference;
        _target = target;
        _caster = caster;
        _degree = degree;
        _objectID = effectID;
        SetDurationRemaining(_baseReference.GetDuration() * 1000);
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
