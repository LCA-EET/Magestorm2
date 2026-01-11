public class DamagingSpell extends CastSpell{
    protected short _damage;

    public DamagingSpell(MatchCharacter caster, int castID, Spell baseReference){
        super(caster, castID, baseReference);
        CalculateDamage();
    }

    private void CalculateDamage()
    {
        byte minRoll = _baseReference.GetMinDamagePerRoll();
        byte maxRoll = _baseReference.GetMaxDamagePerRoll();
        _damage = GameUtils.DiceRoll(minRoll, maxRoll, _baseReference.GetNumRolls());
    }

    public short GetDamage(){
        return _damage;
    }
}
