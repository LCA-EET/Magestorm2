public class DamagingSpell extends CastSpell{
    protected short _damage0, _damage1;
    protected boolean _multiElement;
    public DamagingSpell(MatchCharacter caster, short castID, Spell baseReference){
        super(caster, castID, baseReference);
        _multiElement = (baseReference.GetMinDamagePerRoll1() > 0);
    }

    private void CalculateDamage0()
    {
        byte minRoll0 = _baseReference.GetMinDamagePerRoll0();
        byte maxRoll0 = _baseReference.GetMaxDamagePerRoll0();
        _damage0 = GameUtils.DiceRoll(minRoll0, maxRoll0, _baseReference.GetNumRolls());
    }

    private void CalculateDamage1(){
        byte minRoll1 = _baseReference.GetMinDamagePerRoll1();
        byte maxRoll1 = _baseReference.GetMaxDamagePerRoll1();
        _damage1 = GameUtils.DiceRoll(minRoll1, maxRoll1, _baseReference.GetNumRolls());
    }

    public short GetDamage0(){
        if(_damage0 == 0){
            CalculateDamage0();
        }
        return _damage0;
    }

    public short GetDamage1(){
        if(_damage1 == 0){
            CalculateDamage1();
        }
        return _damage1;
    }

    protected void ApplyDamage(short damage, byte element, MatchCharacter target){
        float appliedDamage = damage;
        byte resistance = target.GetResistance(element);
        if(resistance != 0){
            appliedDamage *= ((100.0f - resistance) / 100.0f);
        }
        target.TakeDamage(appliedDamage, _casterReference);
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        Main.LogMessage("PriorHP: " + affectedPlayer.GetCurrentHP());
        ApplyDamage(GetDamage0(), _baseReference.GetElement0(), affectedPlayer);
        if(_multiElement){
            ApplyDamage(GetDamage1(), _baseReference.GetElement1(), affectedPlayer);
        }
        Main.LogMessage("CurrentHP: " + affectedPlayer.GetCurrentHP());
    }
}
