public class DamagingSpell extends CastSpell{
    protected short _damage;

    public DamagingSpell(MatchCharacter caster, short castID, Spell baseReference){
        super(caster, castID, baseReference);
    }

    private void CalculateDamage()
    {
        byte minRoll = _baseReference.GetMinDamagePerRoll();
        byte maxRoll = _baseReference.GetMaxDamagePerRoll();
        _damage = GameUtils.DiceRoll(minRoll, maxRoll, _baseReference.GetNumRolls());
    }

    public short GetDamage(){
        if(_damage == 0){
            CalculateDamage();
        }
        return _damage;
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        Main.LogMessage("PriorHP: " + affectedPlayer.GetCurrentHP());
        short damage = GetDamage();
        affectedPlayer.TakeDamage(damage, _casterReference);
        Main.LogMessage("CurrentHP: " + affectedPlayer.GetCurrentHP());
    }
}
