public class CastSpell extends TimedObject{
    protected Spell _baseReference;
    protected MatchCharacter _casterReference;
    protected Match _matchReference;
    protected byte _castingTeam, _spellID, _spellLevel;
    public CastSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference){
        _casterReference = caster;
        _matchReference = matchReference;
        _castingTeam = caster.GetTeamID();
        _objectID = castID;
        _spellID = (byte)baseReference.GetSpellID();
        _baseReference = baseReference;
        _spellLevel = _casterReference.GetSkillLevel(_baseReference.GetDisciplineCode());
        byte spellDuration = _baseReference.GetDuration();
        SetDurationRemaining(spellDuration == 0 ? 60000 : spellDuration * 1000);
    }
    public Spell GetBaseSpell(){
        return _baseReference;
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

    protected void ProcessEffect(MatchCharacter spellTarget){
        byte effectCode = _baseReference.GetEffectCode();
        Main.LogMessage("Effect code for base reference " + _baseReference.GetSpellID() + ": " + _baseReference.GetEffectCode());
        boolean successfullyApplied = false;
        MatchCharacter effectTarget;
        if(effectCode > 0 ){
            Effect baseEffect = EffectManager.GetEffect(effectCode);
            if(spellTarget.GetIDinMatch() == _casterReference.GetIDinMatch() || baseEffect.GetEffectTarget() == ControlCodes.EffectTarget_Caster){
                //self-applied effect. Target is the caster.
                successfullyApplied = true;
                effectTarget = _casterReference;
            }
            else{
                if(!spellTarget.IsEffectPrevented(effectCode)){
                    successfullyApplied = _baseReference.SpellType() == ControlCodes.SpellTypes_HarmfulSigil ||
                            SharedFunctions.EffectApplied(0.1f, 0.9f, _baseReference.GetDiscipline().GetStatCode(), spellTarget, _casterReference);
                    effectTarget = spellTarget;
                }
                else{
                    return;
                }
            }
            if(successfullyApplied){
                Main.LogMessage("Effect triggered.");
                effectTarget.TerminateEffects(baseEffect.GetEffectsCancelled());
                effectTarget.AddEffect(CreateEffect(effectTarget, baseEffect));
                byte notificationCode = baseEffect.GetEffectNotificationCode();
                switch(notificationCode){
                    case ControlCodes.EffectNotification_All:
                        _matchReference.SendToAll(Packets.ApplyEffectPacket(effectTarget.GetIDinMatch(), _casterReference.GetIDinMatch(),
                                effectCode, baseEffect.GetDuration(), _spellLevel));
                        break;
                    case ControlCodes.EffectTarget_Caster:
                        _matchReference.SendToPlayer(Packets.ApplyEffectPacket(effectTarget.GetIDinMatch(), spellTarget.GetIDinMatch(),
                                effectCode, baseEffect.GetDuration(), _spellLevel), effectTarget);
                        break;
                }
            }
            else{
                Main.LogMessage("Effect not triggered.");
            }
        }
    }
    protected AppliedEffect CreateEffect(MatchCharacter target, Effect baseEffect){
        return new AppliedEffect(_casterReference, target, baseEffect, _spellLevel);
    }
}
