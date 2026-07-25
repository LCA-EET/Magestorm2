public class ResistanceSpell extends CastSpell{
    public ResistanceSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference) {
        super(caster, castID, baseReference, matchReference);
    }
    @Override
    protected AppliedEffect CreateEffect(MatchCharacter target, Effect baseEffect){
        return new ResistanceEffect(_casterReference, target, baseEffect, _baseReference,
                _spellLevel, _matchReference.IncrementEffectID());
    }
}
