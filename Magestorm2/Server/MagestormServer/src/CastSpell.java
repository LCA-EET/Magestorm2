public class CastSpell {
    protected final short _castID;
    protected Spell _baseReference;
    protected MatchCharacter _casterReference;
    protected long _expiration;
    protected byte _castingTeam;

    public CastSpell(MatchCharacter caster, short castID, Spell baseReference){
        _casterReference = caster;
        _castingTeam = caster.GetTeamID();
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
    public byte GetCasterID(){
        return _casterReference.GetIDinMatch();
    }
    public MatchCharacter GetCasterReference(){
        return _casterReference;
    }
    public byte GetCastingTeam(){
        return _castingTeam;
    }
    public void ProcessSpell(MatchCharacter affectedPlayer){

    }
}
