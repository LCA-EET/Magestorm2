public class Sigil extends DamagingSpell{

    public Sigil(MatchCharacter caster, short castID, Spell baseReference, Match matchReference, byte[] decrypted) {
        super(caster, castID, baseReference, matchReference);
        _bytes = new byte[1 + 2 + 1 + 12];
        _bytes[0] = _spellID;
        ByteUtils.WriteShortAtIndex_LE(_bytes, castID, 1);
        _bytes[3] = decrypted[ControlCodes.CastPayloadStartIndex]; // teamID
        System.arraycopy(decrypted,ControlCodes.CastPayloadStartIndex+1,_bytes, 4, 12);
        caster.IncrementSigilCount();
    }

    public void SigilTriggered(MatchCharacter mc)
    {
        ProcessSpell(mc);
        _matchReference.SendToAll(Packets.TimedObjectExpirationPacket(InGame_Send.SigilExpired, _objectIDAsShort));
        SetDurationRemaining(0);
    }
    @Override
    public boolean ReduceDuration(long msElapsed){
        boolean expired = super.ReduceDuration(msElapsed);
        if(expired){
            _casterReference.DecrementSigilCount();
        }
        return expired;
    }
}
