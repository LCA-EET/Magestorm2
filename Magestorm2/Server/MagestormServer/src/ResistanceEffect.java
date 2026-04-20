public class ResistanceEffect extends AppliedEffect{
    private final float[] _resistances;
    public ResistanceEffect(MatchCharacter caster, MatchCharacter target, Spell baseReference, byte degree, byte effectCode,
                            byte duration) {
        super(caster, target, baseReference, degree, effectCode, duration);
        _resistances = baseReference.GetResistances();
        ApplyResistances(1.0f);
    }
    private void ApplyResistances(float factor){
        for(byte elementID = 0; elementID < _resistances.length; elementID++){
            float resistance = _resistances[elementID];
            if (resistance != 0){
                float adjustment = resistance * factor;
                Main.LogDebug("ResistanceEffect.ApplyResistances(): ElementID: " + elementID + ", Adjustment: " + adjustment);
                _target.AdjustResistance(elementID, adjustment);
            }
        }
    }
    @Override
    public void ReverseEffect(){
        ApplyResistances(-1.0f);
    }
}
