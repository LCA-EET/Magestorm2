public class CastSpell {
    protected final int _castID;
    protected Spell _baseReference;
    protected MatchCharacter _casterReference;

    public CastSpell(MatchCharacter caster, int castID, Spell baseReference){
        _casterReference = caster;
        _castID = castID;
        _baseReference = baseReference;
    }

    public Spell GetBaseSpell(){
        return _baseReference;
    }
}
