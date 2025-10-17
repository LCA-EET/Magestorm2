public class CastSpell {
    private final byte _casterID;
    private final int _castID;
    private Spell _baseReference;

    public CastSpell(byte casterID, int castID, int baseSpellID){
        _casterID = casterID;
        _castID = castID;
        _baseReference = SpellManager.GetSpell(baseSpellID);
    }

    public Spell GetBaseSpell(){
        return _baseReference;
    }

    protected byte GetAttribute(byte key)
    {
        return _baseReference.GetAttribute(key);
    }

}
