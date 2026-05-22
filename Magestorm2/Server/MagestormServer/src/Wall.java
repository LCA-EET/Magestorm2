public class Wall extends DamagingSpell{
    protected byte _elementCode;
    public Wall(MatchCharacter caster, short castID, Spell baseReference, Match matchReference, byte[] prBytes){
        super(caster, castID, baseReference, matchReference);
        _bytes = new byte[27];
        byte[] castIDBytes = ByteUtils.ShortToByteArray(castID);
        _bytes[0] = _spellID;
        System.arraycopy(castIDBytes, 0, _bytes, 1, 2);
        System.arraycopy(prBytes, 0, _bytes, 3, 24);
        _elementCode = _baseReference.GetElement0();
        caster.IncrementWallCount();
        Main.LogDebug("Wall " + _objectID + " created. Duration: " + DurationRemaining());
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
                _matchReference.SendToAll(Packets.TimedObjectExpirationPacket(InGame_Send.WallExpired, _objectID));
                Main.LogDebug("Wall " + _objectID + " has taken " + damageToWall + " damage.");
            }
        }
        else{
            Main.LogDebug("Wall " + _objectID + " hit with same element, zero damage.");
        }
    }
}
