public class CastSpell {
    protected final byte _casterID;
    protected final int _castID;
    protected Spell _baseReference;

    public CastSpell(byte casterID, int castID, int baseSpellID){
        _casterID = casterID;
        _castID = castID;
        _baseReference = SpellManager.GetSpell(baseSpellID);
    }

    public Spell GetBaseSpell(){
        return _baseReference;
    }
}
