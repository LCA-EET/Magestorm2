public class Wall extends DamagingSpell implements ITimedObject{
    protected final byte[] _wallBytes;
    protected byte _elementCode;
    protected Duration _duration;
    public Wall(MatchCharacter caster, short castID, Spell baseReference, Match matchReference, byte[] prBytes){
        super(caster, castID, baseReference, matchReference);
        _wallBytes = new byte[27];
        byte[] castIDBytes = ByteUtils.ShortToByteArray(castID);
        _wallBytes[0] = _spellID;
        System.arraycopy(castIDBytes, 0, _wallBytes, 1, 2);
        System.arraycopy(prBytes, 0, _wallBytes, 3, 24);
        _duration = new Duration(_baseReference.GetDuration() * 1000);
        _elementCode = _baseReference.GetElement0();
        caster.IncrementWallCount();
        Main.LogDebug("Wall " + _castID + " created. Duration: " + _duration.DurationRemaining());
    }
    public boolean IsSolidWall(){
        return _baseReference.SpellType() == ControlCodes.SpellTypes_SolidWall;
    }
    @Override
    public boolean ReduceDuration(long msElapsed){
        boolean expired = super.ReduceDuration(msElapsed);
        if(expired){
            _casterReference.DecrementWallCount();
        }
        return expired;
    }
    public void TakeDamage(DamagingSpell spell){
        if(_elementCode != spell.GetBaseSpell().GetElement0()){
            long damageToWall = spell.GetDamage0() * 1000;
            if(ReduceDuration(damageToWall)){
                _matchReference.SendToAll(Packets.WallExpirationPacket(_castID));
                Main.LogDebug("Wall " + _castID + " has taken " + damageToWall + " damage.");
            }
        }
        else{
            Main.LogDebug("Wall " + _castID + " hit with same element, zero damage.");
        }
    }
    public byte[] GetWallBytes(){
        return _wallBytes;
    }
}
