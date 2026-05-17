public class Sigil extends DamagingSpell{
    private byte _castingTeam;
    private byte[] locationBytes;
    private final Duration _duration;

    public Sigil(MatchCharacter caster, short castID, Spell baseReference, Match matchReference, byte[] payload) {
        super(caster, castID, baseReference, matchReference);
        _duration = new Duration(baseReference.GetDuration() * 1000);
    }
}
