public class ResistanceSpell extends CastSpell{
    public ResistanceSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference) {
        super(caster, castID, baseReference, matchReference);
    }
    @Override
    protected AppliedEffect CreateEffect(MatchCharacter target, byte effectCode){
        return new ResistanceEffect(_casterReference, target, _baseReference, _spellLevel, effectCode, _baseReference.GetDuration());
    }
}
