public class Sigil extends DamagingSpell{
    private byte _castingTeam;
    private byte[] locationBytes;

    public Sigil(MatchCharacter caster, short castID, Spell baseReference, Match matchReference, byte[] payload) {
        super(caster, castID, baseReference, matchReference);
    }
}
