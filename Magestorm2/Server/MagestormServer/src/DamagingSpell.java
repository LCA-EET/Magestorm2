public class DamagingSpell extends CastSpell{
    protected short _damage;

    public DamagingSpell(byte casterID, int castID, int baseSpellID){
        super(casterID, castID, baseSpellID);
        CalculateDamage();
    }

    private void CalculateDamage()
    {
        byte minRoll = GetAttribute(SpellAttributes.MinDamagePerRoll);
        byte maxRoll = GetAttribute(SpellAttributes.MaxDamagePerRoll);
        _damage = GameUtils.DiceRoll(minRoll, maxRoll, 1);
    }

    public short GetDamage(){
        return _damage;
    }
}
