import java.util.ArrayList;

public class CastSpell {
    protected final short _castID;
    protected Spell _baseReference;
    protected MatchCharacter _casterReference;
    protected Match _matchReference;
    protected long _expiration;
    protected byte _castingTeam, _spellID, _spellLevel;

    public CastSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference){
        _casterReference = caster;
        _matchReference = matchReference;
        _castingTeam = caster.GetTeamID();
        _castID = castID;
        _spellID = (byte)baseReference.GetSpellID();
        _baseReference = baseReference;
        _spellLevel = _casterReference.GetSkillLevel(_baseReference.GetDiscipline());
        _expiration = System.currentTimeMillis() + 60000;
    }
    public boolean IsExpired(long currentTimeInMillis){
        return currentTimeInMillis >= _expiration;
    }
    public Spell GetBaseSpell(){
        return _baseReference;
    }
    public short CastID(){
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
    public byte GetSpellID(){
        return _spellID;
    }

    public void ProcessSpell(MatchCharacter affectedPlayer){
        ProcessEffect(affectedPlayer);
    }
    private void DetermineEffectChance(){

    }
    protected void ProcessEffect(MatchCharacter target){
        byte effectCode = _baseReference.GetEffectCode();
        Main.LogMessage("Effect code for base reference " + _baseReference.GetSpellID() + ": " + _baseReference.GetEffectCode());
        boolean successfullyApplied;
        float chance, random;
        if(effectCode > 0 ){
            if(target.GetIDinMatch() == _casterReference.GetIDinMatch()){
                //self-applied effect
                successfullyApplied = true;
            }
            else{
                if(!target.IsEffectPrevented(effectCode)){
                    successfullyApplied = SharedFunctions.EffectApplied(0.1f, 0.9f, _baseReference.GetEffectStatCode(), target, _casterReference);
                }
                else{
                    return;
                }
            }
            if(successfullyApplied){
                Main.LogMessage("Effect triggered.");
                target.TerminateEffects(_baseReference.GetEffectsCancelled(), _spellID);
                target.AddEffect(CreateEffect(target, effectCode));
                _matchReference.SendToAll(Packets.ApplyEffectPacket(target.GetIDinMatch(), _casterReference.GetIDinMatch(),
                        effectCode, _baseReference.GetDuration(), _spellLevel));
            }
            else{
                Main.LogMessage("Effect not triggered.");
            }
        }
    }
    protected AppliedEffect CreateEffect(MatchCharacter target, byte effectCode){
        return new AppliedEffect(_casterReference, target, _baseReference, _spellLevel, effectCode, _baseReference.GetDuration());
    }
}
