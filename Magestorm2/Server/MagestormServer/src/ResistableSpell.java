public class ResistableSpell extends DamagingSpell{

    public ResistableSpell(MatchCharacter caster, short castID, Spell baseReference, Match matchReference) {
        super(caster, castID, baseReference, matchReference);
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        byte statCode = _baseReference.GetEffectStatCode();
        if(SharedFunctions.EffectApplied(0.1f, 0.9f, statCode, affectedPlayer, _casterReference)){
            super.ProcessSpell(affectedPlayer);
        }
        else{
            byte[] toSend = Packets.SpellResistedPacket(affectedPlayer.GetIDinMatch(), _casterReference.GetIDinMatch());
            _matchReference.SendToPlayer(toSend, _casterReference);
            _matchReference.SendToPlayer(toSend, affectedPlayer);
        }
    }
}
