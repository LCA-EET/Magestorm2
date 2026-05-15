public class ResistableSpell extends DamagingSpell{


    public ResistableSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference) {
        super(caster, castID, baseReference, matchReference);
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        if(_casterReference.IsAlive()){
            Effect baseEffect = EffectManager.GetEffect(_baseReference.GetEffectCode());
            byte targetID = affectedPlayer.GetIDinMatch();
            byte statCode = _baseReference.GetDiscipline().GetStatCode();
            if(SharedFunctions.EffectApplied(0.1f, 0.9f, statCode, affectedPlayer, _casterReference)){
                super.ProcessSpell(affectedPlayer);
                _matchReference.SendToAll(Packets.SpawnVFXonPlayerPacket(baseEffect.GetVFXCode(), targetID));
            }
            else{
                byte[] toSend = Packets.SpellResistedPacket(targetID, _casterReference.GetIDinMatch());
                _matchReference.SendToPlayer(toSend, _casterReference);
                _matchReference.SendToPlayer(toSend, affectedPlayer);
            }
        }
    }
}
