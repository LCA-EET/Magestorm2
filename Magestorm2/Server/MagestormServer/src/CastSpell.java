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

        if(effectCode > 0){
            byte effectStatCode = _baseReference.GetEffectStatCode();
            Main.LogMessage("Effect stat code: " + effectStatCode);
            byte casterStat = _casterReference.GetStatistic(effectStatCode);
            Main.LogMessage("Caster stat: " + casterStat);
            byte targetStat = target.GetStatistic(effectStatCode);
            Main.LogMessage("Target stat: " + targetStat);
            byte difference = (byte) (casterStat - targetStat);
            Main.LogMessage("Difference: " + difference);
            float chance = (50 + (difference * 10)) / 100.0f;
            Main.LogMessage("Chance of effect: " + chance);
            if(chance > 0.9f){
                chance = 0.9f;
            }
            if(chance < 0.1f){
                chance = 0.1f;
            }
            float random = SharedFunctions.GetRandomFloat();
            if(chance >= random){
                Main.LogMessage("Effect triggered.");
                target.TerminateEffects(_baseReference.GetEffectsCancelled(), _spellID);
                AppliedEffect ae = CreateEffect(target, effectCode);
                target.AddEffect(ae);
                _matchReference.SendToAll(Packets.ApplyEffectPacket(target.GetIDinMatch(), _casterReference.GetIDinMatch(),
                        effectCode, _baseReference.GetDuration(), _spellLevel));
            }
            else{
                Main.LogMessage("Effect not triggered. Chance: " + chance + ", r: " + random);
            }
        }
    }
    protected AppliedEffect CreateEffect(MatchCharacter target, byte effectCode){
        return new AppliedEffect(_casterReference, target, _spellLevel, effectCode, _baseReference.GetDuration());
    }
}
