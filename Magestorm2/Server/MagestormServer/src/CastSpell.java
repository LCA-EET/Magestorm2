public class CastSpell {
    protected final short _castID;
    protected Spell _baseReference;
    protected MatchCharacter _casterReference;
    protected long _expiration;

    public CastSpell(MatchCharacter caster, short castID, Spell baseReference){
        _casterReference = caster;
        _castID = castID;
        _baseReference = baseReference;
        _expiration = System.currentTimeMillis() + 60000;
    }
    public boolean IsExpired(long currentTimeInMillis){
        return currentTimeInMillis >= _expiration;
    }
    public Spell GetBaseSpell(){
        return _baseReference;
    }
    public short ID(){
        return _castID;
    }
    public MatchCharacter GetCasterReference(){
        return _casterReference;
    }
    public void ProcessSpell(MatchCharacter affectedPlayer){

    }
}
